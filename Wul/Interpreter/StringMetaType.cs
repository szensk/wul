using System.Collections.Generic;
using System.Linq;

namespace Wul.Interpreter
{
    public class StringMetaType : MetaType
    {
        public StringMetaType()
        {
            // Comparison
            Equal.Method = new NetFunction(AreEqual, Equal.Name);
            Compare.Method = new NetFunction(Comparison, Compare.Name);

            // Concat
            Concat.Method = new NetFunction(JoinStrings, Concat.Name);

            // Count
            Count.Method = new NetFunction(Length, Count.Name);
        }

        public IValue AreEqual(List<IValue> arguments, Scope s)
        {
            var strings = arguments.Select(a => a as UString).ToArray();
            UString first = strings[0];
            UString second = strings[1];
            return first.Value.Equals(second.Value) ? Bool.True : Bool.False;
        }

        public IValue Comparison(List<IValue> arguments, Scope s)
        {
            var strings = arguments.Select(a => a as UString).ToArray();
            UString first = strings[0];
            UString second = strings[1];
            return (Number) string.CompareOrdinal(first.Value, second.Value);
        }

        public IValue JoinStrings(List<IValue> argumetns, Scope s)
        {
            var strings = argumetns.OfType<UString>().Select(x => x.Value).ToList();
            if (!strings.Any())
            {
                return Value.Nil;
            }
            return new UString(string.Join("", strings));
        }

        public IValue Length(List<IValue> arguments, Scope s)
        {
            UString first = (UString)arguments[0];

            return (Number) first.Value.Length;
        }
    }
}