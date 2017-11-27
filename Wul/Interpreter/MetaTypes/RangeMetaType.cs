using System;
using System.Collections.Generic;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class RangeMetaType : MetaType
    {
        public static readonly RangeMetaType Instance = new RangeMetaType();

        public RangeMetaType()
        {
            //Equality
            Equal.Method = new NetFunction(AreEqual, Equal.Name);

            //List
            At.Method = new NetFunction(AtIndex, At.Name);
            Remainder.Method = new NetFunction(Remaining, Remainder.Name);
            Count.Method = new NetFunction(Length, Count.Name);

            //Other
            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(IdentityType, Type.Name);
        }

        public IValue AreEqual(List<IValue> arguments, Scope s)
        {
            Range left = arguments[0] as Range;
            Range right = arguments[1] as Range;

            if (left == null || right == null) return Bool.False;

            return left == right ? Bool.True : Bool.False;
        }

        public IValue AtIndex(List<IValue> arguments, Scope s)
        {
            Range range = (Range) arguments[0];
            Number index = (Number) arguments[1];

            if ((int) index.Value != 0)
            {
                throw new IndexOutOfRangeException("ranges can only be indexed by 0");
            }

            return range.First;
        }

        public IValue Remaining(List<IValue> arguments, Scope s)
        {
            Range range = (Range) arguments[0];

            Range remainder = range.Remainder;
            if (remainder == null)
            {
                return Value.Nil;
            }
            return remainder;
        }

        public IValue Length(List<IValue> arguments, Scope s)
        {
            Range range = (Range) arguments[0];

            return range.Count;
        }
    }
}
