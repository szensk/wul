using System;
using System.Collections.Generic;
using System.Linq;

namespace Wul.Parser
{
    public class ProgramNode : SyntaxNode
    {
        public List<ListNode> Expressions { get; }

        public ProgramNode(List<ListNode> expressions) : base(null)
        {
            Expressions = expressions;
        }

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new ProgramNode(Expressions);
        }

        public override string AsString()
        {
            return $"Program[{Expressions.Count}]";
        }

        public override string ToString()
        {
            List<string> strings = new List<string>();
            foreach (var list in Expressions)
            {
                strings.Add($"{list}");
            }
            return string.Join("\n", strings);
        }
    }

    public class ProgramParser : SyntaxNodeParser
    {
        private readonly string FileName;

        public ProgramParser(string fileName)
        {
            FileName = fileName;
        }

        private readonly ListParser listParser = new ListParser();

        public override SyntaxNode Parse(string token, SyntaxNode parent = null)
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
            ProgramNode currentProgram = new ProgramNode(new List<ListNode>());

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
                    currentIndex = endIndex == -1 ? program.Length : endIndex + 1;
                    continue;
                }

                if (closeParenthesis > openParenthesis)
                {
                    throw new Exception("Mismatched parenthesis, have fun");
                }

                currentIndex++;
                if (openParenthesis > 0 && openParenthesis == closeParenthesis)
                {
                    string substring = program.Substring(startIndex, currentIndex - startIndex);
                    ListNode expression = (ListNode) listParser.Parse(substring, currentProgram);
                    if (expression != null) expressions.Add(expression);
                    startIndex = currentIndex;
                }
            }

            if (startIndex < program.Length && openParenthesis > closeParenthesis)
            {
                int line = token.Substring(0, startIndex).Count(c => c == '\n');
                throw new ParseException(FileName, line, startIndex, currentIndex, "unfinished list");
            }

            currentProgram.Expressions.AddRange(expressions);
            return currentProgram;
        }
    }
}
