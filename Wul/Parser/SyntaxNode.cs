using Wul.Interpreter;

namespace Wul.Parser
{
    public abstract class SyntaxNode : IValue
    {
        public abstract string AsString();
    }

    public abstract class SyntaxNodeParser
    {
        public abstract SyntaxNode Parse(string token);
    }
}
