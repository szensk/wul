using Wul.Interpreter.MetaTypes;
using Wul.Parser.Nodes;

namespace Wul.Interpreter
{
    public class SyntaxNodeType : WulType
    {
        public SyntaxNodeType() : base("SyntaxNode", typeof(SyntaxNode))
        {
            
        }

        public static readonly SyntaxNodeType Instance = new SyntaxNodeType();
        public override MetaType DefaultMetaType => SyntaxNodeMetaType.Instance;
    }
}