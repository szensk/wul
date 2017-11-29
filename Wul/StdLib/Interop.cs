using System;
using System.Linq;
using System.Reflection;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    class Interop
    {
        private static ILookup<string, Type> _allTypes;

        private static ILookup<string, Type> AllTypes => _allTypes ?? (_allTypes = AppDomain.CurrentDomain.GetAssemblies()
                                               .SelectMany(a => a.GetTypes()).ToLookup(key => key.FullName));

        private static object InvokeNetFunction(string name, params object[] arguments)
        {
            //TODO no dots?
            int lastDot = name.LastIndexOf('.');
            string className = name.Substring(0, lastDot);
            string methodName = name.Substring(lastDot + 1);

            var types = AllTypes[className];
            Type[] argTypes = arguments.Select(a => a.GetType()).ToArray();

            //TODO if not a method?
            foreach (var type in types)
            {
                MethodInfo methodInfo = null;
                try
                {
                    methodInfo = type.GetMethod(methodName, argTypes);
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
                    if (arguments.Length > 0)
                    {
                        methodInfo = propertyInfo.SetMethod;
                    }
                    else
                    {
                        methodInfo = propertyInfo.GetMethod;
                    }
                }

                if (methodInfo != null)
                {
                    return methodInfo.Invoke(null, arguments);
                }
            }
            throw new Exception($"Method {methodName} not found in {className}");
        }

        private static object NewObject(string className, params object[] arguments)
        {
            var types = AllTypes[className];
            var type = types.First();
            return Activator.CreateInstance(type, arguments);
        }

        [MagicNetFunction("::")]
        internal static IValue CallFrameworkFunction(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            var nameIdentifier = (IdentifierNode)children[0];
            string name = nameIdentifier.Name;

            object result;
            if (children.Length > 1)
            {
                object[] evaluatedArguments = children.Skip(1).Select(s => s.Eval(scope).ToObject()).ToArray();

                result = InvokeNetFunction(name, evaluatedArguments);
            }
            else
            {
                result = InvokeNetFunction(name);
            }

            //TODO find a better way
            //TODO IEnumerables to Lists
            switch (result)
            {
                case string s:
                    return new UString(s);
                case bool b:
                    return b ? Bool.True : Bool.False;
                case double d:
                    return (Number)d;
                case int i:
                    return (Number)i;
                case int[] ia:
                    return new ListTable(ia.Select(n => (Number)n));
                case null:
                    return Value.Nil;
                default:
                    return new NetObject(result);
            }
        }

        [MagicNetFunction("new-object")]
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
    }
}
