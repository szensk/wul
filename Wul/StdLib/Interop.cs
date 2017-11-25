using System;
using System.Linq;
using Wul.Interpreter;
using Wul.Parser;

namespace Wul.StdLib
{
    class Interop
    {
        private static ILookup<string, Type> _allTypes = null;

        private static ILookup<string, Type> AllTypes => _allTypes ?? (_allTypes = AppDomain.CurrentDomain.GetAssemblies()
                                               .SelectMany(a => a.GetTypes()).ToLookup(key => key.FullName));

        private static object InvokeNetFunction(string name, params object[] arguments)
        {
            //TODO no dots?
            int lastDot = name.LastIndexOf('.');
            string className = name.Substring(0, lastDot);
            string methodName = name.Substring(lastDot + 1);

            //TODO type not found handling
            Type t = AllTypes[className].First();
            Type[] argTypes = arguments.Select(a => a.GetType()).ToArray();

            //TODO if not a method
            return t.GetMethod(methodName, argTypes).Invoke(null, arguments);
        }

        internal static IFunction CallFrameworkFunction = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            var nameIdentifier = (IdentifierNode)children[0];
            string name = nameIdentifier.Name;

            var arguments = (ListNode)children[1];
            IValue evaluatedArguments = WulInterpreter.Interpret(arguments, scope) ?? Value.Nil;
            while (evaluatedArguments is SyntaxNode)
            {
                evaluatedArguments = WulInterpreter.Interpret(evaluatedArguments as SyntaxNode, scope) ?? Value.Nil;
            }
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
