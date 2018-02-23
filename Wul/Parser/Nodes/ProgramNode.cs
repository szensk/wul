using System.Collections.Generic;

namespace Wul.Parser.Nodes
{
    public class ProgramNode : SyntaxNode
    {
        public List<ListNode> Expressions { get; }
        public List<int> LineMap { get; }

        public ProgramNode(List<ListNode> expressions, List<int> lineMap, string file) : base(null, file)
        {
            Expressions = expressions;
            LineMap = lineMap;
        }

        public override SyntaxNode ToSyntaxNode(SyntaxNode parent)
        {
            return new ProgramNode(Expressions, new List<int>(), File);
        }

        public override string AsString()
        {
            return $"Program[{Expressions.Count}]";
        }

        public override string ToString()
        {
            List<string> strings = new List<string>();
            foreach (var list in Expressions)
            {
                strings.Add($"{list}");
            }
            return string.Join("\n", strings);
        }
    }
}