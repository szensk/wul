using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wul.Parser;

namespace Unit.Test
{
    [TestClass]
    public class HexNumericParserTest
    {
        [TestMethod]
        public void HexNumeric_StartWith0x()
        {
            NumericParser parser = new NumericParser();
            string token = "0xf2";

            NumericNode node = (NumericNode)parser.Parse(token);

            Assert.AreEqual(242, node.Value);
        }

        [TestMethod]
        public void HexNumeric_AllowsCapital()
        {
            NumericParser parser = new NumericParser();
            string token = "0xF2";

            NumericNode node = (NumericNode)parser.Parse(token);

            Assert.AreEqual(242, node.Value);
        }

        [TestMethod]
        public void HexNumeric_MustStartWith0x()
        {
            NumericParser parser = new NumericParser();
            string token = "f2";

            NumericNode node = (NumericNode)parser.Parse(token);

            Assert.IsNull(node);
        }
    }
}
