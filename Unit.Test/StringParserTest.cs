using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wul.Parser;

namespace Unit.Test
{
    [TestClass]
    public sealed class StringParserTest
    {
        [TestMethod]
        public void StringParser_Null()
        {
            StringParser parser = new StringParser();
            string token = "ok";

            StringNode node = (StringNode)parser.Parse(token);

            Assert.IsNull(node);
        }

        [TestMethod]
        public void StringParser_MismatchedQuotes()
        {
            StringParser parser = new StringParser();
            string token = "ok\"";

            StringNode node = (StringNode)parser.Parse(token);

            Assert.IsNull(node);
        }

        [TestMethod]
        public void StringParser_ShortString()
        {
            StringParser parser = new StringParser();
            string token = "\"true\"";

            StringNode node = (StringNode)parser.Parse(token);

            Assert.AreEqual("true", node.Value);
        }

        [TestMethod]
        public void StringParser_EmptyString()
        { 
            StringParser parser = new StringParser();
            string token = "\"\"";

            StringNode node = (StringNode)parser.Parse(token);

            Assert.AreEqual("", node.Value);
        }

        [TestMethod]
        public void StringParser_EscapedCharacter()
        {
            StringParser parser = new StringParser();
            string token = "\"\t\"";

            StringNode node = (StringNode)parser.Parse(token);

            Assert.AreEqual("\t", node.Value);
        }
    }
}
