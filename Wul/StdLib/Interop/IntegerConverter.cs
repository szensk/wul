using System;
using Wul.Interpreter.Types;

namespace Wul.StdLib.Interop
{
    class IntegerConverter : IValueConverter<int>
    {
        public IValue ConvertToIValue(int original)
        {
            return (Number) original;
        }

        public int ConvertFromIValue(IValue original)
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