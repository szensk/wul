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
            IValue result = firstList.AsList().FirstOrDefault() ?? Value.Nil;
            return result;
        }, "first");

        internal static IFunction Last = new NetFunction((list, scope) =>
        {
            var firstList = list.FirstOrDefault() as ListTable;
            if (firstList == null)
            {
                return list.FirstOrDefault() ?? Value.Nil;
            }
            IValue result = firstList.AsList().LastOrDefault() ?? Value.Nil;
            return result;
        }, "last");

        internal static IFunction Remainder = new NetFunction((list, scope) =>
        {
            var firstList = list[0] as ListTable;
            if (firstList == null || firstList.Count == 0)
            {
                return Value.Nil;
            }

            IValue[] values = firstList.AsList().Skip(1).ToArray();
            return new ListTable(values);
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
