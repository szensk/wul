using Wul.Interpreter.MetaTypes;
using Wul.Parser.Nodes;

namespace Wul.Interpreter.Types
{
    public class StringType : WulType
    {
        public StringType() : base("String", typeof(WulString))
        {
        }

        public static readonly StringType Instance = new StringType();
        public override MetaType DefaultMetaType => StringMetaType.Instance;
    }

    public class WulString : IValue
    {
        public MetaType MetaType { get; set; }

        //TODO this should be private
        public WulString(string value)
        {
            Value = value;
            MetaType = StringMetaType.Instance;
        }

        public static explicit operator WulString(string str)
        {
            if (str == string.Empty) return WulString.EmptyString;
            return new WulString(str);
        }

        public static explicit operator string(WulString ustr)
        {
            if (ustr == WulString.EmptyString) return string.Empty;
            return ustr.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null) return false;
            WulString other = obj as WulString;
            return other != null && Value.Equals(other.Value);
        }

        public string Value { get; }

        public WulType Type => StringType.Instance;

        public static readonly WulString EmptyString = new(string.Empty);

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