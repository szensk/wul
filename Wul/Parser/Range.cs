using System;
using System.Collections.Generic;

namespace Wul.Parser
{
    public class RangeNode : SyntaxNode
    {
        public List<SyntaxNode> Children { get; }

        public RangeNode(SyntaxNode parent, List<SyntaxNode> children) : base(parent)
        {
            Children = children;
        }

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new RangeNode(parent, Children);
        }

        public override string AsString()
        {
            return $"Range[{Children.Count}]";
        }

        public override string ToString()
        {
            List<string> strings = new List<string>();
            foreach (var child in Children)
            {
                strings.Add(child.ToString());
            }
            return "[" + string.Join(' ', strings) + "]";
        }
    }

    public class RangeParser : SyntaxNodeParser
    {
        private static readonly IdentifierParser identifierParser = new IdentifierParser();
        private static readonly NumericParser numericParser = new NumericParser();
        private static readonly ListParser listParser = new ListParser();

        public bool StartsRange(string token)
        {
            int openQuoteIndex = token.IndexOf('[');
            int closeQuoteIndex = token.LastIndexOf(']');
            return openQuoteIndex != -1 && closeQuoteIndex == -1;
        }

        public override SyntaxNode Parse(string token, SyntaxNode parent = null)
        {
            if (token.Length < 2) return null;
            if (token.Trim()[0] != '[') return null;

            //Assume token has been trimmed
            int openIndex = token.IndexOf("[", StringComparison.Ordinal);
            int closeIndex = token.IndexOf("]", StringComparison.Ordinal);

            if (openIndex == -1 || closeIndex == -1) return null;

            string inner = token.Substring(openIndex + 1, closeIndex - (openIndex + 1));
            List<SyntaxNode> children = new List<SyntaxNode>();

            int currentIndex = 0;
            int openParentheses = 0;
            int closeParentheses = 0;
            int startIndex = 0;
            bool startedList = false;
            RangeNode currentRange = new RangeNode(parent, new List<SyntaxNode>());
            
            while (currentIndex < inner.Length)
            {
                if (inner[currentIndex] == '[')
                {
                    openParentheses++;
                }
                else if (inner[currentIndex] == ']')
                {
                    closeParentheses++;
                }

                if (closeParentheses > openParentheses)
                {
                    throw new Exception("Mismatched brackets, have fun");
                }

                currentIndex++;
                if ((currentIndex == inner.Length || inner[currentIndex] == ' ' || inner[currentIndex] == ')') && openParentheses == closeParentheses)
                {
                    string currentInner = inner.Substring(startIndex, currentIndex - startIndex);
                    if (listParser.StartsList(currentInner))
                    {
                        startedList = true;
                        continue;
                    }

                    SyntaxNode item = identifierParser.Parse(currentInner, currentRange)
                                      ?? numericParser.Parse(currentInner, currentRange)
                                      ?? listParser.Parse(currentInner, currentRange);

                    if (item != null)
                    {
                        if (item is ListNode) startedList = false;
                        children.Add(item);
                    }
                    else if (!string.IsNullOrWhiteSpace(currentInner))
                    {
                        throw new Exception("trash in range");
                    }
                    startIndex = currentIndex + 1;
                }
            }

            if (children.Count > 3 || children.Count < 1)
            {
                throw new Exception("invalid number of elements in range");
            }
            if (startedList) throw new Exception("unfinsihed list in range");

            currentRange.Children.AddRange(children);
            return currentRange;
        }
    }
}
