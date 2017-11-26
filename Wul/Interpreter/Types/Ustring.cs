using Wul.Interpreter.MetaTypes;

namespace Wul.Interpreter.Types
{
    public class StringType : WulType
    {
        public StringType() : base("String", typeof(UString))
        {
        }

        public static readonly StringType Instance = new StringType();
        public override MetaType DefaultMetaType => StringMetaType.Instance;
    }

    public class UString : IValue
    {
        public MetaType MetaType { get; set; }

        public UString(string value)
        {
            Value = value;
            MetaType = StringMetaType.Instance;
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