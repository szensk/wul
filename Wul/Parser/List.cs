﻿using System;
using System.Collections.Generic;
using System.Linq;

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
        private static readonly IdentifierParser identifierParser = new IdentifierParser();
        private static readonly NumericParser numericParser = new NumericParser();
        private static readonly StringParser stringParser = new StringParser();
        private static readonly RangeParser rangeParser = new RangeParser();

        public bool StartsList(string token)
        {
            var chars = token.ToCharArray();
            int countOpen = chars.Count(c => c == '(');
            int countClosed = chars.Count(c => c == ')');
            return countOpen > countClosed;
        }

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
            bool startedString = false;
            bool startedRange = false;
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
                    if (stringParser.StartsString(currentInner)) 
                    {
                        startedString = true;
                        continue;
                    }
                    if (rangeParser.StartsRange(currentInner))
                    {
                        startedRange = true;
                        continue;
                    }
                    SyntaxNode item = identifierParser.Parse(currentInner)
                                      ?? numericParser.Parse(currentInner)
                                      ?? stringParser.Parse(currentInner)
                                      ?? rangeParser.Parse(currentInner)
                                      ?? Parse(currentInner);
                    if (item != null)
                    {
                        if (item is StringNode) startedString = false;
                        if (item is RangeNode) startedRange = false;
                        children.Add(item);
                    }
                    else if (!string.IsNullOrWhiteSpace(currentInner))
                    {
                        throw new Exception("trash in list");
                    }
                    startIndex = currentIndex + 1;
                }
            }

            if (startedString) throw new Exception("unfinished string in list");
            if (startedRange) throw new Exception("unfinished range in list");

            return new ListNode(children);
        }
    }
}
