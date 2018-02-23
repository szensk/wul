using Wul.Interpreter;

namespace Wul.Parser.Nodes
{
    public class StringNode : SyntaxNode
    {
        public virtual bool Interpolated => false;

        protected string _Value { get; }

        public virtual string Value(Scope scope = null)
        {
            return _Value;
        }

        public StringNode(SyntaxNode parent, string value) : base(parent)
        {
            _Value = value;
        }

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new StringNode(parent, _Value);
        }

        public override string AsString()
        {
            return $"String[{_Value}]";
        }

        public override string ToString()
        {
            return $"'{_Value}'";
        }
    }
}