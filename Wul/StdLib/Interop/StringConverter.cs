using Wul.Interpreter.Types;

namespace Wul.StdLib.Interop
{
    class StringConverter : IValueConverter<string>
    {
        public IValue ConvertToIValue(string original)
        {
            if (original == null) return Value.Nil;
            return new WulString(original);
        }

        public string ConvertFromIValue(IValue original)
        {
            if (original == Value.Nil) return null;
            return original.AsString();
        }
    }
}