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
    }

    public class ListParser : SyntaxNodeParser
    {
        private readonly IdentifierParser identifierParser = new IdentifierParser();
        private readonly NumericParser numericParser = new NumericParser();

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
            int openParenthesis = 0;
            int closeParenthesis = 0;
            int startIndex = 0;
            while (currentIndex < inner.Length)
            {
                if (inner[currentIndex] == '(')
                {
                    openParenthesis++;
                }
                else if (inner[currentIndex] == ')')
                {
                    closeParenthesis++;
                }

                if (closeParenthesis > openParenthesis)
                {
                    throw new Exception("Mismatched parenthesis, have fun");
                }

                currentIndex++;
                if ((currentIndex == inner.Length || inner[currentIndex] == ' ' || inner[currentIndex] == ')') && openParenthesis == closeParenthesis)
                {
                    string currentInner = inner.Substring(startIndex, currentIndex - startIndex);
                    var item = Parse(currentInner) ?? identifierParser.Parse(currentInner) ?? numericParser.Parse(currentInner);
                    if (item != null) children.Add(item);
                    startIndex = currentIndex + 1;
                }
            }

            return new ListNode(children);
        }
    }
}
