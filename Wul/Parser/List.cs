using System;
using System.Collections.Generic;
using System.Linq;

namespace Wul.Parser
{
    public class ListNode : SyntaxNode
    {
        public List<SyntaxNode> Children { get; set; }

        public ListNode(SyntaxNode parent, List<SyntaxNode> children) : base(parent)
        {
            Children = children;
        }

        public override string AsString()
        {
            return $"List[{Children.Count}]";
        }

        public override string ToString()
        {
            List<string> strings = new List<string>();
            foreach (var child in Children)
            {
                strings.Add(child.ToString());
            }
            return "(" + string.Join(' ', strings) + ")";
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

        public override SyntaxNode Parse(string token, SyntaxNode parent = null)
        {
            if (token.Length < 2) return null;

            //If it starts with a comment, don't include it
            int commentIndex = token.IndexOf(';');
            int openIndex = token.IndexOf('(');
            if (commentIndex != -1 && commentIndex < openIndex)
            {
                int lineIndex = token.IndexOf('\n');
                openIndex = token.IndexOf('(', lineIndex);
            }
            int lastCloseIndex = token.LastIndexOf(')');

            if (openIndex == -1 || lastCloseIndex == -1) return null;

            string inner = token.Substring(openIndex + 1, lastCloseIndex - (openIndex + 1));
            List<SyntaxNode> children = new List<SyntaxNode>();
            
            int currentIndex = 0;
            int openParentheses = 0;
            int closeParentheses = 0;
            int startIndex = 0;
            bool startedString = false;
            bool startedRange = false;
            ListNode currentList = new ListNode(parent, new List<SyntaxNode>());

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
                else if (inner[currentIndex] == ';')
                {
                    int endIndex = inner.IndexOf('\n', currentIndex);
                    currentIndex = endIndex == -1 ? inner.Length : endIndex + 1;
                    continue;
                }

                if (closeParentheses > openParentheses)
                {
                    throw new Exception($"Mismatched parenthesis: {inner}");
                }

                currentIndex++;
                if ((currentIndex == inner.Length || char.IsWhiteSpace(inner[currentIndex]) || inner[currentIndex] == ')') && openParentheses == closeParentheses)
                {
                    string currentInner = inner.Substring(startIndex, currentIndex - startIndex).Trim();
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

                    SyntaxNode item = identifierParser.Parse(currentInner, currentList)
                                      ?? numericParser.Parse(currentInner, currentList)
                                      ?? stringParser.Parse(currentInner, currentList)
                                      ?? rangeParser.Parse(currentInner, currentList)
                                      ?? Parse(currentInner, currentList);
                    if (item != null)
                    {
                        if (item is StringNode) startedString = false;
                        if (item is RangeNode) startedRange = false;
                        children.Add(item);
                    }
                    else if (!string.IsNullOrWhiteSpace(currentInner) && !CommentParser.StartsComment(currentInner))
                    {
                        throw new Exception($"trash in list\n\t'{currentInner}'");
                    }

                    startIndex = currentIndex + 1;
                }
            }

            if (startedString) throw new Exception("unfinished string in list");
            if (startedRange) throw new Exception("unfinished range in list");

            currentList.Children.AddRange(children);
            return currentList;
        }
    }
}
