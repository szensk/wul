using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class RangeMetaType : MetaType
    {
        public static readonly RangeMetaType Instance = new RangeMetaType();

        private RangeMetaType()
        {
            //Arithmetic
            Multiply.Method = new NetFunction(MultiplyRange, Multiply.Name);

            //Equality
            Equal.Method = new NetFunction(AreEqual, Equal.Name);

            //List
            At.Method = new NetFunction(AtIndex, At.Name);
            Remainder.Method = new NetFunction(Remaining, Remainder.Name);
            Count.Method = new NetFunction(Length, Count.Name);
            Contains.Method = new NetFunction(RangeContains, Contains.Name);

            Invoke.Method = new NetFunction(RangeIndex, Invoke.Name);

            //Other
            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(IdentityType, Type.Name);
        }

        private IValue MultiplyRange(List<IValue> arguments, Scope s)
        {
            Range r = (Range) arguments[0];

            double result = 1;
            foreach (var number in r.AsList().AsList().Cast<Number>())
            {
                result *= number;
            }
            return (Number) result;
        }

        private IValue AreEqual(List<IValue> arguments, Scope s)
        {
            Range left = arguments[0] as Range;
            Range right = arguments[1] as Range;

            if (left == null || right == null) return Bool.False;

            return Equals(left, right) ? Bool.True : Bool.False;
        }

        private IValue RangeIndex(List<IValue> arguments, Scope s)
        {
            Range range = arguments[0] as Range;
            IValue target = arguments[1];

            if (range == null || target == null || !(target.MetaType?.At.IsDefined ?? false)) return Value.Nil;

            //TODO lazily transform range to list of indices
            var indexes = range.AsList().AsList();
            if (indexes.Count == 1)
            {
                return target.MetaType.At.Invoke(new List<IValue>{target, indexes[0]}, s).First();
            }

            List<IValue> values = new List<IValue>(indexes.Count);
            List<IValue> atArguments = new List<IValue>(2) { target, null};
            foreach (var index in indexes)
            {
                atArguments[1] = index;
                values.Add(target.MetaType.At.Invoke(atArguments, s).First());
            }
            return new ListTable(values);
        }

        private IValue AtIndex(List<IValue> arguments, Scope s)
        {
            Range range = (Range) arguments[0];
            Number index = (Number) arguments[1];

            if ((int) index.Value != 0)
            {
                throw new IndexOutOfRangeException("ranges can only be indexed by 0");
            }

            return range.First;
        }

        private IValue Remaining(List<IValue> arguments, Scope s)
        {
            Range range = (Range) arguments[0];

            Range remainder = range.Remainder;
            if (remainder == null)
            {
                return Value.Nil;
            }
            return remainder;
        }

        private IValue Length(List<IValue> arguments, Scope s)
        {
            Range range = (Range) arguments[0];

            return range.Count;
        }

        private IValue RangeContains(List<IValue> arguments, Scope s)
        {
            Range left = (Range) arguments[0];
            Number right = (Number) arguments[1];

            return left.Contains(right) ? Bool.True : Bool.False;
        }
    }
}
