using System;
using System.Text.RegularExpressions;

namespace Wul.Parser
{
    static class Integer
    {
        public static bool Literal(string token)
        {
            var match = Regex.Match(token, @"^\-?[0-9]+$");
            return match.Success;
        }

        public static bool HexLiteral(string token)
        {
            var match = Regex.Match(token, @"^0x[0-9a-fA-F]+$");
            return match.Success;
        }
    }

    static class Decimal
    {
        public static bool Literal(string token)
        {
            var match = Regex.Match(token, @"^\-?[0-9]*\.[0-9]+$");
            return match.Success;
        }
    }

    public class NumericNode : SyntaxNode
    {
        private NumericNode(SyntaxNode parent, double value) : base(parent)
        {
            Value = value;
        }

        public NumericNode(SyntaxNode parent, string match, bool hex = false) : base(parent)
        {
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

    public class NumericParser : SyntaxNodeParser
    {
        public override SyntaxNode Parse(string token, SyntaxNode parent = null)
        {
            if (Integer.Literal(token) || Decimal.Literal(token))
            {
                return new NumericNode(parent, token);
            }
            else if (Integer.HexLiteral(token))
            {
                return new NumericNode(parent, token, hex: true);
            }
            else
            {
                return null;
            }
        }
    }
}
