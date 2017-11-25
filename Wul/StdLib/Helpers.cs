﻿using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    static class Helpers
    {
        public static IValue Eval(this SyntaxNode node, Scope scope)
        {
            IValue result = WulInterpreter.Interpret(node, scope) ?? Value.Nil;
            while (result is SyntaxNode)
            {
                result = WulInterpreter.Interpret(result as SyntaxNode, scope) ?? Value.Nil;
            }
            return result;
        }
    }
}
