using Wul.Interpreter;
using Wul.Interpreter.MetaTypes;
using Wul.Interpreter.Types;

namespace Wul.Parser
{
    public class SyntaxNodeType : WulType
    {
        public SyntaxNodeType() : base("SyntaxNode", typeof(SyntaxNode))
        {
            
        }

        public static readonly SyntaxNodeType Instance = new SyntaxNodeType();
    }

    public abstract class SyntaxNode : IValue
    {
        private static readonly SyntaxNodeMetaType metaType = new SyntaxNodeMetaType();
        public MetaType MetaType { get; set; } = metaType;

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
