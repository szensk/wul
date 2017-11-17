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
        public NumericNode(string match)
        {
            Value = double.Parse(match);
        }

        public double Value { get; }
    }

    class NumericParser : SyntaxNodeParser
    {
        public override SyntaxNode Parse(string token)
        {
            if (Integer.Literal(token) || Decimal.Literal(token))
            {
                return new NumericNode(token);
            }
            else
            {
                return null;
            }
        }
    }
}
