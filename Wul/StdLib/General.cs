using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;

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

        private static void RegisterNameInScope(Scope parentScope, Scope inScope, IdentifierNode nameName, IValue value)
        {
            string name = nameName.Name;
            inScope[name] = value;
        }

        private static IValue EvalRemainder(Scope scope, IEnumerable<SyntaxNode> childrenToEval)
        {
            IValue result = Value.Nil;
            foreach (var child in childrenToEval)
            {
                result = child.Eval(scope);
            }
            return result;
        }

        [MagicFunction("let")]
        internal static IValue Let(ListNode list, Scope scope)
        {
            Scope currentScope = scope.EmptyChildScope();

            var children = list.Children.Skip(1).ToArray();
            if (children[0] is IdentifierNode nameIdentifier)
            {
                RegisterNameInScope(scope, currentScope, nameIdentifier, children[1].EvalOnce(scope));
                return EvalRemainder(currentScope, children.Skip(2));
            }
            else if (children[0] is ListNode namesNode)
            {
                //Destructuring form e.g.
                //(let (x y) (5 6) (print x y) (print y x))
                var values = children[1].EvalMany(scope);
                if (values.Count == 1 && values[0] is ListTable lt)
                {
                    values = lt.AsList();
                }
                for (int i = 0; i < namesNode.Children.Count; i++)
                {
                    var nameNode = (IdentifierNode) namesNode.Children[i];
                    var value = i < values.Count ? values[i] : Value.Nil;
                    RegisterNameInScope(scope, currentScope, nameNode, value);
                }

                return EvalRemainder(currentScope, children.Skip(2));
            }
            throw new Exception("unknown form of let, identifier or list expected");
        }

        //TODO allow other multiple statements as the body (merge them into a do or something idk)
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
        [MagicFunction("->")] //TODO sugar -> (+ $1 $2)
        internal static IValue Lambda(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            List<string> argNames;
            if (children.Length == 2)
            {
                var arguments = (ListNode) children[0];
                argNames = arguments.Children
                    .OfType<IdentifierNode>()
                    .Select(a => a.Name)
                    .ToList();
            }
            else
            {
                argNames = new List<string>();
            }

            var body = children[children.Length == 2 ? 1 : 0];
            ListNode listbody = body as ListNode;
            if (listbody == null)
            {
                listbody = new ListNode(list, new List<SyntaxNode>(), list.Line);
                listbody.Children.Add(new IdentifierNode(listbody, "return"));               
                listbody.Children.Add(body.ToSyntaxNode(listbody));
                System.Diagnostics.Debug.WriteLine("shorthand -> expands to " + listbody);
            }

            return new Function(listbody, "unnamed function", argNames, scope);
        }

        //[NetFunction("then")]
        //[NetFunction("else")]
        //internal static IValue Then(List<IValue> list, Scope scope)
        //{
        //    if (list.Count == 1)
        //    {
        //        return list.First();
        //    }
        //    else
        //    {
        //        return new ListTable(list.ToArray());
        //    }
        //}

        [MultiMagicFunction("if")]
        internal static List<IValue> If(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            var condition = children[0];
            var result = condition.EvalOnce(scope);

            var listChildren = children.OfType<ListNode>();

            List<IValue> returnValue = Value.ListWith(Value.Nil);
            if (!ReferenceEquals(result, Value.Nil) && !ReferenceEquals(result, Bool.False))
            {
                var thenBlock =
                    listChildren.FirstOrDefault(l => (l.Children.First() as IdentifierNode)?.Name == "then");
                if (thenBlock != null) returnValue = thenBlock.EvalManyOnce(scope);
            }
            else
            {
                var elseBlock =
                    listChildren.FirstOrDefault(l => (l.Children.First() as IdentifierNode)?.Name == "else");
                if (elseBlock != null) returnValue = elseBlock.EvalManyOnce(scope);
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
                return first.MetaType.Type.Invoke(list, scope).First();
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

        [MultiNetFunction("unpack")]
        private static List<IValue> Unpack(List<IValue> list, Scope scope)
        {
            var result = new List<IValue>();
            foreach (var item in list)
            {
                if (item is ListTable lt)
                {
                    result.AddRange(lt.AsList());
                }
                else
                {
                    result.Add(item);
                }

            }
            return result;
        }

        [MultiNetFunction("then")]
        [MultiNetFunction("else")]
        [MultiNetFunction("return")]
        private static List<IValue> Return(List<IValue> list, Scope scope)
        {
            switch (list.Count)
            {
                case 0:
                    return Value.ListWith(Value.Nil);
                case 1:
                    return Value.ListWith(list.First());
                default:
                    return list;
            }
        }

        //TODO Scope should be able to hold WeakReference<IValue> and Lazy<IValue>
        //[NetFunction("weakref")]
    }
}
