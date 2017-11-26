using Wul.Interpreter;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;

namespace Wul.Parser
{
    public abstract class SyntaxNode : IValue
    {
        public MetaType MetaType { get; set; } = SyntaxNodeMetaType.Instance;

        public WulType Type => SyntaxNodeType.Instance;

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
