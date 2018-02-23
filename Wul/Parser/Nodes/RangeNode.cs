using System.Collections.Generic;

namespace Wul.Parser.Nodes
{
    public class RangeNode : SyntaxNode
    {
        public List<SyntaxNode> Children { get; }

        public RangeNode(SyntaxNode parent, List<SyntaxNode> children) : base(parent)
        {
            Children = children;
        }

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new RangeNode(parent, Children);
        }

        public override string AsString()
        {
            return $"Range[{Children.Count}]";
        }

        public override string ToString()
        {
            List<string> strings = new List<string>();
            foreach (var child in Children)
            {
                strings.Add(child.ToString());
            }
            return "[" + string.Join(' ', strings) + "]";
        }
    }
}