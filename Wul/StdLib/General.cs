using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;
using Wul.StdLib.Attribute;

namespace Wul.StdLib
{
    [StdLib]
    class General
    {
        [NetFunction("identity")]
        internal static IValue Identity(IValue first, Scope scope) => first;

        [MagicFunction("def")]
        internal static IValue Define(ListNode list, Scope scope)
        {
            return RegisterNameValues(list, scope);
        }

        [MagicFunction("let")]
        internal static IValue Let(ListNode list, Scope scope)
        {
            Scope currentScope = scope.EmptyChildScope();
            return RegisterNameValues(list, currentScope);
        }

        private static void RegisterNameInScope(Scope inScope, IdentifierNode nameName, IValue value)
        {
            string name = nameName.Name;
            if (ReferenceEquals(value, Value.Nil))
            {
                inScope.Remove(name);
            }
            else
            {
                inScope[name] = value;
            }
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

        private static IValue RegisterNameValues(ListNode list, Scope scope)
        {
            var name = ((IdentifierNode) list.Children.First()).Name;
            var children = list.Children.Skip(1).ToArray();
            if (children[0] is IdentifierNode nameIdentifier)
            {
                var value = children[1].EvalOnce(scope);
                RegisterNameInScope(scope, nameIdentifier, value);
                return EvalRemainder(scope, children.Skip(2));
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
                    RegisterNameInScope(scope, nameNode, value);
                }

                return EvalRemainder(scope, children.Skip(2));
            }
            throw new Exception($"unknown form of {name}, identifier or list expected");
        }
        
        //TODO allow other multiple statements as the body (merge them into a do or something idk)
        [MagicFunction("defn")]
        internal static IValue DefineFunction(ListNode list, Scope scope)
        {
            var children = list.Children.Skip(1).ToArray();

            var nameIdentifier = (IdentifierNode) children[0];
            string name = nameIdentifier.Name;

            int bodyIndex = 2;
            List<string> argNames;
            if (children.Length == 3)
            {
                var arguments = (ListNode) children[1];
                argNames = arguments.Children
                    .OfType<IdentifierNode>()
                    .Select(a => a.Name)
                    .ToList();
            }
            else
            {
                argNames = new List<string>();
                bodyIndex = 1;
            }

            var body = (ListNode) children[bodyIndex];
            scope[name] = Value.Nil;
            var function = new Function(body, name, argNames, scope);
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
        internal static IValue Type(IValue first, Scope scope)
        {
            if (first.MetaType?.Type?.IsDefined == true)
            {
                return first.MetaType.Type.Invoke(Value.ListWith(first), scope).First();
            }
            else
            {
                return (IValue) first.Type ?? Value.Nil;
            }
        }

        [NetFunction("exit")]
        internal static IValue Exit(IValue code, Scope scope) => Exit(code as Number);

        //TODO ultimately I want to be able to annotate these cleaner functions directly
        internal static Number Exit(Number code)
        {
            double exitCode = code?.Value ?? 0;
            Environment.Exit((int) exitCode);
            return exitCode;
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

        [MultiMagicFunction("unpack")]
        private static List<IValue> Unpack(ListNode list, Scope scope)
        {
            var first = list.Children[1];
            //this is dumb, should be a separate function "unpack-recursive"
            bool recursive = first is IdentifierNode i && i.Name == "recursive";
            var evaluatedArguments = list.Children.Skip(recursive ? 2 : 1).Select(v => v.Eval(scope)).ToList();
            return Unpack(evaluatedArguments, scope, recursive);
        }

        private static List<IValue> Unpack(List<IValue> list, Scope scope, bool recursive)
        {
            var result = new List<IValue>();
            foreach (var item in list)
            {
                if (item is ListTable lt)
                {
                    if (recursive)
                    {
                        result.AddRange(Unpack(lt.AsList(), scope, recursive));
                    }
                    else
                    {
                        result.AddRange(lt.AsList());
                    }
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

        //this might be useless, creates a named weak reference to a value
        [MagicFunction("weakref")]
        private static IValue WeakReference(ListNode list, Scope scope)
        {
            var name = ((IdentifierNode)list.Children[1]).Name;
            var children = list.Children.Skip(2).ToArray();
            var value = children[0].EvalOnce(scope);
            if (ReferenceEquals(value, Value.Nil))
            {
                scope.Remove(name);
            }
            else
            {
                scope.SetWeak(name, value);
            }
            return value;
        }
    }
}
