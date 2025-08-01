﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;
using Wul.StdLib.Attribute;

namespace Wul.StdLib
{
    [StdLib]
    internal class List
    {
        [NetFunction("list")]
        internal static IValue Listify(List<IValue> list, Scope scope)
        {
            if (list.Count == 1 && list[0].Type == RangeType.Instance)
            {
                return ((Interpreter.Types.Range) list[0]).AsList();
            }
            else
            {
                return new ListTable(list);
            }
        }

        [NetFunction("first")]
        internal static IValue First(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.At.Invoke(new List<IValue>{ first, (Number) 0}, scope).First();
        }

        [NetFunction("last")]
        internal static IValue Last(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            Number count = (Number) first.MetaType.Count.Invoke(list, scope).First();

            return first.MetaType.At.Invoke(new List<IValue> {first, (Number) (count.Value - 1)}, scope).First();
        }

        [NetFunction("rem")]
        internal static IValue Remainder(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Remainder.Invoke(list, scope).First();
        }

        // Reverse remainder 
        [NetFunction("reverse")]
        internal static IValue Reverse(List<IValue> list, Scope scope)
        {
            IValue first = list.First();
            if (first is Interpreter.Types.Range r)
            {
                return r.Reverse;
            }
            else if (first is Interpreter.Types.WulString s)
            {
                char[] chars = s.Value.ToCharArray();
                Array.Reverse(chars);
                return (WulString) new string(chars);
            }
            else
            {
                var len = (Number)Length(list, scope);
                var range = new Interpreter.Types.Range(len - 1, 0, -1);
                var args = new List<IValue> { range, first };
                return range.MetaType.Invoke.Invoke(args, scope).First();
            }
        }

        [MagicFunction("at")]
        internal static IValue AtIndex(ListNode list, Scope scope)
        {
            IValue first = list.Children[1].Eval(scope);

            if (first.Type == MapType.Instance || first is NetObject)
            {
                return first.MetaType.At.Invoke(new List<IValue> {list}, scope).First();
            }
            else
            {
                var evaluatedArguments = list.Children.Skip(1).Select(c => c.EvalOnce(scope)).ToList();
                return first.MetaType.At.Invoke(evaluatedArguments, scope).First();
            }
        }

        [MagicFunction("set")]
        internal static IValue SetIndex(ListNode list, Scope scope)
        {
            IValue first = list.Children[1].Eval(scope);

            if (first.Type == MapType.Instance)
            {
                return first.MetaType.Set.Invoke(new List<IValue> { list }, scope).First();
            }
            else
            {
                var evaluatedArguments = list.Children.Skip(1).Select(c => c.EvalOnce(scope)).ToList();
                return first.MetaType.Set.Invoke(evaluatedArguments, scope).First();
            }
        }

        [NetFunction("empty?")]
        internal static IValue Empty(List<IValue> list, Scope scope)
        {
            IValue first = list.First();
            if (first == Value.Nil) return Bool.True;
            
            Number count = (Number) first.MetaType.Count.Invoke(list, scope).First();
            return count == 0 ? Bool.True : Bool.False;
        }

        [NetFunction("len")]
        [NetFunction("#")]
        internal static IValue Length(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Count.Invoke(list, scope).First();
        }

        [NetFunction("push")]
        internal static IValue PushEnd(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Push.Invoke(list, scope).First();
        }

        [NetFunction("pop")]
        internal static IValue PopEnd(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Pop.Invoke(list, scope).First();
        }

        [NetFunction("contains?")]
        internal static IValue Contains(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Contains.Invoke(list, scope).First();
        }

        private static IValue MapMap(List<IValue> list, Scope scope)
        {
            var mapToMap = (MapTable) list[0];
            var callback = list[1];
            var func = callback.MetaType.Invoke;
            var macro = callback.MetaType.ApplyMacro;

            IEnumerable<IValue> result;
            if (func.IsDefined)
            {
                result = mapToMap.AsDictionary()
                    .Select(item => func.Invoke(Value.ListWith(callback, item.Key, item.Value), scope).First());
            }
            else if (macro.IsDefined)
            {
                result = mapToMap.AsDictionary()
                    .Select(item => ApplyMacro(callback, scope, item.Key, item.Value));
            }
            else
            {
                result = mapToMap.AsDictionary().Values.Select(item => callback);
            }

            return new ListTable(result);
        }

        private static ListTable MapList(List<IValue> list, Scope scope)
        {
            var listToMap = (ListTable)list[0];
            var callback = list[1];
            var func = callback.MetaType.Invoke;
            var macro = callback.MetaType.ApplyMacro;

            IEnumerable<IValue> result;
            if (func.IsDefined)
            {
                result = listToMap.AsList()
                    .Select(item => func.Invoke(Value.ListWith(callback, item), scope).First());
            }
            else if (macro.IsDefined)
            {
                result = listToMap.AsList()
                    .Select(item => ApplyMacro(callback, scope, item));
            }
            else
            {
                result = listToMap.AsList().Select(item => callback);
            }

            return new ListTable(result);
        }

        private static ListTable MapEnumberable(List<IValue> list, Scope scope)
        {
            var enumerable = list[0];
            var callback = list[1];
            var func = callback.MetaType.Invoke;
            var macro = callback.MetaType.ApplyMacro;
            var rem = enumerable.MetaType.Remainder;
            var first = enumerable.MetaType.At;

            if (rem == null || !rem.IsDefined || first == null || !first.IsDefined) throw new Exception("Must be enumerable");

            List<IValue> result = [];

            while (enumerable != null && Empty(Value.ListWith(enumerable), scope) != Bool.True)
            {
                IValue cbresult = callback;
                if (func.IsDefined)
                {
                    var firstElement = first.Invoke(Value.ListWith(enumerable, (Number)0), scope).First();
                    cbresult = func.Invoke(Value.ListWith(callback, firstElement), scope).First();
                } 
                else if (macro.IsDefined)
                {
                    var firstElement = first.Invoke(Value.ListWith(enumerable, (Number)0), scope).First();
                    cbresult = ApplyMacro(callback, scope, firstElement);
                }
                result.Add(cbresult);
                enumerable = rem.Invoke(Value.ListWith(enumerable), scope).First();
            }

            return new ListTable(result);
        }

        private static IValue ApplyMacro(IValue callback, Scope scope, params IValue[] arguments)
        {
            var macro = callback.MetaType.ApplyMacro;

            var listNode = new ListNode(null, new List<SyntaxNode>());
            listNode.Children.Add(new IdentifierNode(listNode, "null"));
            foreach (var item in arguments)
            {
                var argumentNode = item.ToSyntaxNode(listNode);
                listNode.Children.Add(argumentNode);
            }

            var expandedMacro = (ListTable) macro.Invoke(Value.ListWith(callback, listNode), scope).First();
            var macroNode = expandedMacro.ToSyntaxNode(null);
            return macroNode.Eval(scope);
        }

        [NetFunction("map")]
        internal static IValue Map(List<IValue> list, Scope scope)
        {
            if (list[0] is MapTable) return MapMap(list, scope);
            if (list[0] is ListTable) return MapList(list, scope);
            else return MapEnumberable(list, scope);
        }

        private static int DefaultComparator(IValue x, IValue y, Scope scope)
        {
            var cmps = x.MetaType.Compare.Invoke(Value.ListWith(x,y), scope);
            var cmp = cmps[0] as Number;
            return cmp ?? 0;
        }

        private static int CustomComparator(IValue comparer, IValue x, IValue y, Scope scope)
        {
            var cmps = comparer.MetaType.Invoke.Invoke(Value.ListWith(comparer, x, y), scope);
            var cmp = cmps[0] as Number;
            return cmp ?? 0;
        }

        [NetFunction("sort")]
        private static ListTable Sort(List<IValue> list, Scope scope)
        {
            var lt = list[0] as ListTable;
            Comparison<IValue> comparator;
            if (list.Count > 1) 
            {
                var comparer = list[1];
                comparator = (x,y) => CustomComparator(comparer, x, y, scope);
            }
            else 
            {
                comparator = (x,y) => DefaultComparator(x, y, scope);
            }
            var results = lt.AsList();
            results.Sort(comparator);
            return new ListTable(results);
        }
    }
}
