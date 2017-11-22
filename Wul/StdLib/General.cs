using System.Linq;
using Wul.Interpreter;
using Wul.Parser;

namespace Wul.StdLib
{
    class General
    {
        internal static IFunction Identity = new NetFunction((list, scope) => list.FirstOrDefault() ?? Value.Nil, "identity");

        internal static IFunction Let = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();
            var nameIdentifier = children[0] as IdentifierNode;
            string name = nameIdentifier.Name;
            var value = WulInterpreter.Interpret(children[1], scope) ?? Value.Nil;
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

        internal static IFunction Lambda = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            var arguments = children[0] as ListNode;
            var argNames = arguments.Children.Select(a => a as IdentifierNode).Select(a => a.Name);

            var body = children[1] as ListNode;
            var function = new Function(body, "unnamed function", argNames.ToList());

            return function;
        }, "lambda");

        internal static IFunction DefineMagicFunction = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            var nameIdentifier = children[0] as IdentifierNode;
            string name = nameIdentifier.Name;

            var arguments = children[1] as ListNode;
            var argNames = arguments.Children.Select(a => a as IdentifierNode).Select(a => a.Name);

            var body = children[2] as ListNode;
            var function = new MagicFunction(body, name, argNames.ToList());
            scope[name] = function;

            return function;
        }, "def!");

        internal static IFunction Then = new NetFunction((list, scope) =>
        {
            if (list.Count == 1)
            {
                return list.First();
            }
            else
            {
                return new ListTable(list.ToArray());
            }
        }, "then/else");

        internal static IFunction If = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            var condition = children[0];
            var result = WulInterpreter.Interpret(condition, scope) ?? Value.Nil;

            var listChildren = children.OfType<ListNode>();

            IValue returnValue = Value.Nil;
            if (result != Value.Nil && result != Bool.False)
            {
                var thenBlock = listChildren.FirstOrDefault(l => (l.Children.First() as IdentifierNode)?.Name == "then");
                if (thenBlock != null) returnValue = WulInterpreter.Interpret(thenBlock, scope);
            }
            else
            {
                var elseBlock = listChildren.FirstOrDefault(l => (l.Children.First() as IdentifierNode)?.Name == "else");
                if (elseBlock != null) returnValue = WulInterpreter.Interpret(elseBlock, scope);
            }

            return returnValue;
        }, "if");

        internal static IFunction Evaluate = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            IValue result = WulInterpreter.Interpret(children[0], scope) ?? Value.Nil;
            while (result is SyntaxNode)
            {
                result = WulInterpreter.Interpret(result as SyntaxNode, scope) ?? Value.Nil;
            }
            return result;
        }, "eval");
    }
}
