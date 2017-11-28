using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wul.Interpreter;

namespace Wul.Parser
{
    public class StringNode : SyntaxNode
    {
        public virtual bool Interpolated => false;

        protected string _Value { get; }

        public virtual string Value(Scope scope = null)
        {
            return _Value;
        }

        public StringNode(string value)
        {
            _Value = value;
        }
        
        public override string AsString()
        {
            return $"String[{_Value}]";
        }
    }

    public class InterpolatedStringNode : StringNode
    {
        //TODO remove subparsers like this, also present in ListParser
        private static readonly IdentifierParser identifierParser = new IdentifierParser();
        private static readonly NumericParser numericParser = new NumericParser();
        private static readonly StringParser stringParser = new StringParser();
        private static readonly ListParser listParser = new ListParser();

        public override bool Interpolated => true;

        public override string Value(Scope scope = null)
        {
            var strings = _chunks.Select(c =>
                {
                    if (c.String != null) return c.String;
                    if (c.Interpolation != null) return WulInterpreter.Interpret(c.Interpolation, scope).AsString(); //TODO call tostring metamethod
                    return null;
                })
                .Where(s => s != null);
            return string.Join("", strings);
        }

        private readonly List<Chunk> _chunks;

        private class Chunk
        {
            private string RestoreSpecialCharacters(string input)
            {
                return input.Replace("{{", "{").Replace("}}", "}");
            }

            public Chunk(string str)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    String = RestoreSpecialCharacters(str);
                }
            }

            public Chunk(SyntaxNode interpolation)
            {
                Interpolation = interpolation;
            }

            public string String { get; }
            public SyntaxNode Interpolation { get; }
        }

        private SyntaxNode ParseInterpolationString(string interpolation)
        {
            return listParser.Parse(interpolation) ?? identifierParser.Parse(interpolation) ?? numericParser.Parse(interpolation) ?? stringParser.Parse(interpolation);
        }
        
        public InterpolatedStringNode(string value) : base(value)
        {
            _chunks = new List<Chunk>();
            int lastIndex = 0;
            for (int i = 0; i < value.Length; ++i)
            {
                char c = value[i];
                if (c == '{' && i + 1 < value.Length && value[i + 1] != '{')
                {
                    int endBracket = value.IndexOf('}', i + 1);
                    if (endBracket != -1)
                    {
                        string beforeInterpolation = value.Substring(lastIndex, i - lastIndex);
                        int length = endBracket - (i + 1);
                        string interpolation = value.Substring(i + 1, length);
                        _chunks.Add(new Chunk(beforeInterpolation));
                        if (length > 0) _chunks.Add(new Chunk(ParseInterpolationString(interpolation)));
                        lastIndex = endBracket + 1;
                    }
                }
            }
            if (lastIndex < value.Length)
            {
                string endInterpolation = value.Substring(lastIndex, value.Length - lastIndex);
                _chunks.Add(new Chunk(endInterpolation));
            }
        }

        public override string AsString()
        {
            return $"InterpolatedString[{_Value}]";
        }
    }

    public class StringParser : SyntaxNodeParser
    {
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

        public override SyntaxNode Parse(string token)
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
            
            return interpolated ? new InterpolatedStringNode(value) : new StringNode(value);
        }
    }
}
