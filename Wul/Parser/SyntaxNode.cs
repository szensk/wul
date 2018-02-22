using System;
using Wul.Interpreter;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;

namespace Wul.Parser
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
    }

    public abstract class SyntaxNodeParser
    {
        public abstract SyntaxNode Parse(string token, SyntaxNode parent = null);
    }
}
