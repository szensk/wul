using System.Collections.Generic;

namespace Wul.Parser.Nodes
{
    public class ListNode : SyntaxNode
    {
        public int Line { get; }
        public bool NamedParameterList;
        public List<SyntaxNode> Children { get; }

        public ListNode(SyntaxNode parent, List<SyntaxNode> children, int? line = null) : base(parent)
        {
            Line = line ?? 0;
            Children = children;
        }

        public List<IdentifierNode> IdentifierNodes()
        {
            List<IdentifierNode> identifierNodes = new List<IdentifierNode>();
            foreach (SyntaxNode node in Children)
            {
                if (node is IdentifierNode identifierNode)
                {
                    identifierNodes.Add(identifierNode);
                }
                if (node is InterpolatedStringNode interpolatedString)
                {
                    identifierNodes.AddRange(interpolatedString.ReferencedNames);
                }
                if (node is ListNode listNode)
                {
                    identifierNodes.AddRange(listNode.IdentifierNodes());
                }
            }

            return identifierNodes;
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