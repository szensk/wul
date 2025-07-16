using System;

namespace Wul.Parser.Nodes
{
    public class NumericNode : SyntaxNode
    {
        private NumericNode(SyntaxNode parent, double value) : base(parent)
        {
            Value = value;
        }

        public NumericNode(SyntaxNode parent, string match, bool hex = false, bool underscore = false) : base(parent)
        {
            if (underscore) match = match.Replace("_", string.Empty);
            Value = hex ? Convert.ToInt32(match, 16) : double.Parse(match);
        }

        public double Value { get; }

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new NumericNode(parent, Value);
        }

        public override string AsString()
        {
            return $"Numeric[{Value}]";
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}