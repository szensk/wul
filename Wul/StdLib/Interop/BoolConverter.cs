using System;
using Wul.Interpreter.Types;

namespace Wul.StdLib.Interop
{
    class BoolConverter : IValueConverter<bool>
    {
        public IValue ConvertToIValue(bool original)
        {
            return original ? Bool.True : Bool.False;
        }

        public bool ConvertFromIValue(IValue original)
        {
            if (original == Value.Nil) throw new Exception("Unable to convert null to bool");
            if (original is Bool b)
            {
                return b.Value;
            }
            throw new Exception($"Unable to convert {original.Type.Name} to bool");
        }
    }
}