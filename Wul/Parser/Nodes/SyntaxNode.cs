using System;
using Wul.Interpreter;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;

namespace Wul.Parser.Nodes
{
    public abstract class SyntaxNode : IValue
    {
        public MetaType MetaType { get; set; } = SyntaxNodeMetaType.Instance;

        public WulType Type => SyntaxNodeType.Instance;

        public SyntaxNode Parent { get; }

        public string File { get; }

        protected SyntaxNode(SyntaxNode parent, string file = null)
        {
            File = file ?? parent?.File;
            Parent = parent;
        }

        public abstract SyntaxNode ToSyntaxNode(SyntaxNode parent);
        public abstract string AsString();

        public object ToObject()
        {
            throw new InvalidCastException();
        }

        private int GetLastCharacterOfLine(int lineCount, ProgramNode program)
        {
            if (lineCount < program?.LineMap.Count)
            {
                return program.LineMap[lineCount];
            }
            return -1;
        }

        //TODO Fix the line number
        // ListParser sends the line relative to the input it is parsing
        // so in a multi-list program it is not correct
        public ParseException CreateParseException(int line, int currentIndex, string message)
        {
            SyntaxNode program = this;
            while (program != null && !(program is ProgramNode)) program = program.Parent;
            int lineEnd = currentIndex - GetLastCharacterOfLine(line, (ProgramNode) program) - 1;
            return new ParseException(File, line, 0, program == null ? 0 : lineEnd, message);
        }
    }
}
