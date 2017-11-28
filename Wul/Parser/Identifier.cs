﻿using System.Text.RegularExpressions;

namespace Wul.Parser
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
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(null, obj)) return false;
            IdentifierNode other = obj as IdentifierNode;
            return other != null && Name.Equals(other.Name);
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

    public class IdentifierParser : SyntaxNodeParser
    {
        public override SyntaxNode Parse(string token, SyntaxNode parent = null)
        {
            //TODO this is a mess
            if (token.StartsWith("..") && token.EndsWith("..")) return new IdentifierNode(parent, token);
            if (token.StartsWith("-"))
            {
                if (Regex.Match(token, @"-[0-9]*\.?[0-9]+").Success) return null;
            }

            var match = Regex.Match(token, @"^([a-zA-Z\>\<\+\-\!\@\#\$\%\^\&\*~?=\|/\:]+[a-zA-Z0-9\>\<\+\-\!\@\#\$\%\^\&\*~?\.=\|/\:]*)$");

            if (match.Success)
            {
                string name = match.Groups[1].Value;
                return new IdentifierNode(parent, name);
            }

            return null;
        }
    }
}
