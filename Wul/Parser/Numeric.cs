using System.Text.RegularExpressions;

namespace Wul.Parser
{
    //TODO for hex literals etc
    class Digit
    {
        
    }

    class Integer
    {
        public static bool Literal(string token)
        {
            var match = Regex.Match(token, @"^\-?[0-9]+$");
            return match.Success;
        }    
    }

    class Decimal
    {
        public static bool Literal(string token)
        {
            var match = Regex.Match(token, @"^\-?[0-9]*\.[0-9]+$");
            return match.Success;
        }
    }

    class NumericNode : SyntaxNode
    {
        public NumericNode(SyntaxNode parent, double value) : base(parent)
        {
            Value = value;
        }

        public NumericNode(SyntaxNode parent, string match) : base(parent)
        {
            Value = double.Parse(match);
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

    class NumericParser : SyntaxNodeParser
    {
        public override SyntaxNode Parse(string token, SyntaxNode parent = null)
        {
            if (Integer.Literal(token) || Decimal.Literal(token))
            {
                return new NumericNode(parent, token);
            }
            else
            {
                return null;
            }
        }
    }
}
