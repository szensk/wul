using System;
using System.Diagnostics;
using Wul.Parser;

namespace Wul.Interpreter.Types
{
    abstract class Value : IValue
    {
        //TODO do I want a nil meta type?
        public MetaType Metatype { get; set; } = null;

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

    //For development only
    internal class Sentinel : Value
    {
        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            throw new NotImplementedException();
        }

        public override string AsString()
        {
            return "nel";
        }

        public override object ToObject()
        {
            throw new NotImplementedException();
        }

        ~Sentinel()
        {
            Debug.WriteLine("Deleting sentinel value");
        }
    }
}