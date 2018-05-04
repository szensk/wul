using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class ListMetaType : MetaType
    {
        public static readonly ListMetaType Instance = new ListMetaType();

        private ListMetaType()
        {
            //Equality
            Equal.Method = NetFunction.FromSingle(AreEqual, Equal.Name);

            //List
            At.Method = NetFunction.FromSingle(AtIndex, At.Name);
            Set.Method = NetFunction.FromSingle(SetIndex, Set.Name);
            Remainder.Method = NetFunction.FromSingle(Remaining, Remainder.Name);
            Count.Method = NetFunction.FromSingle(Length, Count.Name);
            Concat.Method = NetFunction.FromSingle(JoinLists, Concat.Name);

            Push.Method = NetFunction.FromSingle(PushEnd, Push.Name);
            Pop.Method = NetFunction.FromSingle(PopEnd, Pop.Name);
            Contains.Method = NetFunction.FromSingle(ListContains, Contains.Name);
            
            //Other
            AsString.Method = NetFunction.FromSingle(IdentityString, AsString.Name);
            Type.Method = NetFunction.FromSingle(IdentityType, Type.Name);
        }

        private IValue Length(List<IValue> arguments, Scope s)
        {
            ListTable list = (ListTable) arguments.First();

            return list.Count;
        }

        private IValue Remaining(List<IValue> arguments, Scope s)
        {
            var firstList = (ListTable)arguments[0];
            if (firstList == null || firstList.Count == 0)
            {
                return Value.Nil;
            }

            var values = firstList.AsList().Skip(1).ToArray();
            return new ListTable(values);
        }

        private IValue AtIndex(List<IValue> arguments, Scope s)
        {
            ListTable list = (ListTable) arguments.First();
            Number index = (Number) arguments.Skip(1).First();
            if (index < 0) index = list.Count + index;

            return list[index];
        }

        private IValue SetIndex(List<IValue> arguments, Scope s)
        {
            ListTable list = (ListTable) arguments[0];
            Number index = (Number)arguments[1];
            IValue value = arguments[2];

            list.Assign(index, value);

            return list;
        }

        private IValue JoinLists(List<IValue> argument, Scope s)
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

        private IValue AreEqual(List<IValue> arguments, Scope s)
        {
            ListTable left = (ListTable) arguments[0];
            ListTable right = (ListTable) arguments[1];

            return left.AsList().SequenceEqual(right.AsList()) ? Bool.True : Bool.False;
        }

        private IValue PushEnd(List<IValue> arguments, Scope s)
        {
            ListTable left = (ListTable) arguments[0];

            foreach (IValue val in arguments.Skip(1))
            {
                left.Add(val);
            }

            return left;
        }

        private IValue PopEnd(List<IValue> arguments, Scope s)
        {
            ListTable left = (ListTable) arguments[0];
            if (left.Count < 1) return Value.Nil;

            var list = left.AsList();
            IValue first = list.First();
            list.RemoveAt(0);
            return first;
        }

        private IValue ListContains(List<IValue> arguments, Scope s)
        {
            ListTable left = (ListTable)arguments[0];
            IValue right = arguments[1];

            var list = left.AsList();
            return list.Contains(right) ? Bool.True : Bool.False;
        }
    }
}