using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser;

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

            return first.MetaType.At.Invoke(new List<IValue>{ first, (Number) 0}, scope);
        }

        [NetFunction("last")]
        internal static IValue Last(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            Number count = (Number) first.MetaType.Count.Invoke(list, scope);

            return first.MetaType.At.Invoke(new List<IValue> {first, (Number) (count.Value - 1)}, scope);
        }

        [NetFunction("rem")]
        internal static IValue Remainder(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Remainder.Invoke(list, scope);
        }

        [MagicNetFunction("at")]
        internal static IValue AtIndex(ListNode list, Scope scope)
        {
            IValue first = list.Children[1].Eval(scope);

            if (first.Type == MapType.Instance || first is NetObject)
            {
                return first.MetaType.At.Invoke(new List<IValue> {list}, scope);
            }
            else
            {
                var evaluatedArguments = list.Children.Skip(1).Select(c => c.Eval(scope)).ToList();
                return first.MetaType.At.Invoke(evaluatedArguments, scope);
            }
        }

        [MagicNetFunction("set")]
        internal static IValue SetIndex(ListNode list, Scope scope)
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
        }

        [NetFunction("empty?")]
        internal static IValue Empty(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            Number count = (Number) first.MetaType.Count.Invoke(list, scope);

            return count == 0 ? Bool.True : Bool.False;
        }

        [NetFunction("len")]
        [NetFunction("#")]
        internal static IValue Length(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.MetaType.Count.Invoke(list, scope);
        }
    }
}
