using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class ListMetaType : MetaType
    {
        public ListMetaType()
        {
            //Equality
            Equal.Method = new NetFunction(AreEqual, Equal.Name);

            //List
            At.Method = new NetFunction(AtIndex, At.Name);
            Remainder.Method = new NetFunction(Remaining, Remainder.Name);
            Count.Method = new NetFunction(Length, Count.Name);
            Concat.Method = new NetFunction(JoinLists, Concat.Name);

            //Other
            AsString.Method = new NetFunction(IdentityString, AsString.Name);
        }

        public IValue Length(List<IValue> arguments, Scope s)
        {
            ListTable list = (ListTable) arguments.First();

            return list.Count;
        }

        public IValue Remaining(List<IValue> arguments, Scope s)
        {
            var firstList = (ListTable)arguments[0];
            if (firstList == null || firstList.Count == 0)
            {
                return Value.Nil;
            }

            var values = firstList.AsList().Skip(1).ToArray();
            return new ListTable(values);
        }

        public IValue AtIndex(List<IValue> arguments, Scope s)
        {
            ListTable list = (ListTable) arguments.First();
            Number index = (Number) arguments.Skip(1).First();

            return list[index];
        }

        public IValue JoinLists(List<IValue> argument, Scope s)
        {
            var lists = argument.Select(a => a as ListTable).ToList();
            if (!lists.Any())
            {
                return Value.Nil;
            }
            if (lists.Any(l => l == null))
            {
                throw new InvalidOperationException("All elements must be lists");
            }

            List<IValue> results = new List<IValue>();
            foreach (var l in lists)
            {
                results.AddRange(l.AsList());
            }
            return new ListTable(results.ToArray());
        }

        public IValue AreEqual(List<IValue> arguments, Scope s)
        {
            ListTable left = (ListTable) arguments[0];
            ListTable right = (ListTable) arguments[1];

            return left.AsList().SequenceEqual(right.AsList()) ? Bool.True : Bool.False;
        }
    }
}