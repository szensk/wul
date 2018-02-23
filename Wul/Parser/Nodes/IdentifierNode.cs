using System;

namespace Wul.Parser.Nodes
{
    public class IdentifierNode : SyntaxNode
    {
        public string Name { get; }

        public IdentifierNode(SyntaxNode parent, string name) : base(parent)
        {
            Name = name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj)) return true;
            if (Object.ReferenceEquals(null, obj)) return false;
            return obj is IdentifierNode other && Name.Equals(other.Name);
        }

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new IdentifierNode(parent, Name);
        }

        public override string AsString()
        {
            return $"Identifer[{Name}]";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}