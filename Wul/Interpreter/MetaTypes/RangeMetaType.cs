using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;
using Wul.StdLib;
using Range = Wul.Interpreter.Types.Range;

namespace Wul.Interpreter.MetaTypes
{
    public class RangeMetaType : MetaType
    {
        public static readonly RangeMetaType Instance = new RangeMetaType();

        private RangeMetaType()
        {
            //Arithmetic
            Multiply.Method = new NetFunction(MultiplyRange, Multiply.Name);
            Add.Method = new NetFunction(AddRange, Add.Name);

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

        private IValue AddRange(List<IValue> arguments, Scope s)
        {
            Range r = (Range)arguments[0];

            double result = 0;
            foreach (var number in r.AsList().AsList().Cast<Number>())
            {
                result += number;
            }
            return (Number)result;
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
            IValue target = arguments[1];
            if (arguments[0] is not Range range || target == null || !(target.MetaType?.At.IsDefined ?? false)) return Value.Nil;

            var values = Helpers.IterateOverEnumerable(range, (index) => { return target.MetaType.At.Invoke([target, index], s).First(); }, s);

            if (target is WulString)
                return new WulString(string.Join(string.Empty, values.Select(s => ((WulString)s).Value)));
            else
                return new ListTable(values);
        }

        private IValue AtIndex(List<IValue> arguments, Scope s)
        {
            Range range = (Range) arguments[0];
            Number nindex = (Number) arguments[1];
            int index = (int) nindex;

            if (index == 0)
            {
                return range.First;
            }

            if (range.Contains(index))
            {
                return range.NthElement(index);
            }

            throw new ArgumentException($"index out of range {range.First}");
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
