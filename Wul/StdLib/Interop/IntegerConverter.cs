using System;
using Wul.Interpreter.Types;

namespace Wul.StdLib.Interop
{
    class IntegerConverter : ValueConverter<long>
    {
        public override IValue ConvertToIValue(long original)
        {
            return (Number) original;
        }

        public override long ConvertFromIValue(IValue original)
        {
            if (original == Value.Nil) throw new Exception("Unable to convert null to int");
            if (original is Number n && n.IsInteger)
            {
                return (int) n.Value;
            }
            throw new Exception($"Unable to convert {original.Type.Name} to int");
        }
    }
}