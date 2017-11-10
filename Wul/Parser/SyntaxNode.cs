namespace Wul.Parser
{
    public abstract class SyntaxNode
    {
    }

    public abstract class SyntaxNodeParser
    {
        public abstract SyntaxNode Parse(string token);
    }
}
