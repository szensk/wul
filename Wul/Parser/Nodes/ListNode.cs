using System.Collections.Generic;

namespace Wul.Parser.Nodes
{
    public class ListNode : ParentSyntaxNode
    {
        public int Line { get; }
        public bool NamedParameterList;

        public ListNode(SyntaxNode parent, List<SyntaxNode> children, int? line = null) : base(parent)
        {
            Line = line ?? 0;
            Children = children;
        }

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new ListNode(parent, Children);
        }

        public override string AsString()
        {
            return $"List[{Children.Count}]";
        }

        public override string ToString()
        {
            List<string> strings = new List<string>();
            foreach (var child in Children)
            {
                strings.Add(child.ToString());
            }
            return "(" + string.Join(' ', strings) + ")";
        }
    }
}