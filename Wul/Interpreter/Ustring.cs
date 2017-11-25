namespace Wul.Interpreter
{
    public class UString : IValue
    {
        private static readonly StringMetaType metaType = new StringMetaType();
        public MetaType MetaType => metaType;

        public UString(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public string AsString()
        {
            return Value;
        }

        public object ToObject()
        {
            return Value;
        }
    }
}