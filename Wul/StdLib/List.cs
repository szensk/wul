using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;

namespace Wul.StdLib
{
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

        [NetFunction("map")]
        internal static IValue Map(List<IValue> list, Scope scope)
        {
            var listToMap = list[0] as ListTable;
            if (listToMap == null && list[0] is Interpreter.Types.Range r)
            {
                listToMap = r.AsList();
            }
            var callback = list[1];
            var func = callback.MetaType.Invoke;

            if (!func.IsDefined)
            {
                throw new Exception("Callback is not a function or invokeable");
            }

            var result = listToMap.AsList()
                .Select(item => func.Invoke(Value.ListWith(callback, item), scope)
                .First());

            return new ListTable(result);
        }
    }
}
