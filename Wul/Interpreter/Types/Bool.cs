using Wul.Interpreter.MetaTypes;
using Wul.Parser.Nodes;

namespace Wul.Interpreter.Types
{
    public class BoolType : WulType
    {
        public BoolType() : base("Bool", typeof(Bool))
        {
        }

        public static readonly BoolType Instance = new BoolType();
        public override MetaType DefaultMetaType => BoolMetaType.Instance;
    }

    public class Bool : IValue
    {
        public static Bool True = new Bool(true);
        public static Bool False = new Bool(false);

        public MetaType MetaType { get; set; }

        public readonly bool Value;

        private Bool(bool value)
        {
            Value = value;
            MetaType = BoolMetaType.Instance;
        }

        public static implicit operator bool(Bool b)
        {
            return b.Value;
        }
        
        public WulType Type => BoolType.Instance;

        public SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new BooleanNode(parent, Value);
        }

        public string AsString()
        {
            return $"{Value}";
        }

        public object ToObject()
        {
            return Value;
        }
    }
}
