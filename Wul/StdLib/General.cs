using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    class General
    {
        internal static IFunction Identity = new NetFunction((list, scope) => list.FirstOrDefault() ?? Value.Nil, "identity");

        internal static IFunction Define = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();
            var nameIdentifier = (IdentifierNode) children[0];
            string name = nameIdentifier.Name;
            var value = WulInterpreter.Interpret(children[1], scope) ?? Value.Nil;
            scope[name] = value;
            return value;
        }, "def");

        internal static IFunction DefineFunction = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            var nameIdentifier = (IdentifierNode) children[0];
            string name = nameIdentifier.Name;

            var arguments = (ListNode) children[1];
            var argNames = arguments.Children.OfType<IdentifierNode>().Select(a => a.Name);

            var body = (ListNode) children[2];
            var function = new Function(body, name, argNames.ToList());
            scope[name] = function;

            return function;
        }, "defn");

        internal static IFunction Lambda = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            var arguments = (ListNode) children[0];
            var argNames = arguments.Children.OfType<IdentifierNode>().Select(a => a.Name);

            var body = (ListNode) children[1];
            var function = new Function(body, "unnamed function", argNames.ToList());

            return function;
        }, "lambda");

        internal static IFunction DefineMagicFunction = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            var nameIdentifier = (IdentifierNode) children[0];
            string name = nameIdentifier.Name;

            var arguments = (ListNode) children[1];
            var argNames = arguments.Children.OfType<IdentifierNode>().Select(a => a.Name);

            var body = (ListNode) children[2];
            var function = new MagicFunction(body, name, argNames.ToList());
            scope[name] = function;

            return function;
        }, "@defn");

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

            return children[0].Eval(scope);
        }, "eval");

        internal static IFunction Coalesce = new NetFunction((list, scope) =>
        {
            var firstNonNull = list.FirstOrDefault(i => i != Value.Nil);

            if (firstNonNull != null)
            {
                return firstNonNull;
            }
            else
            {
                return Value.Nil;
            }
        }, "??");

        internal static IFunction Type = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Type.Invoke(list, scope);
        }, "type");

        internal static IFunction Quote = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            return children[0];
        }, "quote");
    }
}
