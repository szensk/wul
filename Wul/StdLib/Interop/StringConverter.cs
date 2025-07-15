using Wul.Interpreter.Types;

namespace Wul.StdLib.Interop
{
    class StringConverter : ValueConverter<string>
    {
        public override IValue ConvertToIValue(string original)
        {
            if (original == null) return Value.Nil;
            return new WulString(original);
        }

        public override string ConvertFromIValue(IValue original)
        {
            if (original == Value.Nil) return null;
            return original.AsString();
        }

        public override int Priority => 10;
    }
}