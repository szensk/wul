using System.Collections.Generic;
using Wul.Parser.Nodes;

namespace Wul.Parser.Parsers
{
    public class ProgramParser : SyntaxNodeParser
    {
        private string FileName { get; }
        private static readonly ListParser ListParser = new ListParser();
        private readonly List<int> LineMap;

        public ProgramParser(string fileName = null)
        {
            FileName = fileName;
            LineMap = new List<int>();
        }
        
        public override SyntaxNode Parse(string token, SyntaxNode parent = null)
        {
            return Parse(token, 1);
        }

        private ProgramNode Parse(string program, int lineCount)
        {
            if (program == "")
            {
                return new ProgramNode(new List<ListNode>(), new List<int>(), FileName);
            }

            List<ListNode> expressions = new List<ListNode>();
            int currentIndex = 0;
            int openParenthesis = 0;
            int closeParenthesis = 0;
            int startIndex = 0;
            ProgramNode currentProgram = new ProgramNode(new List<ListNode>(), LineMap, FileName);

            while (currentIndex < program.Length)
            {
                char currentChar = program[currentIndex];
                if (currentChar == '\n')
                {
                    LineMap.Add(currentIndex);
                    lineCount++;
                }
                else if (currentChar == '(')
                {
                    openParenthesis++;
                }
                else if (currentChar == ')')
                {
                    closeParenthesis++;
                }
                else if (currentChar  == ';')
                {
                    int endIndex = program.IndexOf('\n', currentIndex);
                    if (endIndex != -1)
                    {
                        LineMap.Add(currentIndex);
                        lineCount++;
                        currentIndex = endIndex + 1;
                    }
                    else
                    {
                        currentIndex = program.Length;
                    }
                    continue;
                }
                else if (!char.IsWhiteSpace(currentChar) && openParenthesis == closeParenthesis)
                {
                    throw currentProgram.CreateParseException(lineCount, currentIndex, "garbage in program");

                }

                if (closeParenthesis > openParenthesis)
                {
                    throw currentProgram.CreateParseException(lineCount, currentIndex, "mismatched parenthesis");
                }

                currentIndex++;
                if (openParenthesis > 0 && openParenthesis == closeParenthesis)
                {
                    string substring = program.Substring(startIndex, currentIndex - startIndex);
                    ListNode expression = (ListNode) ListParser.Parse(substring, lineCount, currentProgram);
                    if (expression != null) expressions.Add(expression);
                    startIndex = currentIndex;
                }
            }

            if (startIndex < program.Length && openParenthesis > closeParenthesis)
            {
                throw currentProgram.CreateParseException(lineCount, currentIndex, "unfinished list");
            }

            currentProgram.Expressions.AddRange(expressions);
            return currentProgram;
        }
    }
}
