namespace Wul.Parser.Nodes
{
    public class BooleanNode : SyntaxNode
    {
        public BooleanNode(SyntaxNode parent, bool value) : base(parent)
        {
            Value = value;
        }
        
        public bool Value { get; }

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new BooleanNode(parent, Value);
        }

        //AsString is used by Wul
        public override string AsString()
        {
            return $"{Value}";
        }

        //ToString is used by C#
        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
