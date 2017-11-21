using System.Linq;
using Wul.Interpreter;
using Wul.Parser;

namespace Wul.StdLib
{
    class General
    {
        //TODO Convert to magic function so that identifer doesn't need quoting
        internal static IFunction Let = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();
            var nameIdentifier = children[0] as IdentifierNode;
            string name = nameIdentifier.Name;
            var value = WulInterpreter.Interpret(children[1], scope);
            scope[name] = value;
            return value;
        }, "let");

        internal static IFunction Define = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            var nameIdentifier = children[0] as IdentifierNode;
            string name = nameIdentifier.Name;

            var arguments = children[1] as ListNode;
            var argNames = arguments.Children.Select(a => a as IdentifierNode).Select(a => a.Name);

            var body = children[2] as ListNode;
            var function = new Function(body, name, argNames.ToList());
            scope[name] = function;

            return function;
        }, "def");
    }
}
