using System.Collections.Generic;

namespace Wul.Parser.Nodes
{
    public abstract class ParentSyntaxNode : SyntaxNode
    {
        protected ParentSyntaxNode(SyntaxNode parent, string file = null) : base(parent, file)
        {
        }

        public List<SyntaxNode> Children { get; protected set; }

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
                if (node is RangeNode rangeNode)
                {
                    identifierNodes.AddRange(rangeNode.IdentifierNodes());
                }
            }

            return identifierNodes;
        }
    }
}