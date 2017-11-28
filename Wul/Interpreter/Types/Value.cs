using Wul.Parser;

namespace Wul.Interpreter.Types
{
    abstract class Value : IValue
    {
        //TODO do I want a nil meta type?
        public MetaType MetaType { get; set; } = null;

        public WulType Type => null;

        public abstract SyntaxNode ToSyntaxNode(SyntaxNode parent);

        public abstract string AsString();

        public abstract object ToObject();

        public static Value Nil = new Nill();
    }

    internal class Nill : Value
    {
        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new IdentifierNode(parent, "nil");
        }

        public override string AsString()
        {
            return "nil";
        }

        public override object ToObject()
        {
            return null;
        }
    }
}