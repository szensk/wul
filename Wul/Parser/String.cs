using System;

namespace Wul.Parser
{
    public class StringNode : SyntaxNode
    {
        public string Value { get; }

        public StringNode(string value)
        {
            Value = value;
        }

        public override string AsString()
        {
            return $"String[{Value}]";
        }
    }

    public class StringParser : SyntaxNodeParser
    {
        private string[] Symbols = {"'", "\""};

        public bool StartsString(string token)
        {
            int openQuoteIndex = token.IndexOf("\"", StringComparison.Ordinal);
            int closeQuoteIndex = token.LastIndexOf("\"", StringComparison.Ordinal);
            return openQuoteIndex != -1 && closeQuoteIndex == openQuoteIndex;
        }

        public override SyntaxNode Parse(string token)
        {
            if (token.Length < 2) return null;

            int openQuoteIndex = token.IndexOf("\"", StringComparison.Ordinal);
            int closeQuoteIndex = token.LastIndexOf("\"", StringComparison.Ordinal);

            if (closeQuoteIndex == -1 || openQuoteIndex == closeQuoteIndex) return null;

            string value = token.Substring(openQuoteIndex + 1, closeQuoteIndex - (openQuoteIndex + 1));

            return new StringNode(value);
        }
    }
}
