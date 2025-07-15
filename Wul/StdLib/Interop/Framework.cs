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

            Converters = [];
            
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

            Converters = Converters.OrderByDescending(kvp => kvp.Value.Priority).ToDictionary();
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

        private static MethodInfo FindMethodWithTypes(Scope scope, string name, params Type[] argTypes)
        {
            MethodInfo methodInfo = null;
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
                
                foreach (var type in types)
                {
                    try
                    {
                        methodInfo = type.GetMethod(methodName, argTypes);
                        if (methodInfo == null)
                        {
                            methodInfo = type.GetMethod(methodName, argTypes);
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
                        methodInfo = argTypes.Length > 0 ? propertyInfo.SetMethod : propertyInfo.GetMethod;
                    }
                }
            }
            
            return methodInfo;
        }

        //TODO load assemblies on demand
        //TODO cache method resolution: stored at a scope level, must know the name and the argument types
        private static object InvokeNetFunction(Scope scope, string name, params IValue[] arguments)
        {
            var objectArguments = arguments.Select(a => a.ToObject()).ToArray();
            Type[] argTypes = objectArguments.Select(a => a.GetType()).ToArray();

            var methodInfo = FindMethodWithTypes(scope, name, argTypes);
            if (methodInfo != null)
            {
                if (methodInfo.IsStatic)
                {
                    return methodInfo.Invoke(null, objectArguments);
                } 
                else
                {
                    return methodInfo.Invoke(objectArguments[0], objectArguments.Skip(1).ToArray());
                }
            }
            else
            {
                throw new MethodNotFoundException($"Method {name} not found");
            }
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

        private static Func<List<IValue>, Scope, IValue> CreateNetFunction(MethodInfo methodInfo)
        {
            if (methodInfo.IsStatic)
            {
                IValue InvokeStatic(List<IValue> list, Scope s)
                {
                    object result = methodInfo.Invoke(null, list.Select(l => l.ToObject()).ToArray());
                    return ConvertToIValue(result);
                }
                return InvokeStatic;
            }
            else
            {
                IValue InvokeMethod(List<IValue> list, Scope s)
                {
                    object result = methodInfo.Invoke(list[0].ToObject(), list.Skip(1).Select(l => l.ToObject()).ToArray());
                    return ConvertToIValue(result);
                }
                return InvokeMethod;
            }
        }

        //(::defn Math.Max System.Math.Max System.Decimal System.Decimal)
        [MagicFunction("::defn")]
        internal static IValue ImportFunction(ListNode list, Scope scope)
        {
            int children = list.Children.Count - 1;

            if (children < 1)
            {
                throw new ArgumentException("Too few parameters");
            }

            var args = list.Children.Skip(1).ToArray();
            var nameIdentifier = (IdentifierNode) args[0];
            string name = nameIdentifier.Name;

            var methodIdentifier = (IdentifierNode) args[1];
            string methodName = methodIdentifier.Name;

            var argTypes = new Type[children > 2 ? children - 2 : 0];

            int startIndex = 2;
            for (int i = startIndex; i < children; i++)
            {
                Type type = null;
                if (args[i] is IdentifierNode typeIdentifier)
                {
                    var types = AllTypes[typeIdentifier.Name];
                    type = types.FirstOrDefault();
                    argTypes[i - startIndex] = type;
                }
                if (type == null)
                {
                    IValue result = Helpers.EvalOnce(args[i], scope);
                    var types = AllTypes[(result as IdentifierNode).Name];
                    type = types.FirstOrDefault();
                }
                argTypes[i - startIndex] = type;
            }

            var method = FindMethodWithTypes(scope, methodName, argTypes);
            scope[name] = Value.Nil;
            var function = new NetFunction(CreateNetFunction(method), methodName);
            scope[name] = function;

            return function;
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
