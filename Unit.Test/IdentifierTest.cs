using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wul.Parser.Nodes;
using Wul.Parser.Parsers;

namespace Unit.Test
{
    [TestClass]
    public class IdentifierTest
    {
        [TestMethod]
        public void Identifer_CannotContain_OpenParenthesis()
        {
            IdentifierParser parser = new IdentifierParser();
            string token = "(";

            IdentifierNode node = (IdentifierNode)parser.Parse(token);

            Assert.IsNull(node);
        }

        [TestMethod]
        public void Identifer_CannotContain_CloseParenthesis()
        {
            IdentifierParser parser = new IdentifierParser();
            string token = ")";

            IdentifierNode node = (IdentifierNode)parser.Parse(token);

            Assert.IsNull(node);
        }

        [TestMethod]
        public void Identifer_CannotContain_SingleNumber()
        {
            IdentifierParser parser = new IdentifierParser();
            string token = "2";

            IdentifierNode node = (IdentifierNode)parser.Parse(token);

            Assert.IsNull(node);
        }

        [TestMethod]
        public void Identifer_CannotStartWithDash()
        {
            IdentifierParser parser = new IdentifierParser();
            string token = "-";

            IdentifierNode node = (IdentifierNode)parser.Parse(token);

            Assert.AreEqual(token, node.Name);
        }

        [TestMethod]
        public void Identifer_Operator_IsValidIdentifer()
        {
            IdentifierParser parser = new IdentifierParser();
            string token = "<=";

            IdentifierNode node = (IdentifierNode)parser.Parse(token);

            Assert.AreEqual(token, node.Name);
        }

        [TestMethod]
        public void Identifer_SingleChar_IsValidIdentifer()
        {
            IdentifierParser parser = new IdentifierParser();
            string token = "a";

            IdentifierNode node = (IdentifierNode) parser.Parse(token);

            Assert.AreEqual(token, node.Name);
        }

        [TestMethod]
        public void Identifer_MultipleCharacters_IsValidIdentifier()
        {
            IdentifierParser parser = new IdentifierParser();
            string token = "ab";

            IdentifierNode node = (IdentifierNode) parser.Parse(token);

            Assert.AreEqual(token, node.Name);
        }

        [TestMethod]
        public void Identifer_CanContainQuestionMark_IsValidIdentifier()
        {
            IdentifierParser parser = new IdentifierParser();
            string token = "ab?";

            IdentifierNode node = (IdentifierNode) parser.Parse(token);

            Assert.AreEqual(token, node.Name);
        }

        [TestMethod]
        public void Identifer_VariableArgument_IsValidIdentifier()
        {
            IdentifierParser parser = new IdentifierParser();
            string token = "...";

            IdentifierNode node = (IdentifierNode)parser.Parse(token);

            Assert.AreEqual(token, node.Name);
        }
    }
}
