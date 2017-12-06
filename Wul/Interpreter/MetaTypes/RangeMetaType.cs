using System;
using System.Collections.Generic;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class RangeMetaType : MetaType
    {
        public static readonly RangeMetaType Instance = new RangeMetaType();

        public RangeMetaType() : base(null)
        {
            //Equality
            Equal = new NetFunction(AreEqual, Equal.Name);

            //List
            At = new NetFunction(AtIndex, At.Name);
            Remainder = new NetFunction(Remaining, Remainder.Name);
            Count = new NetFunction(Length, Count.Name);

            Invoke = new NetFunction(RangeIndex, Invoke.Name);

            //Other
            AsString = new NetFunction(IdentityString, AsString.Name);
            Type = new NetFunction(IdentityType, Type.Name);

            InitializeDictionary();
        }

        public IValue AreEqual(List<IValue> arguments, Scope s)
        {
            Range left = arguments[0] as Range;
            Range right = arguments[1] as Range;

            if (left == null || right == null) return Bool.False;

            return left == right ? Bool.True : Bool.False;
        }

        public IValue RangeIndex(List<IValue> arguments, Scope s)
        {
            Range range = arguments[0] as Range;
            IValue target = arguments[1];

            if (range == null || target == null || !(target.Metatype?.At.IsDefined ?? false)) return Value.Nil;

            var indexes = range.AsList().AsList();
            if (indexes.Count == 1)
            {
                return target.Metatype.At.Invoke(new List<IValue>{target, indexes[0]}, s);
            }

            List<IValue> values = new List<IValue>(indexes.Count);
            List<IValue> atArguments = new List<IValue>(2) { target, null};
            foreach (var index in indexes)
            {
                atArguments[1] = index;
                values.Add(target.Metatype.At.Invoke(atArguments, s));
            }
            return new ListTable(values);
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
