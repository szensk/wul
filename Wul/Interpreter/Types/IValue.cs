﻿using Wul.Parser.Nodes;

namespace Wul.Interpreter.Types
{
    public interface IValue
    {    
        //Meta-type
        MetaType MetaType { get; set; }

        WulType Type { get; }

        SyntaxNode ToSyntaxNode(SyntaxNode parent);

        string AsString();

        object ToObject();
    }
}
