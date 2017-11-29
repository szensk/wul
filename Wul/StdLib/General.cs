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
        [NetFunction("identity")]
        internal static IValue Identity(List<IValue> list, Scope scope)
        {
            return list.FirstOrDefault() ?? Value.Nil;
        }

        [MagicNetFunction("def")]
        internal static IValue Define(ListNode list, Scope scope)
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
        }

        [MagicNetFunction("defn")]
        internal static IValue DefineFunction(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            var nameIdentifier = (IdentifierNode) children[0];
            string name = nameIdentifier.Name;

            var arguments = (ListNode) children[1];
            var argNames = arguments.Children.OfType<IdentifierNode>().Select(a => a.Name);

            var body = (ListNode) children[2];
            var function = new Function(body, name, argNames.ToList(), scope);
            scope[name] = function;

            return function;
        }

        [MagicNetFunction("lambda")]
        internal static IValue Lambda(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            var arguments = (ListNode) children[0];
            var argNames = arguments.Children.OfType<IdentifierNode>().Select(a => a.Name);

            var body = (ListNode) children[1];
            var function = new Function(body, "unnamed function", argNames.ToList(), scope);

            return function;
        }

        [MagicNetFunction("@defn")]
        internal static IValue DefineMagicFunction(ListNode list, Scope scope)
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
        }

        [NetFunction("then")]
        [NetFunction("else")]
        internal static IValue Then(List<IValue> list, Scope scope)
        {
            if (list.Count == 1)
            {
                return list.First();
            }
            else
            {
                return new ListTable(list.ToArray());
            }
        }

        [MagicNetFunction("if")]
        internal static IValue If(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            var condition = children[0];
            var result = WulInterpreter.Interpret(condition, scope) ?? Value.Nil;

            var listChildren = children.OfType<ListNode>();

            IValue returnValue = Value.Nil;
            if (result != Value.Nil && result != Bool.False)
            {
                var thenBlock =
                    listChildren.FirstOrDefault(l => (l.Children.First() as IdentifierNode)?.Name == "then");
                if (thenBlock != null) returnValue = WulInterpreter.Interpret(thenBlock, scope);
            }
            else
            {
                var elseBlock =
                    listChildren.FirstOrDefault(l => (l.Children.First() as IdentifierNode)?.Name == "else");
                if (elseBlock != null) returnValue = WulInterpreter.Interpret(elseBlock, scope);
            }

            return returnValue;
        }

        [MagicNetFunction("eval")]
        internal static IValue Evaluate(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            return children[0].Eval(scope);
        }

        [NetFunction("??")]
        internal static IValue Coalesce(List<IValue> list, Scope scope)
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
        }

        [NetFunction("type")]
        internal static IValue Type(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Type.Invoke(list, scope);
        }

        [MagicNetFunction("quote")]
        internal static IValue Quote(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            return children[0];
        }

        [NetFunction("exit")]
        internal static IValue Exit(List<IValue> list, Scope scope)
        {
            Number code = list.FirstOrDefault() as Number;

            double exitCode = code?.Value ?? 0; 
            Environment.Exit((int) exitCode);

            return (Number) exitCode;
        }

        //I don't like this macro
        [MagicNetFunction("unpack")]
        internal static IValue Unpack(ListNode list, Scope scope)
        {
            ListNode listToUnpack  = (ListNode)list.Children[1].Eval(scope).ToSyntaxNode(list.Parent);
            ListNode replaceInList = (ListNode)listToUnpack.Parent;
            List<SyntaxNode> originalList = replaceInList.Children;
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

            IValue result = replaceInList.Eval(scope);
            replaceInList.MacroResult = result;
            replaceInList.Children = originalList;
            return result;
        }
    }
}
