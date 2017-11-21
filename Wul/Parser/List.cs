using System;
using System.Collections.Generic;

namespace Wul.Parser
{
    public class ListNode : SyntaxNode
    {
        public List<SyntaxNode> Children { get; }

        public ListNode(List<SyntaxNode> children)
        {
            Children = children;
        }

        public override string AsString()
        {
            return $"List[{Children.Count}]";
        }
    }

    public class ListParser : SyntaxNodeParser
    {
        private readonly IdentifierParser identifierParser = new IdentifierParser();
        private readonly NumericParser numericParser = new NumericParser();
        private readonly StringParser stringParser = new StringParser();

        public override SyntaxNode Parse(string token)
        {
            if (token.Length < 2) return null;

            //Assume token has been trimmed
            int openIndex = token.IndexOf("(", StringComparison.Ordinal);
            int closeIndex = token.LastIndexOf(")", StringComparison.Ordinal);

            if (openIndex == -1 || closeIndex == -1) return null;

            string inner = token.Substring(openIndex + 1, closeIndex - (openIndex + 1));
            List<SyntaxNode> children = new List<SyntaxNode>();
            
            int currentIndex = 0;
            int openParentheses = 0;
            int closeParentheses = 0;
            int startIndex = 0;
            while (currentIndex < inner.Length)
            {
                if (inner[currentIndex] == '(')
                {
                    openParentheses++;
                }
                else if (inner[currentIndex] == ')')
                {
                    closeParentheses++;
                }

                if (closeParentheses > openParentheses)
                {
                    throw new Exception("Mismatched parenthesis, have fun");
                }

                currentIndex++;
                if ((currentIndex == inner.Length || inner[currentIndex] == ' ' || inner[currentIndex] == ')') && openParentheses == closeParentheses)
                {
                    string currentInner = inner.Substring(startIndex, currentIndex - startIndex);
                    if (stringParser.StartsString(currentInner)) continue;
                    var item = Parse(currentInner) ?? identifierParser.Parse(currentInner) ?? numericParser.Parse(currentInner) ?? stringParser.Parse(currentInner);
                    if (item != null) children.Add(item);
                    startIndex = currentIndex + 1;
                }
            }

            return new ListNode(children);
        }
    }
}
