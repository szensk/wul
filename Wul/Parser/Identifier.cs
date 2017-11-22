﻿using System.Text.RegularExpressions;

namespace Wul.Parser
{
    public class IdentifierNode : SyntaxNode
    {
        public string Name { get; }

        public IdentifierNode(string name)
        {
            Name = name;
        }

        public override string AsString()
        {
            return $"Identifer[{Name}]";
        }
    }

    public class IdentifierParser : SyntaxNodeParser
    {
        public override SyntaxNode Parse(string token)
        {
            //Regex lol
            var match = Regex.Match(token, @"^([a-zA-Z\>\<\+\-\!\@\#\$\%\^\&\*~?\.=`]+[a-zA-Z0-9\>\<\+\-\!\@\#\$\%\^\&\*~?\.=]*)$");

            if (match.Success)
            {
                string name = match.Groups[1].Value;
                return new IdentifierNode(name);
            }

            return null;
        }
    }
}