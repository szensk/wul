using System;
using System.Collections.Generic;
using System.Diagnostics;
using Wul.Parser.Nodes;

namespace Wul.Interpreter.Types
{
    abstract class Value : IValue
    {
        public MetaType MetaType { get; set; } = null;

        public WulType Type => null;

        public abstract SyntaxNode ToSyntaxNode(SyntaxNode parent);

        public abstract string AsString();

        public abstract object ToObject();

        public static readonly Value Nil = new Nill();

        public static readonly List<IValue> EmptyList = new List<IValue>();

        public static List<IValue> ListWith(params IValue[] values)
        {
            return new List<IValue>(values);
        }
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