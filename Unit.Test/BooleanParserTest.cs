using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wul.Parser;

namespace Unit.Test
{
    [TestClass]
    public sealed class BooleanParserTest
    {
        [TestMethod]
        public void BooleanParser_Null()
        {
            BooleanParser parser = new BooleanParser();
            string token = "ok";

            BooleanNode node = (BooleanNode)parser.Parse(token);

            Assert.IsNull(node);
        }

        [TestMethod]
        public void BooleanParser_True()
        {
            BooleanParser parser = new BooleanParser();
            string token = "true";

            BooleanNode node = (BooleanNode)parser.Parse(token);

            Assert.IsTrue(node.Value);
        }

        [TestMethod]
        public void BooleanParser_False()
        {
            BooleanParser parser = new BooleanParser();
            string token = "false";

            BooleanNode node = (BooleanNode)parser.Parse(token);

            Assert.IsFalse(node.Value);
        }

        [TestMethod]
        public void BooleanParser_True_CaseSensitive()
        {
            BooleanParser parser = new BooleanParser();
            string token = "True";

            BooleanNode node = (BooleanNode)parser.Parse(token);

            Assert.IsNull(node);
        }

        [TestMethod]
        public void BooleanParser_False_CaseSensitive()
        {
            BooleanParser parser = new BooleanParser();
            string token = "FALSE";

            BooleanNode node = (BooleanNode)parser.Parse(token);

            Assert.IsNull(node);
        }
    }
}
