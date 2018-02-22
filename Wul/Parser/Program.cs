using System;
using System.Collections.Generic;

namespace Wul.Parser
{
    public class ProgramNode : SyntaxNode
    {
        public List<ListNode> Expressions { get; }

        public ProgramNode(List<ListNode> expressions, string file) : base(null, file)
        {
            Expressions = expressions;
        }

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new ProgramNode(Expressions, File);
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

        public ProgramParser(string fileName = null)
        {
            FileName = fileName;
        }

        private readonly ListParser listParser = new ListParser();

        public override SyntaxNode Parse(string token, SyntaxNode parent = null)
        {
            return Parse(token, 1);
        }

        private SyntaxNode Parse(string token, int lineCount)
        {
            string program = token.Trim();
            //Maps from line # to last character of that line
            Dictionary<int, int> lineMap = new Dictionary<int, int>();

            if (program == "")
            {
                return new ProgramNode(new List<ListNode>(), FileName);
            }

            List<ListNode> expressions = new List<ListNode>();
            int currentIndex = 0;
            int openParenthesis = 0;
            int closeParenthesis = 0;
            int startIndex = 0;
            ProgramNode currentProgram = new ProgramNode(new List<ListNode>(), FileName);

            while (currentIndex < program.Length)
            {
                if (program[currentIndex] == '\n')
                {
                    lineMap.Add(lineCount, currentIndex);
                    lineCount++;
                }
                else if (program[currentIndex] == '(')
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
                    if (endIndex != -1)
                    {
                        lineMap.Add(lineCount, currentIndex);
                        lineCount++;
                        currentIndex = endIndex + 1;
                    }
                    else
                    {
                        currentIndex = program.Length;
                    }
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
                    ListNode expression = (ListNode) listParser.Parse(substring, lineCount, currentProgram);
                    if (expression != null) expressions.Add(expression);
                    startIndex = currentIndex;
                }
            }

            if (startIndex < program.Length && openParenthesis > closeParenthesis)
            {
                int lineEnd;
                if (!lineMap.TryGetValue(lineCount - 1, out lineEnd)) lineEnd = -1;
                throw new ParseException(FileName, lineCount, 0, currentIndex - lineEnd - 1, "unfinished list");
            }

            currentProgram.Expressions.AddRange(expressions);
            return currentProgram;
        }
    }
}
