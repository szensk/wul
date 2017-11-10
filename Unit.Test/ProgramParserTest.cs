using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wul.Parser;

namespace Unit.Test
{
    [TestClass]

    public class ProgramParserTest
    {
        [TestMethod]
        public void ProgramParser_EmptyProgram()
        {
            ProgramParser parser = new ProgramParser();
            string program = "";

            ProgramNode node = (ProgramNode) parser.Parse(program);

            Assert.AreEqual(0, node.Expressions.Count);
        }

        [TestMethod]
        public void ProgramParser_OneExpression()
        {
            ProgramParser parser = new ProgramParser();
            string program = "(print ok)";

            ProgramNode node = (ProgramNode)parser.Parse(program);

            Assert.AreEqual(1, node.Expressions.Count);
        }

        [TestMethod]
        public void ProgramParser_TwoExpressions()
        {
            ProgramParser parser = new ProgramParser();
            string program = "(print ok) (die)";

            ProgramNode node = (ProgramNode)parser.Parse(program);

            Assert.AreEqual(2, node.Expressions.Count);
        }

        [TestMethod]
        public void ProgramParser_SimpleExample()
        {
            ProgramParser parser = new ProgramParser();
            const string program = @"
                (def fact a (
                    (if (< a 2) 
                      (then 1)
                      (else (fact (- a 1)))
                    )
                  )
                )

                (print (fact 10))";

            ProgramNode node = (ProgramNode)parser.Parse(program);

            Assert.AreEqual(2, node.Expressions.Count);
        }
    }
}
