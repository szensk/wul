using System;
using Wul.Interpreter.Types;

namespace Wul.StdLib.Interop
{
    class DoubleConverter : IValueConverter<double>
    {
        public IValue ConvertToIValue(double original)
        {
            return (Number) original;
        }

        public double ConvertFromIValue(IValue original)
        {
            if (original == Value.Nil) throw new Exception("Unable to convert null to double");
            if (original is Number n)
            {
                return n.Value;
            }
            throw new Exception($"Unable to convert {original.Type.Name} to double");
        }
    }
}