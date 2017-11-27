﻿using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.StdLib
{
    internal class List
    {
        internal static IFunction Listify = new NetFunction((list, Scope) =>
        {
            if (list.Count == 1 && list[0].Type == RangeType.Instance)
            {
                return ((Interpreter.Types.Range) list[0]).AsList();
            }
            else
            {
                return new ListTable(list);
            }
        }, "list");

        internal static IFunction First = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.At.Invoke(new List<IValue>{ first, (Number) 0}, scope);
        }, "first");

        internal static IFunction Last = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            Number count = (Number) first.MetaType.Count.Invoke(list, scope);

            return first.MetaType.At.Invoke(new List<IValue> {first, (Number) (count.Value - 1)}, scope);
        }, "last");

        internal static IFunction Remainder = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Remainder.Invoke(list, scope);
        }, "rem");

        internal static IFunction AtIndex = new MagicNetFunction((list, scope) =>
        {
            IValue first = list.Children[1].Eval(scope);

            if (first.Type == MapType.Instance)
            {
                return first.MetaType.At.Invoke(new List<IValue> {list}, scope);
            }
            else
            {
                var evaluatedArguments = list.Children.Skip(1).Select(c => c.Eval(scope)).ToList();
                return first.MetaType.At.Invoke(evaluatedArguments, scope);
            }
        }, "at");

        internal static IFunction SetIndex = new MagicNetFunction((list, scope) =>
        {
            IValue first = list.Children[1].Eval(scope);

            if (first.Type == MapType.Instance)
            {
                return first.MetaType.Set.Invoke(new List<IValue> { list }, scope);
            }
            else
            {
                var evaluatedArguments = list.Children.Skip(1).Select(c => c.Eval(scope)).ToList();
                return first.MetaType.Set.Invoke(evaluatedArguments, scope);
            }
        }, "set");

        internal static IFunction Empty = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            Number count = (Number) first.MetaType.Count.Invoke(list, scope);

            return count == 0 ? Bool.True : Bool.False;
        }, "empty?");

        internal static IFunction Length = new NetFunction((list, scope) =>
        {
            IValue first = list.First();

            return first.MetaType.Count.Invoke(list, scope);
        }, "length");
    }
}
