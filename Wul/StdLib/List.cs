using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;

namespace Wul.StdLib
{
    internal class List
    {
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
