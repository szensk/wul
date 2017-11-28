using Wul.Interpreter;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;

namespace Wul.Parser
{
    public abstract class SyntaxNode : IValue
    {
        public MetaType MetaType { get; set; } = SyntaxNodeMetaType.Instance;

        public WulType Type => SyntaxNodeType.Instance;

        public SyntaxNode Parent { get; private set; }

        protected SyntaxNode(SyntaxNode parent)
        {
            Parent = parent;
        }

        public SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            //TODO is setting parent a bad idea
            Parent = parent;
            return this;
        }

        public abstract string AsString();

        public object ToObject()
        {
            //No idea what to do here
            return this;
        }
    }

    public abstract class SyntaxNodeParser
    {
        public abstract SyntaxNode Parse(string token, SyntaxNode parent = null);
    }
}
