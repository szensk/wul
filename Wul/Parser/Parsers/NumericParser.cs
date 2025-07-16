using System.Text.RegularExpressions;
using Wul.Parser.Nodes;

namespace Wul.Parser.Parsers
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

    static class UnderscoreInteger
    {
        public static bool Literal(string token)
        {
            var match = Regex.Match(token, @"^\-?[0-9_]+$");
            return match.Success;
        }

        public static bool HexLiteral(string token)
        {
            var match = Regex.Match(token, @"^0x[0-9a-fA-F_]+$");
            return match.Success;
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
            else if (UnderscoreInteger.Literal(token))
            {
                return new NumericNode(parent, token, underscore: true);
            }
            else if (UnderscoreInteger.HexLiteral(token))
            {
                return new NumericNode(parent, token, underscore: true, hex: true);
            }
            else
            {
                return null;
            }
        }
    }
}
