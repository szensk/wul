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

        [MagicFunction("def")]
        internal static IValue Define(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();
            var nameIdentifier = (IdentifierNode) children[0];
            string name = nameIdentifier.Name;
            var value = children[1].EvalOnce(scope);

            if (ReferenceEquals(value, Value.Nil))
            {
                scope.Remove(name);
            }
            else
            {
                scope[name] = value;
            }

            return value;
        }

        [MagicFunction("let")]
        internal static IValue Let(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();
            var nameIdentifier = (IdentifierNode) children[0];
            string name = nameIdentifier.Name;
            var value = children[1].EvalOnce(scope);

            //Do we want a closure or an empty child scope?
            Scope currentScope = scope.EmptyChildScope();
            currentScope[name] = value;

            var childrenToEval = children.Skip(2);
            IValue result = Value.Nil;
            foreach (var child in childrenToEval)
            {
                //TODO Should probably be eval once
                result = child.Eval(currentScope);
            }
            return result;
        }

        [MagicFunction("defn")]
        internal static IValue DefineFunction(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            var nameIdentifier = (IdentifierNode) children[0];
            string name = nameIdentifier.Name;

            var arguments = (ListNode) children[1];
            var argNames = arguments.Children.OfType<IdentifierNode>().Select(a => a.Name);

            var body = (ListNode) children[2];
            scope[name] = Value.Nil;
            var function = new Function(body, name, argNames.ToList(), scope);
            scope[name] = function;

            return function;
        }

        [MagicFunction("lambda")]
        internal static IValue Lambda(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            var arguments = (ListNode) children[0];
            var argNames = arguments.Children.OfType<IdentifierNode>().Select(a => a.Name);

            var body = (ListNode) children[1];
            var function = new Function(body, "unnamed function", argNames.ToList(), scope);

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

        [MagicFunction("if")]
        internal static IValue If(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            var condition = children[0];
            var result = condition.EvalOnce(scope);

            var listChildren = children.OfType<ListNode>();

            IValue returnValue = Value.Nil;
            if (!ReferenceEquals(result, Value.Nil) && !ReferenceEquals(result, Bool.False))
            {
                var thenBlock =
                    listChildren.FirstOrDefault(l => (l.Children.First() as IdentifierNode)?.Name == "then");
                if (thenBlock != null) returnValue = thenBlock.EvalOnce(scope);
            }
            else
            {
                var elseBlock =
                    listChildren.FirstOrDefault(l => (l.Children.First() as IdentifierNode)?.Name == "else");
                if (elseBlock != null) returnValue = elseBlock.EvalOnce(scope);
            }

            return returnValue;
        }

        [NetFunction("??")]
        internal static IValue Coalesce(List<IValue> list, Scope scope)
        {
            IValue firstNonNull = list.FirstOrDefault(i => !ReferenceEquals(i, Value.Nil));

            if (firstNonNull != null)
            {
                return firstNonNull;
            }
            else
            {
                return Value.Nil;
            }
        }

        [NetFunction("do")]
        internal static IValue Do(List<IValue> list, Scope scope)
        {
            return list.LastOrDefault() ?? Value.Nil;
        }

        [NetFunction("type")]
        internal static IValue Type(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            if (first.MetaType?.Type?.IsDefined ?? false)
            {
                return first.MetaType.Type.Invoke(list, scope);
            }
            else
            {
                return (IValue) first.Type ?? Value.Nil;
            }
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

        [MagicFunction("time")]
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

        [MagicFunction("global")]
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
                //TODO Should probably be eval once
                return identifier.Eval(rootScope);
            }
            //TODO Should probably be eval once
            IValue value = list.Children[2].Eval(scope);
            rootScope[identifier.Name] = value;
            return value;
        }

        [MagicFunction("assign")]
        internal static IValue AssignUpval(ListNode list, Scope scope)
        {
            IdentifierNode identifier = (IdentifierNode) list.Children[1];
            IValue value = list.Children[2].EvalOnce(scope);

            scope.Assign(identifier.Name, value);

            return value;
        }
    }
}
