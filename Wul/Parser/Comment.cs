using System.Text.RegularExpressions;

namespace Wul.Parser
{
    public class CommentNode : SyntaxNode
    {
        public string Comment { get; }

        public CommentNode(SyntaxNode parent, string comment) : base(parent)
        {
            Comment = comment;
        }

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new CommentNode(parent, Comment);
        }

        public override string AsString()
        {
            return "CommentNode";
        }
    }

    public class CommentParser : SyntaxNodeParser
    {
        public static bool StartsComment(string token)
        {
            return Regex.Match(token, ";(.*)$").Success;
        }

        public override SyntaxNode Parse(string token, SyntaxNode parent = null)
        {
            if (token.Length < 1) return null;

            var match = Regex.Match(token, ";(.*)$");
            if (match.Success)
            {
                string comment = match.Groups[1].Value;
                return new CommentNode(parent, comment);
            }
            else
            {
                return null;
            }
        }
    }
}
