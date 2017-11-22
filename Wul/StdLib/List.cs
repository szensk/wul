using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;

namespace Wul.StdLib
{
    internal class List
    {
        internal static IFunction Concat = new NetFunction((list, scope) =>
        {
            var lists = list.OfType<ListTable>();
            if (!lists.Any())
            {
                return Value.Nil;
            }

            List<IValue> results = new List<IValue>();
            foreach (var l in lists)
            {
                results.AddRange(l.AsList());
            }
            return new ListTable(results.ToArray());
        }, "concat");

        internal static IFunction First = new NetFunction((list, scope) =>
        {
            var firstList = list.FirstOrDefault() as ListTable;
            if (firstList == null)
            {
                return list.FirstOrDefault() ?? Value.Nil;
            }
            if (firstList.Count == 0)
            {
                return Value.Nil;
            }

            return firstList.AsList()[0];
        }, "first");

        internal static IFunction Remainder = new NetFunction((list, scope) =>
        {
            var firstList = list[0] as ListTable;
            if (firstList == null || firstList.Count <= 1)
            {
                return Value.Nil;
            }

            return new ListTable(firstList.AsList().Skip(1).ToArray());
        }, "rem");

        internal static IFunction Empty = new NetFunction((list, scope) =>
        {
            ListTable firstList = list.FirstOrDefault() as ListTable;
            if (firstList == null)
            {
                return Value.Nil;
            }

            return firstList.Count == 0 ? Bool.True : Bool.False;
        }, "empty?");

        internal static IFunction Length = new NetFunction((list, scope) =>
        {
            ListTable firstList = list.FirstOrDefault() as ListTable;
            if (firstList == null)
            {
                return Value.Nil;
            }

            return firstList.Count;
        }, "length");
    }
}
