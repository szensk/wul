using System.Linq;

namespace Wul.Parser
{
    public class BooleanNode : SyntaxNode
    {
        public BooleanNode(bool value)
        {
            Value = value;
        }

        public BooleanNode(string match)
        {
            Value = bool.Parse(match);
        }

        public bool Value { get; }

        public static BooleanNode False = new BooleanNode(false);
        public static BooleanNode True = new BooleanNode(true);
        public override string AsString()
        {
            return $"{Value}";
        }
    }

    public class BooleanParser : SyntaxNodeParser
    {
        public override SyntaxNode Parse(string token)
        {
            if (token.Any(char.IsUpper)) return null;
            if (bool.TryParse(token, out bool value))
            {
                return value ? BooleanNode.True : BooleanNode.False;
            }
            else
            {
                return null;
            }
        }
    }
}
