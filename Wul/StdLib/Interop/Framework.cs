using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;
using Wul.StdLib.Attribute;

namespace Wul.StdLib.Interop
{
    [StdLib]
    internal class Framework
    {
        private static ILookup<string, Type> _allTypes;

        private static ILookup<string, Type> AllTypes => _allTypes ?? (_allTypes = LoadAllTypes());

        private static void LoadAssemblies()
        {
            var referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();

            foreach (AssemblyName assembly in referencedAssemblies)
            {
                System.Diagnostics.Debug.WriteLine($"Loading assembly {assembly.Name}");
                Assembly.Load(assembly);
            }
        }

        private static ILookup<string, Type> LoadAllTypes()
        {
            LoadAssemblies();
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()).ToLookup(key => key.FullName);
        }

        private static Dictionary<Type, IValueConverter> Converters;

        private static void LoadConvertersIfNeeded()
        {
            if (Converters != null) return;

            Converters = new Dictionary<Type, IValueConverter>();
            
            var types = Assembly.GetAssembly(typeof(Global)).GetTypes();
            var converterType = typeof(IValueConverter);
            var converters = types.Where(t => !t.IsAbstract && converterType.IsAssignableFrom(t));

            //Create a single instance of all converters
            foreach (var converter in converters)
            {
                var b = converter.BaseType;
                var netType = b.GenericTypeArguments.First();
                var instance = (IValueConverter) Activator.CreateInstance(converter);
                Converters[netType] = instance;
            }
        }

        private class MethodNotFoundException : Exception
        {
            public MethodNotFoundException(string message) : base(message)
            {
                
            }
        }

        public static IValue ConvertToIValue(object o)
        {
            if (o == null) return Value.Nil;

            LoadConvertersIfNeeded();
            var type = o.GetType();
            foreach (var converter in Converters)
            {
                if (converter.Key.IsAssignableFrom(type))
                {
                    return converter.Value.ConvertToIValue(o);
                }
            }
            
            return new NetObject(o);
        }

        //TODO load assemblies on demand
        //TODO cache method resolution: stored at a scope level, must know the name and the argument types
        private static object InvokeNetFunction(Scope scope, string name, params IValue[] arguments)
        {
            var usings = new List<string> {""};
            usings.AddRange(scope.Usings);

            foreach(string use in usings)
            {
                string fullName = string.IsNullOrEmpty(use) ? name : use + "." + name;
                int lastDot = fullName.LastIndexOf('.');
                if (lastDot == -1) continue;
                string className = fullName.Substring(0, lastDot);
                string methodName = fullName.Substring(lastDot + 1);

                var types = AllTypes[className];
                var objectArguments = arguments.Select(a => a.ToObject()).ToArray();
                Type[] argTypes = objectArguments.Select(a => a.GetType()).ToArray();
                
                foreach (var type in types)
                {
                    MethodInfo methodInfo = null;
                    try
                    {
                        methodInfo = type.GetMethod(methodName, argTypes);
                        if (methodInfo == null)
                        {
                            objectArguments = arguments.Select(a =>
                            {
                                if (a is Number n) return (int) n.Value;
                                return a.ToObject();
                            }).ToArray();
                            methodInfo = type.GetMethod(methodName, objectArguments.Select(a => a.GetType()).ToArray());
                        }
                    }
                    catch
                    {
                        //We might want to display something
                    }

                    PropertyInfo propertyInfo = null;
                    try
                    {
                        propertyInfo = type.GetProperty(methodName);
                    }
                    catch
                    {
                        //We might want to display something
                    }

                    if (propertyInfo != null)
                    {
                        methodInfo = arguments.Length > 0 ? propertyInfo.SetMethod : propertyInfo.GetMethod;
                    }

                    if (methodInfo != null)
                    {
                        return methodInfo.Invoke(null, objectArguments);
                    }
                }
            }
            throw new MethodNotFoundException($"Method {name} not found");
        }

        private static object NewObject(string className, params object[] arguments)
        {
            var types = AllTypes[className];
            var type = types.FirstOrDefault();
            if (type == null)
            {
                throw new Exception($"Cannot find type {className}");
            }
            return Activator.CreateInstance(type, arguments);
        }

        [MagicFunction("::")]
        internal static IValue CallFrameworkFunction(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            var nameIdentifier = (IdentifierNode)children[0];
            string name = nameIdentifier.Name;

            object result;
            if (children.Length > 1)
            {
                var evaluatedArguments = children.Skip(1).Select(s => s.Eval(scope)).ToArray();
                result = InvokeNetFunction(scope, name, evaluatedArguments);
            }
            else
            {
                result = InvokeNetFunction(scope, name);
            }

            return ConvertToIValue(result);
        }

        [MagicFunction("::new")]
        internal static IValue NewObject(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            var nameIdentifier = (IdentifierNode)children[0];
            string name = nameIdentifier.Name;

            object result;
            if (children.Length > 1)
            {
                object[] evaluatedArguments = children.Skip(1).Select(s => s.Eval(scope).ToObject()).ToArray();

                result = NewObject(name, evaluatedArguments);
            }
            else
            {
                result = NewObject(name);
            }

            return result == null ? (IValue) Value.Nil : new NetObject(result);
        }

        [NetFunction("convert")]
        internal static IValue Convert(List<IValue> list, Scope scope)
        {
            if (list.Count == 1 && list.First() is NetObject n)
            {
                return ConvertToIValue(n.ToObject());
            }

            IEnumerable<IValue> items = list.Select(item =>
            {
                var netObject = item as NetObject;
                if (netObject == null) return item;
                return ConvertToIValue(netObject.ToObject());
            });

            return new ListTable(items);
        }

        [MagicFunction("using")]
        internal static IValue Using(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            foreach (var value in children)
            {
                if (value is IdentifierNode id)
                {
                    scope.Usings.Add(id.Name);
                }
            }
            return Value.Nil;
        }
    }
}
