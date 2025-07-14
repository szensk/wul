using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wul.Interpreter;
using Wul.Interpreter.Types;
using Wul.Parser.Nodes;
using Wul.Parser.Parsers;

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

            Assert.AreEqual("true", node.Value());
        }

        [TestMethod]
        public void StringParser_EmptyString()
        { 
            StringParser parser = new StringParser();
            string token = "\"\"";

            StringNode node = (StringNode)parser.Parse(token);

            Assert.AreEqual("", node.Value());
        }

        [TestMethod]
        public void StringParser_EscapedCharacter()
        {
            StringParser parser = new StringParser();
            string token = "\"\t\"";

            StringNode node = (StringNode)parser.Parse(token);

            Assert.AreEqual("\t", node.Value());
        }

        [TestMethod]
        public void StringParser_EscapedQuote()
        {
            StringParser parser = new StringParser();
            string token = "\"\\\"\"";

            StringNode node = (StringNode)parser.Parse(token);

            Assert.AreEqual("\"", node.Value());
        }

        [TestMethod]
        public void StringParser_Interpolated()
        {
            StringParser parser = new StringParser();
            string token = "\"hello {{{world}}}\"";

            StringNode node = (StringNode)parser.Parse(token);

           Assert.IsTrue(node.Interpolated);
        }


        [TestMethod]
        public void StringParser_Interpolated_Correct()
        {
            StringParser parser = new StringParser();
            string expected = "hello {world}. OK!";

            string token = "\"hello {{{world}}}. OK!\"";

            InterpolatedStringNode node = (InterpolatedStringNode)parser.Parse(token);

            Scope scope = new Scope {["world"] = new WulString("world")};

            Assert.AreEqual(expected, node.Value(scope));
        }

        [TestMethod]
        public void StringParser_NotInterpolated()
        {
            StringParser parser = new StringParser();
            string token = "'hello {world}'";

            StringNode node = (StringNode)parser.Parse(token);

            Assert.IsFalse(node.Interpolated);
        }

        [TestMethod]
        public void StringParser_ListShouldNotBeAString()
        {
            StringParser parser = new StringParser();
            string token = "('hello {world}')";

            StringNode node = (StringNode)parser.Parse(token);

            Assert.IsNull(node);
        }

        [TestMethod]
        public void StringParser_ListOfTwoShouldNotBeAString()
        {
            StringParser parser = new StringParser();
            string token = "('hello' 'world')";

            StringNode node = (StringNode)parser.Parse(token);

            Assert.IsNull(node);
        }

        [TestMethod]
        public void StringParser_NestedInterpoalted()
        {
            StringParser parser = new StringParser();
            string token = "'a \"\" b'";

            StringNode node = (StringNode)parser.Parse(token);

            Assert.AreEqual("a \"\" b", node.Value());
        }
    }
}
