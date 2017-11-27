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
                    
                }

                if (propertyInfo != null)
                {
                    methodInfo = propertyInfo.SetMethod;
                }

                if (methodInfo != null)
                {
                    return methodInfo.Invoke(null, arguments);
                }
            }
            return null;
        }

        internal static IFunction CallFrameworkFunction = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            var nameIdentifier = (IdentifierNode)children[0];
            string name = nameIdentifier.Name;

            var arguments = (ListNode)children[1];
            IValue evaluatedArguments = arguments.Eval(scope);
            var finalArguments = ((ListTable)evaluatedArguments).AsList().Select(i => i.ToObject()).ToArray();

            object result = InvokeNetFunction(name, finalArguments);

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
                default:
                    return Value.Nil;
            }
        }, "::");
    }
}
