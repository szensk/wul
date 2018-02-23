using System.Text.RegularExpressions;
using Wul.Parser.Nodes;

namespace Wul.Parser.Parsers
{
    public class IdentifierParser : SyntaxNodeParser
    {
        public override SyntaxNode Parse(string token, SyntaxNode parent = null)
        {
            //TODO this is a mess
            if (token.StartsWith("..") && token.EndsWith("..")) return new IdentifierNode(parent, token);
            if (token.StartsWith("-"))
            {
                if (Regex.Match(token, @"-[0-9]*\.?[0-9]+").Success) return null;
            }

            var match = Regex.Match(token, @"^([a-zA-Z\>\<\+\-\!\@\#\$\%\^\&\*~?=\|/\:_]+[a-zA-Z0-9\>\<\+\-\!\@\#\$\%\^\&\*~?\.=\|/\:_]*)$");

            if (match.Success)
            {
                string name = match.Groups[1].Value;
                return new IdentifierNode(parent, name);
            }

            return null;
        }
    }
}
