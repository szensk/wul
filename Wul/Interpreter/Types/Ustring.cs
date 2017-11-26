using Wul.Interpreter.MetaTypes;

namespace Wul.Interpreter.Types
{
    public class StringType : WulType
    {
        public StringType() : base("String", typeof(UString))
        {
        }

        public static readonly StringType Instance = new StringType();
    }

    public class UString : IValue
    {
        private static readonly StringMetaType metaType = new StringMetaType();
        public MetaType MetaType { get; set; }

        public UString(string value)
        {
            Value = value;
            MetaType = metaType;
        }

        public string Value { get; }

        public WulType Type => StringType.Instance;

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