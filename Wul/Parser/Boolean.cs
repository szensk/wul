using System.Linq;

namespace Wul.Parser
{
    public class BooleanNode : SyntaxNode
    {
        public BooleanNode(SyntaxNode parent, bool value) : base(parent)
        {
            Value = value;
        }
        
        public bool Value { get; }

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new BooleanNode(parent, Value);
        }

        //AsString is used by Wul
        public override string AsString()
        {
            return $"{Value}";
        }

        //ToString is used by C#
        public override string ToString()
        {
            return $"{Value}";
        }
    }

    public class BooleanParser : SyntaxNodeParser
    {
        public override SyntaxNode Parse(string token, SyntaxNode parent = null)
        {
            if (token.Any(char.IsUpper)) return null;
            if (bool.TryParse(token, out bool value))
            {
                return new BooleanNode(parent, value);
            }
            else
            {
                return null;
            }
        }
    }
}
