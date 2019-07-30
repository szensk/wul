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
            Equal.Method = new NetFunction(AreEqual, Equal.Name);
            Compare.Method = new NetFunction(Comparison, Compare.Name);

            At.Method = new NetFunction(CharacterAtIndex, At.Name);
            Concat.Method = new NetFunction(JoinStrings, Concat.Name);
            Remainder.Method = new NetFunction(Remaining, Remainder.Name);

            // Count
            Count.Method = new NetFunction(Length, Count.Name);

            AsString.Method = new NetFunction(IdentityString, AsString.Name);
            Type.Method = new NetFunction(IdentityType, Type.Name);
        }

        private IValue AreEqual(List<IValue> arguments, Scope s)
        {
            var strings = arguments.Select(a => a as WulString).ToArray();
            WulString first = strings[0];
            WulString second = strings[1];
            return first.Value.Equals(second.Value) ? Bool.True : Bool.False;
        }

        private IValue Comparison(List<IValue> arguments, Scope s)
        {
            var strings = arguments.Select(a => a as WulString).ToArray();
            WulString first = strings[0];
            WulString second = strings[1];
            return (Number) string.CompareOrdinal(first.Value, second.Value);
        }

        private IValue JoinStrings(List<IValue> arguments, Scope s)
        {
            var strings = arguments.Cast<WulString>().Select(x => x.Value).ToList();
            if (!strings.Any())
            {
                return Value.Nil;
            }
            return new WulString(string.Join("", strings));
        }

        private IValue Remaining(List<IValue> arguments, Scope s)
        {
            var first = (WulString)arguments[0];
            if (first.Value.Length == 0)
            {
                return Value.Nil;
            }

            return new WulString(first.Value.Substring(1));
        }

        private IValue CharacterAtIndex(List<IValue> arguments, Scope s)
        {
            WulString str = (WulString)arguments.First();
            Number index = (Number)arguments.Skip(1).First();

            return new WulString(str.Value[index].ToString());
        }

        private IValue Length(List<IValue> arguments, Scope s)
        {
            WulString first = (WulString)arguments[0];

            return (Number) first.Value.Length;
        }
    }
}