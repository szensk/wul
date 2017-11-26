using Wul.Interpreter.MetaTypes;

namespace Wul.Interpreter.Types
{
    public class UString : IValue
    {
        private static readonly StringMetaType metaType = new StringMetaType();
        public MetaType ValueMetaType { get; set; }
        public MetaType MetaType => ValueMetaType;

        public UString(string value)
        {
            Value = value;
            ValueMetaType = metaType;
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