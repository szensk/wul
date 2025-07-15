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
            if (first.Value.Length == 0) return Value.Nil;
            if (first.Value.Length == 1) return WulString.EmptyString;
            return new WulString(first.Value.Substring(1));
        }

        private IValue CharacterAtIndex(List<IValue> arguments, Scope s)
        {
            WulString str = (WulString) arguments[0];
            IValue firstArg = arguments[1];
            if (firstArg is Number index)
            {
                if (index >= str.Value.Length) return Value.Nil;
                return new WulString(str.Value[index].ToString());
            }
            else if (firstArg is WulString pattern)
            {
                //TODO should it find all matches?
                var start = str.Value.IndexOf(pattern.Value);
                if (start >= 0)
                {
                    return new Range(start, start + pattern.Value.Length - 1, 1);
                }
                return Value.Nil;
            }
            else if (firstArg is Range rng)
            {
                List<string> subsections = [];
                var substrings = StdLib.Helpers.IterateOverEnumerable<string>(rng, (val) => str.Value.Substring((int)(Number)val, 1), s);
                return (WulString) string.Join(string.Empty, substrings);
            }
            else
            {
                throw new System.ArgumentException("Index of a string must be one of: Number, String, Range");
            }
        }

        private IValue Length(List<IValue> arguments, Scope s)
        {
            WulString first = (WulString)arguments[0];

            return (Number) first.Value.Length;
        }
    }
}