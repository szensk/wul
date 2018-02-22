using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Wul.Parser
{
    public class ListNode : SyntaxNode
    {
        public int Line { get; }
        public bool NamedParameterList;
        public List<SyntaxNode> Children { get; }

        public ListNode(SyntaxNode parent, List<SyntaxNode> children, int? line = null) : base(parent)
        {
            Line = line ?? 0;
            Children = children;
        }

        public List<IdentifierNode> IdentifierNodes()
        {
            List<IdentifierNode> identifierNodes = new List<IdentifierNode>();
            foreach (SyntaxNode node in Children)
            {
                if (node is IdentifierNode identifierNode)
                {
                    identifierNodes.Add(identifierNode);
                }
                if (node is InterpolatedStringNode interpolatedString)
                {
                    identifierNodes.AddRange(interpolatedString.ReferencedNames);
                }
                if (node is ListNode listNode)
                {
                    identifierNodes.AddRange(listNode.IdentifierNodes());
                }
            }

            return identifierNodes;
        }

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new ListNode(parent, Children);
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

        private bool StartsComment(string token)
        {
            return Regex.Match(token, ";(.*)$").Success;
        }

        public override SyntaxNode Parse(string token, SyntaxNode parent = null)
        {
            return Parse(token, 1, parent);
        }

        private void QuoteChild(int childShouldQuote, ListNode currentList, SyntaxNode itemToAdd, List<SyntaxNode> childList)
        {
            if (childShouldQuote > 0)
            {
                ListNode quoteList = new ListNode(currentList, new List<SyntaxNode>());
                if (childShouldQuote > 1)
                {
                    ListNode secondQuoteList = new ListNode(quoteList, new List<SyntaxNode>());
                    secondQuoteList.Children.Add(new IdentifierNode(secondQuoteList, "quote"));
                    secondQuoteList.Children.Add(new IdentifierNode(secondQuoteList, "quote"));
                    quoteList.Children.Add(secondQuoteList);
                }
                else
                {
                    quoteList.Children.Add(new IdentifierNode(quoteList, "quote"));

                }
                quoteList.Children.Add(itemToAdd);
                childList.Add(quoteList);
            }
            else
            {
                childList.Add(itemToAdd);
            }
        }

        private bool IsNamedParameter(SyntaxNode item)
        {
            var identifer = item as IdentifierNode;
            return identifer?.Name.EndsWith(':') ?? false;
        }

        private string GetInnerString(string token)
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

           return token.Substring(openIndex + 1, lastCloseIndex - (openIndex + 1));
        }

        public SyntaxNode Parse(string token, int lineCount, SyntaxNode parent = null)
        {
            string inner = GetInnerString(token);
            if (inner == null) return null;
            if (string.IsNullOrWhiteSpace(inner)) return new ListNode(parent, new List<SyntaxNode>());

            int currentIndex = 0;
            int openParentheses = 0;
            int closeParentheses = 0;
            int startIndex = 0;
            bool startedString = false;
            bool startedRange = false;

            ListNode currentList = new ListNode(parent, new List<SyntaxNode>(), lineCount);
            List<SyntaxNode> children = new List<SyntaxNode>();
            
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
                    throw new Exception($"Mismatched parenthesis:\n\t{inner}");
                }

                currentIndex++;
                if ((currentIndex == inner.Length || char.IsWhiteSpace(inner[currentIndex]) || inner[currentIndex] == ')') && openParentheses == closeParentheses)
                {
                    var nextInner = inner.Substring(startIndex, currentIndex - startIndex);
                    lineCount += nextInner.Count(c => c == '\n');
                    string currentInner = nextInner.Trim();
                    int childShouldQuote = 0;
                    if (currentInner.StartsWith('`'))
                    {
                        if (currentInner.StartsWith("``"))
                        {
                            childShouldQuote++;
                        }
                        currentInner = currentInner.Replace("`", "");
                        childShouldQuote++;
                    }
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
                                      ?? rangeParser.Parse(currentInner, currentList);

                    if (item == null)
                    {
                        var innerString = GetInnerString(currentInner);
                        if (innerString == null)
                        {
                            startIndex = currentIndex;
                            currentInner = null;
                        }
                        else 
                        {
                            item =  string.IsNullOrWhiteSpace(innerString)
                                ? new ListNode(currentList, new List<SyntaxNode>())
                                : Parse(currentInner, lineCount, currentList);
                        } 
                    }

                    if (item != null)
                    {
                        if (IsNamedParameter(item)) currentList.NamedParameterList = true;
                        if (item is StringNode) startedString = false;
                        if (item is RangeNode) startedRange = false;
                        QuoteChild(childShouldQuote, currentList, item, children);
                    }
                    else if (!string.IsNullOrWhiteSpace(currentInner) && StartsComment(currentInner))
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
