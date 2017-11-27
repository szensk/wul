using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wul.Interpreter.Types;

namespace Unit.Test.Types
{
    [TestClass]
    public sealed class RangeTest
    {
        [TestMethod]
        public void Range_Forward_OneIncrement_Count()
        {
            //Arrange
            int expected = 5;

            var range = new Range(0, 4, 1);

            //Act
            int count = range.Count;

            //Assert
            Assert.AreEqual(expected, count);
        }

        [TestMethod]
        public void Range_Forward_OneIncrement_NoEnd_Count()
        {
            //Arrange
            double expected = double.PositiveInfinity;

            var range = new Range(0, null, 1);

            //Act
            double count = range.Count;

            //Assert
            Assert.AreEqual(expected, count);
        }

        [TestMethod]
        public void Range_Forward_TwoIncrement_Count()
        {
            //Arrange
            int expected = 3;

            var range = new Range(0, 4, 2);

            //Act
            int count = range.Count;

            //Assert
            Assert.AreEqual(expected, count);
        }

        [TestMethod]
        public void Range_Forward_MassiveIncrement_Count()
        {
            //Arrange
            int expected = 1;

            var range = new Range(0, 4, 5);

            //Act
            int count = range.Count;

            //Assert
            Assert.AreEqual(expected, count);
        }

        [TestMethod]
        public void Range_Backward_OneIncrement_Count()
        {
            //Arrange
            int expected = 5;

            var range = new Range(4, 0, -1);

            //Act
            int count = range.Count;

            //Assert
            Assert.AreEqual(expected, count);
        }

        [TestMethod]
        public void Range_Backward_TwoIncrement_Count()
        {
            //Arrange
            int expected = 3;

            var range = new Range(4, 0, -2);

            //Act
            int count = range.Count;

            //Assert
            Assert.AreEqual(expected, count);
        }

        [TestMethod]
        public void Range_Backward_MassiveIncrement_Count()
        {
            //Arrange
            int expected = 1;

            var range = new Range(4, 0, -5);

            //Act
            int count = range.Count;

            //Assert
            Assert.AreEqual(expected, count);
        }
    }
}
