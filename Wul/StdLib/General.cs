using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [MagicNetFunction("let")]
        internal static IValue Let(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();
            var nameIdentifier = (IdentifierNode) children[0];
            string name = nameIdentifier.Name;
            var value = WulInterpreter.Interpret(children[1], scope) ?? Value.Nil;

            Scope currentScope = scope.EmptyChildScope();
            currentScope[name] = value;

            return children[2].Eval(currentScope);
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

        [NetFunction("unpack")]
        internal static IValue Unpack(List<IValue> list, Scope scope)
        {
            if (!(list[0] is ListTable listTable)) return Value.Nil;

            UnpackList unpack = new UnpackList(listTable);

            return unpack;
        }

        [MagicNetFunction("time")]
        internal static IValue Time(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            var sw = Stopwatch.StartNew();
            foreach (SyntaxNode child in children)
            {
                child.Eval(scope);
            }
            sw.Stop();

            return (Number)sw.ElapsedMilliseconds;
        }

        [MagicNetFunction("global")]
        internal static IValue Global(ListNode list, Scope scope)
        {
            Scope rootScope = scope;
            while (rootScope.Parent != null)
            {
                rootScope = rootScope.Parent;
            }

            IdentifierNode identifier = (IdentifierNode)list.Children[1];
            if (list.Children.Count == 2)
            {
                return identifier.Eval(rootScope);
            }
            IValue value = list.Children[2].Eval(scope);
            rootScope[identifier.Name] = value;
            return value;
        }
    }
}
