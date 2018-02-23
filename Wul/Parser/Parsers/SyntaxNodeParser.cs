using Wul.Parser.Nodes;

namespace Wul.Parser.Parsers
{
    public abstract class SyntaxNodeParser
    {
        public abstract SyntaxNode Parse(string token, SyntaxNode parent = null);
    }
}