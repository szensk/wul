using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wul.Parser.Nodes;
using Wul.Parser.Parsers;

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
                (def fact (a) (
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

        [TestMethod]
        public void ProgramParser_AnotherExample()
        {
            //Arrange
            const string program = @"
                (def apply (func list)
                  (if (empty? list) 
                    (then ()) 
                    (else 
                      (concat 
                        ((func (first list))) 
                        (apply func (rem list))
                      )
                    )
                  )
                )

                (let +1 (lambda (a) (+ a 1)))
                (apply +1 (1 2 3)) ; returns (2 3 4)";

            ProgramParser parser = new ProgramParser();
            
            //Act
            ProgramNode node = (ProgramNode)parser.Parse(program);

            //Assert
            Assert.AreEqual(3, node.Expressions.Count);
        }

        [TestMethod]
        public void ProgramParser_CommentExample()
        {
            //Arrange
            const string program = @"
                (defn add (a b c)
                  (+ a b) ; this adds the first two arguments
                )

                (print add)
                (print (dump add))";

            ProgramParser parser = new ProgramParser();

            //Act
            ProgramNode node = (ProgramNode)parser.Parse(program);

            //Assert
            Assert.AreEqual(3, node.Expressions.Count);
        }

        [TestMethod]
        public void ProgramParser_ListOfOne()
        {
            //Arrange
            const string program = "(:: System.Console.WriteLine ('ok'))";
            ProgramParser parser = new ProgramParser();

            //Act
            ProgramNode node = (ProgramNode)parser.Parse(program);

            //Assert
            Assert.AreEqual(1, node.Expressions.Count);
        }

        [TestMethod]
        public void ProgramParser_FactorialExample()
        {
            //Arrange
            const string program = "(defn fact (a)\r\n\t(if (< a 2) \r\n\t\t(then 1) \r\n\t\t(else (* a (fact (- a 1))))\r\n\t)\r\n)";
            ProgramParser parser = new ProgramParser();

            //Act
            ProgramNode node = (ProgramNode)parser.Parse(program);

            //Assert
            Assert.AreEqual(1, node.Expressions.Count);
            Assert.AreEqual(4, node.Expressions.First().Children.Count);
        }

        [TestMethod]
        public void ProgramParser_CommentWithParenthesis()
        {
            //Arrange
            const string program = "(defn cos (a) (identity a) \r\n;(that is it)\r\n)";
            ProgramParser parser = new ProgramParser();

            //Act
            ProgramNode node = (ProgramNode)parser.Parse(program);

            //Assert
            Assert.AreEqual(1, node.Expressions.Count);
            Assert.AreEqual(4, node.Expressions.First().Children.Count);
        }

        [TestMethod]
        public void ProgramParser_LeadingCommentWithParenthesis()
        {
            //Arrange
            const string program = "; test\r\n; (that is it)\r\n(print 'ok')";
            ProgramParser parser = new ProgramParser();

            //Act
            ProgramNode node = (ProgramNode)parser.Parse(program);

            //Assert
            Assert.AreEqual(1, node.Expressions.Count);
            Assert.AreEqual(2, node.Expressions.First().Children.Count);
        }

        [TestMethod]
        public void ProgramParser_CommentWithStartingParenthesis()
        {
            //Arrange
            const string program = "(defn test ()\r\n"
                                  +"   ;(unpack\r\n"
                                  +"        (?? 1 2)\r\n"
                                  +"    ;)\r\n"
                                  +")\r\n";
            ProgramParser parser = new ProgramParser();

            //Act
            ProgramNode node = (ProgramNode)parser.Parse(program);

            //Assert
            Assert.AreEqual(1, node.Expressions.Count);
            Assert.AreEqual(4, node.Expressions.First().Children.Count);
        }
    }
}
