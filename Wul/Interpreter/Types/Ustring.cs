using Wul.Interpreter.MetaTypes;
using Wul.Parser;

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

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(null, obj)) return false;
            UString other = obj as UString;
            return other != null && Value.Equals(other.Value);
        }

        public string Value { get; }

        public WulType Type => StringType.Instance;

        public SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new StringNode(parent, Value);
        }

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