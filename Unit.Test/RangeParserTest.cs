using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wul.Parser;

namespace Unit.Test
{
    [TestClass]
    public sealed class RangeParserTest
    {
        [TestMethod]
        public void Range_WithListChild()
        {
            //Arrange
            string range = "[(- (# l) 1) 0 -2]";
            RangeParser parser = new RangeParser();
            
            //Act
            RangeNode node = (RangeNode) parser.Parse(range);

            //Assert
            Assert.AreEqual(3, node.Children.Count);
        }
    }
}
