using Wul.Interpreter;

namespace Wul.Parser
{
    public abstract class SyntaxNode : IValue
    {
        private static readonly SyntaxNodeMetaType metaType = new SyntaxNodeMetaType();
        public MetaType MetaType => metaType;
        public abstract string AsString();

        public object ToObject()
        {
            //No idea what to do here
            return this;
        }
    }

    public abstract class SyntaxNodeParser
    {
        public abstract SyntaxNode Parse(string token);
    }
}
