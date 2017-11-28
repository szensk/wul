﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Wul.Interpreter.Types;

namespace Wul.Parser
{
    public class ListNode : SyntaxNode
    {
        public List<SyntaxNode> Children { get; set; }

        public IValue MacroResult { get; set; } = null;

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

            //Assume token has been trimmed
            int openIndex = token.IndexOf('(');
            int lastCloseIndex = token.LastIndexOf(')');

            if (openIndex == -1 || lastCloseIndex == -1) return null;

            string inner = token.Substring(openIndex + 1, lastCloseIndex - (openIndex + 1));
            inner = Regex.Replace(inner, "([^\"].*)\\s+([^\"].*)", "$1 $2");
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

                if (closeParentheses > openParentheses)
                {
                    throw new Exception("Mismatched parenthesis, have fun");
                }

                currentIndex++;
                if ((currentIndex == inner.Length || inner[currentIndex] == ' ' || inner[currentIndex] == ')') && openParentheses == closeParentheses)
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
                    else if (!string.IsNullOrWhiteSpace(currentInner))
                    {
                        throw new Exception("trash in list");
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
