using System;
using System.Collections.Generic;
using System.Linq;
using Wul.Interpreter;

namespace Wul.Parser.Nodes
{
    public class InterpolatedStringNode : StringNode
    {
        public override bool Interpolated => true;

        public IEnumerable<IdentifierNode> ReferencedNames
        {
            get
            {
                return Enumerable.Select(_chunks, c => c.Interpolation)
                    .Where<SyntaxNode>(c => c != null)
                    .SelectMany(c =>
                    {
                        if (c is IdentifierNode id) return new List<IdentifierNode> {id};
                        if (c is ListNode ln) return ln.IdentifierNodes();
                        return new List<IdentifierNode>();
                    });
            }
        }

        public override string Value(Scope scope = null)
        {
            var strings = Enumerable.Select(_chunks, c =>
                {
                    if (c.String != null) return c.String;
                    //TODO call tostring metamethod
                    if (c.Interpolation != null)
                    {
                        return WulInterpreter.Interpret((SyntaxNode) c.Interpolation, scope).First().AsString();
                    }
                    return null;
                })
                .Where<string>(s => s != null);
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
       
        public InterpolatedStringNode(SyntaxNode parent, string value, Func<string, SyntaxNode, SyntaxNode> parseInterpolatedSection) : base(parent, value)
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
                        if (length > 0) _chunks.Add(new Chunk(parseInterpolatedSection(interpolation, this)));
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

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            //TODO
            return this; //new InterpolatedStringNode(parent, _Value);
        }

        public override string AsString()
        {
            return $"InterpolatedString[{_Value}]";
        }

        public override string ToString()
        {
            return $"\"{_Value}\"";
        }
    }
}