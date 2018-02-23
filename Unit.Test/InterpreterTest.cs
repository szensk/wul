using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wul.Interpreter;
using Wul.Parser.Nodes;
using Wul.Parser.Parsers;

namespace Unit.Test
{
    [TestClass]
    public class InterpreterTest
    {
        [TestMethod]
        public void Interpreter_PrintNil()
        {
            ProgramParser parser = new ProgramParser();
            string program = "(print nil)";

            ProgramNode node = (ProgramNode) parser.Parse(program);

            WulInterpreter.Interpret(node);
        }
    }
}
