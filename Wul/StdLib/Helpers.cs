using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser;

namespace Wul.StdLib
{
    static class Helpers
    {
        public static IValue Eval(this SyntaxNode node, Scope scope)
        {
            IValue result = WulInterpreter.Interpret(node, scope).FirstOrDefault() ?? Value.Nil;
            while (result is SyntaxNode)
            {
                result = WulInterpreter.Interpret(result as SyntaxNode, scope).FirstOrDefault() ?? Value.Nil;
            }
            return result;
        }

        public static IValue EvalOnce(this SyntaxNode node, Scope scope)
        {
            return WulInterpreter.Interpret(node, scope).FirstOrDefault() ?? Value.Nil;
        }


        public static List<IValue> PushFront(this List<IValue> list, params IValue[] values)
        {
            list.InsertRange(0, values);
            return list;
        }

        public static List<IValue> PushBack(this List<IValue> list, params IValue[] values)
        {
            list.AddRange(values);
            return list;
        }

        public static ProgramNode Parse(string programText, ProgramParser parser = null)
        {
            parser = parser ?? new ProgramParser();
            return (ProgramNode) parser.Parse(programText);
        }

        // full file name
        public static ProgramNode ParseFile(string fileName)
        {
            ProgramParser parser = new ProgramParser(fileName);
            string contents = File.ReadAllText(fileName);
            return Parse(contents, parser);
        }

        // full file name
        public static List<IValue> LoadFile(string fileName, Scope scope)
        {
            ProgramNode program = ParseFile(fileName);
            return WulInterpreter.Interpret(program, scope);
        }
    }
}

