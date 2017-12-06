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

            return first.Metatype.At.Invoke(new List<IValue>{ first, (Number) 0}, scope);
        }

        [NetFunction("last")]
        internal static IValue Last(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            Number count = (Number) first.Metatype.Count.Invoke(list, scope);

            return first.Metatype.At.Invoke(new List<IValue> {first, (Number) (count.Value - 1)}, scope);
        }

        [NetFunction("rem")]
        internal static IValue Remainder(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.Metatype.Remainder.Invoke(list, scope);
        }

        [MagicFunction("at")]
        internal static IValue AtIndex(ListNode list, Scope scope)
        {
            IValue first = list.Children[1].Eval(scope);

            //TODO UnpackList
            if (first.Type == MapType.Instance || first is NetObject)
            {
                return first.Metatype.At.Invoke(new List<IValue> {list}, scope);
            }
            else
            {
                var evaluatedArguments = list.Children.Skip(1).Select(c => c.EvalOnce(scope)).ToList();
                return first.Metatype.At.Invoke(evaluatedArguments, scope);
            }
        }

        [MagicFunction("set")]
        internal static IValue SetIndex(ListNode list, Scope scope)
        {
            IValue first = list.Children[1].Eval(scope);

            if (first.Type == MapType.Instance)
            {
                return first.Metatype.Set.Invoke(new List<IValue> { list }, scope);
            }
            else
            {
                var evaluatedArguments = list.Children.Skip(1).Select(c => c.EvalOnce(scope)).ToList();
                return first.Metatype.Set.Invoke(evaluatedArguments, scope);
            }
        }

        [NetFunction("empty?")]
        internal static IValue Empty(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            Number count = (Number) first.Metatype.Count.Invoke(list, scope);

            return count == 0 ? Bool.True : Bool.False;
        }

        [NetFunction("len")]
        [NetFunction("#")]
        internal static IValue Length(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.Metatype.Count.Invoke(list, scope);
        }

        [NetFunction("push")]
        internal static IValue PushEnd(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.Metatype.Push.Invoke(list, scope);
        }

        [NetFunction("pop")]
        internal static IValue PopEnd(List<IValue> list, Scope scope)
        {
            IValue first = list.First();

            return first.Metatype.Pop.Invoke(list, scope);
        }
    }
}
