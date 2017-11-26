using Wul.Interpreter;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;

namespace Wul.Parser
{
    public abstract class SyntaxNode : IValue
    {
        private static readonly SyntaxNodeMetaType metaType = new SyntaxNodeMetaType();
        public MetaType ValueMetaType { get; set; } = metaType;
        public MetaType MetaType => ValueMetaType;
        
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
