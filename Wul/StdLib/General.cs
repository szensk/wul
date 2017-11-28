using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    class General
    {
        [GlobalName("identity")]
        internal static IFunction Identity = new NetFunction((list, scope) => list.FirstOrDefault() ?? Value.Nil, "identity");

        [GlobalName("def")]
        internal static IFunction Define = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();
            var nameIdentifier = (IdentifierNode) children[0];
            string name = nameIdentifier.Name;
            var value = WulInterpreter.Interpret(children[1], scope) ?? Value.Nil;

            if (value == Value.Nil)
            {
                scope.Remove(name);
            }
            else
            {
                scope[name] = value;
            }

            return value;
        }, "def");

        [GlobalName("defn")]
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

        [GlobalName("lambda")]
        internal static IFunction Lambda = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            var arguments = (ListNode) children[0];
            var argNames = arguments.Children.OfType<IdentifierNode>().Select(a => a.Name);

            var body = (ListNode) children[1];
            var function = new Function(body, "unnamed function", argNames.ToList());

            return function;
        }, "lambda");

        [GlobalName("@defn")]
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

        [GlobalName("then")]
        [GlobalName("else")]
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

        [GlobalName("if")]
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

        [GlobalName("eval")]
        internal static IFunction Evaluate = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            return children[0].Eval(scope);
        }, "eval");

        [GlobalName("??")]
        internal static IFunction Coalesce = new NetFunction((list, scope) =>
        {
            IValue firstNonNull = list.FirstOrDefault(i => i != Value.Nil);

            if (firstNonNull != null)
            {
                return firstNonNull;
            }
            else
            {
                return Value.Nil;
            }
        }, "??");

        [GlobalName("type")]
        internal static IFunction Type = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Type.Invoke(list, scope);
        }, "type");

        [GlobalName("quote")]
        internal static IFunction Quote = new MagicNetFunction((list, scope) =>
        {
            var children = list.Children.Skip(1).ToArray();

            return children[0];
        }, "quote");

        [GlobalName("exit")]
        internal static IFunction Exit = new NetFunction((list, scope) =>
        {
            Number code = list.First() as Number;

            Environment.Exit((int)code.Value);

            return code;
        }, "exit");

        //I don't like this macro
        [GlobalName("unpack")]
        internal static IFunction Unpack = new MagicNetFunction((list, scope) =>
        {
            ListNode listToUnpack  = (ListNode) list.Children[1].Eval(scope).ToSyntaxNode(list.Parent);
            ListNode replaceInList = (ListNode) listToUnpack.Parent;
            List<SyntaxNode> replacementList = new List<SyntaxNode>();

            foreach (SyntaxNode node in replaceInList.Children)
            {
                if (node == list)
                {
                    replacementList.AddRange(listToUnpack.Children);
                }
                else
                {
                    replacementList.Add(node);
                }
            }
            replaceInList.Children = replacementList;

            return replaceInList.Eval(scope);
        }, "unpack");
    }
}
