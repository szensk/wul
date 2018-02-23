using System;
using System.Text;
using Wul.Parser.Nodes;

namespace Wul.Parser.Parsers
{
    public class StringParser : SyntaxNodeParser
    {
        private static readonly IdentifierParser identifierParser = new IdentifierParser();
        private static readonly NumericParser numericParser = new NumericParser();
        private static readonly RangeParser rangeParser = new RangeParser();
        private static readonly ListParser listParser = new ListParser();

        public bool StartsString(string token)
        {
            int openQuoteIndex = token.IndexOf("\"", StringComparison.Ordinal);
            int closeQuoteIndex = token.LastIndexOf("\"", StringComparison.Ordinal);
            if (openQuoteIndex == -1)
            {
                openQuoteIndex = token.IndexOf("'", StringComparison.Ordinal);
                closeQuoteIndex = token.LastIndexOf("'", StringComparison.Ordinal);
            }
            return openQuoteIndex != -1 && closeQuoteIndex == openQuoteIndex;
        }

        private static string Unescape(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            StringBuilder sb = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length;)
            {
                int j = text.IndexOf('\\', i);
                if (j < 0 || j == text.Length - 1) j = text.Length;
                sb.Append(text, i, j - i);
                if (j >= text.Length) break;
                switch (text[j + 1])
                {
                    case 'n':
                        sb.Append('\n');
                        break;  
                    case 'r':
                        sb.Append('\r');
                        break;  
                    case 't':
                        sb.Append('\t');
                        break;  
                    case '\\':
                        sb.Append('\\');
                        break;
                    case '"':
                        sb.Append('"');
                        break;
                    case '\'':
                        sb.Append('\'');
                        break;
                    default:
                        sb.Append('\\').Append(text[j + 1]);
                        break;
                }
                i = j + 2;
            }

            return sb.ToString();
        }
        private SyntaxNode ParseInterpolationString(string interpolation, SyntaxNode parent)
        {
            return listParser.Parse(interpolation)
                   ?? identifierParser.Parse(interpolation)
                   ?? numericParser.Parse(interpolation)
                   ?? Parse(interpolation)
                   ?? rangeParser.Parse(interpolation);
        }

        public override SyntaxNode Parse(string token, SyntaxNode parent = null)
        {
            if (token.Length < 2) return null;
            if (token[0] != '\"' && token[0] != '\'') return null;

            bool interpolated = true;

            int openQuoteIndex = token.IndexOf("\"", StringComparison.Ordinal);
            int closeQuoteIndex = token.LastIndexOf("\"", StringComparison.Ordinal);

            if (openQuoteIndex == -1)
            {
                interpolated = false;
                openQuoteIndex = token.IndexOf("'", StringComparison.Ordinal);
                closeQuoteIndex = token.LastIndexOf("'", StringComparison.Ordinal);
            }

            if (closeQuoteIndex == -1 || openQuoteIndex == closeQuoteIndex) return null;

            string substring = token.Substring(openQuoteIndex + 1, closeQuoteIndex - (openQuoteIndex + 1));
            string value = Unescape(substring);
            
            return interpolated ? new InterpolatedStringNode(parent, value, ParseInterpolationString) : new StringNode(parent, value);
        }
    }
}
