using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter.Types;

namespace Wul.Interpreter.MetaTypes
{
    public class StringMetaType : MetaType
    {
        public static readonly StringMetaType Instance = new StringMetaType();

        private StringMetaType()
        {
            // Comparison
            Equal.Method = NetFunction.FromSingle(AreEqual, Equal.Name);
            Compare.Method = NetFunction.FromSingle(Comparison, Compare.Name);

            At.Method = NetFunction.FromSingle(CharacterAtIndex, At.Name);
            Concat.Method = NetFunction.FromSingle(JoinStrings, Concat.Name);
            Remainder.Method = NetFunction.FromSingle(Remaining, Remainder.Name);

            // Count
            Count.Method = NetFunction.FromSingle(Length, Count.Name);

            AsString.Method = NetFunction.FromSingle(IdentityString, AsString.Name);
            Type.Method = NetFunction.FromSingle(IdentityType, Type.Name);
        }

        private IValue AreEqual(List<IValue> arguments, Scope s)
        {
            var strings = arguments.Select(a => a as UString).ToArray();
            UString first = strings[0];
            UString second = strings[1];
            return first.Value.Equals(second.Value) ? Bool.True : Bool.False;
        }

        private IValue Comparison(List<IValue> arguments, Scope s)
        {
            var strings = arguments.Select(a => a as UString).ToArray();
            UString first = strings[0];
            UString second = strings[1];
            return (Number) string.CompareOrdinal(first.Value, second.Value);
        }

        private IValue JoinStrings(List<IValue> argumetns, Scope s)
        {
            var strings = argumetns.OfType<UString>().Select(x => x.Value).ToList();
            if (!strings.Any())
            {
                return Value.Nil;
            }
            return new UString(string.Join("", strings));
        }

        private IValue Remaining(List<IValue> arguments, Scope s)
        {
            var first = (UString)arguments[0];
            if (first.Value.Length == 0)
            {
                return Value.Nil;
            }

            return new UString(first.Value.Substring(1));
        }

        private IValue CharacterAtIndex(List<IValue> arguments, Scope s)
        {
            UString str = (UString)arguments.First();
            Number index = (Number)arguments.Skip(1).First();

            return new UString(str.Value[index].ToString());
        }

        private IValue Length(List<IValue> arguments, Scope s)
        {
            UString first = (UString)arguments[0];

            return (Number) first.Value.Length;
        }
    }
}