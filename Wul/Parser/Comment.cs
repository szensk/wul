using System.Text.RegularExpressions;

namespace Wul.Parser
{
    public class CommentNode : SyntaxNode
    {
        public string Comment { get; }

        public CommentNode(string comment)
        {
            Comment = comment;
        }

        public override string AsString()
        {
            return "CommentNode";
        }
    }

    public class CommentParser : SyntaxNodeParser
    {
        public override SyntaxNode Parse(string token)
        {
            if (token.Length < 1) return null;

            var match = Regex.Match(token, ";(.*)$");
            if (match.Success)
            {
                string comment = match.Groups[1].Value;
                return new CommentNode(comment);
            }
            else
            {
                return null;
            }
        }
    }
}
