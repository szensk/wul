using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Wul.Parser
{
    public class WhiteSpace
    {
        public int Devour(string token)
        {
            var match = Regex.Match(token, @"(\S)");
            if (match.Success)
            {
                string found = match.Groups[1].Value;
                return token.IndexOf(found, StringComparison.Ordinal);
            }
            else
            {
                return 0;
            }
        }
    }

    public class ProgramNode : SyntaxNode
    {
        public List<ListNode> Expressions { get; }

        public ProgramNode(List<ListNode> expressions)
        {
            Expressions = expressions;
        }

        public override string AsString()
        {
            return $"Program[{Expressions.Count}]";
        }
    }

    public class ProgramParser : SyntaxNodeParser
    {
        private readonly ListParser listParser = new ListParser();

        public override SyntaxNode Parse(string token)
        {
            string program = token.Trim();

            if (program == "")
            {
                return new ProgramNode(new List<ListNode>());
            }

            List<ListNode> expressions = new List<ListNode>();
            int currentIndex = 0;
            int openParenthesis = 0;
            int closeParenthesis = 0;
            int startIndex = 0;
            while (currentIndex < program.Length)
            {
                if (program[currentIndex] == '(')
                {
                    openParenthesis++;
                }
                else if (program[currentIndex] == ')')
                {
                    closeParenthesis++;
                }
                else if (program[currentIndex] == ';')
                {
                    int endIndex = program.IndexOf('\n', currentIndex);
                    startIndex = endIndex == -1 ? program.Length : endIndex + 1;
                    currentIndex = endIndex == -1 ? program.Length : endIndex + 1;
                    continue;
                }

                if (closeParenthesis > openParenthesis)
                {
                    throw new Exception("Mismatched parenthesis, have fun");
                }

                currentIndex++;
                if (openParenthesis != 0 && openParenthesis == closeParenthesis)
                {
                    string substring = program.Substring(startIndex, currentIndex - startIndex);
                    ListNode expression = (ListNode) listParser.Parse(substring);
                    if (expression != null) expressions.Add(expression);
                    startIndex = currentIndex;
                }
            }

            return new ProgramNode(expressions);
        }
    }
}
