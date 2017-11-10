using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wul.Parser;

namespace Unit.Test
{
    [TestClass]
    public sealed class ListTest
    {
        [TestMethod]
        public void List_EmptyList()
        {
            ListParser parser = new ListParser();
            string list = "()";

            ListNode node = (ListNode) parser.Parse(list);

            Assert.IsTrue(node.Children.Count == 0);
        }

        [TestMethod]
        public void List_ListOfOneNumeric()
        {
            ListParser parser = new ListParser();
            string list = "(1)";

            ListNode node = (ListNode)parser.Parse(list);

            Assert.IsTrue(node.Children.Count == 1);
        }

        [TestMethod]
        public void List_ListOfTwoNumerics()
        {
            ListParser parser = new ListParser();
            string list = "(1 2)";

            ListNode node = (ListNode)parser.Parse(list);

            Assert.IsTrue(node.Children.Count == 2);
        }

        [TestMethod]
        public void List_ListOfOneIdentifier()
        {
            ListParser parser = new ListParser();
            string list = "(a)";

            ListNode node = (ListNode)parser.Parse(list);

            Assert.IsTrue(node.Children.Count == 1);
        }

        [TestMethod]
        public void List_ListOfTwoIdentifiers()
        {
            ListParser parser = new ListParser();
            string list = "(a b)";

            ListNode node = (ListNode)parser.Parse(list);

            Assert.AreEqual(2, node.Children.Count);
        }

        [TestMethod]
        public void List_ListOfOneList()
        {
            ListParser parser = new ListParser();
            string list = "(())";

            ListNode node = (ListNode)parser.Parse(list);

            Assert.IsTrue(node.Children.Count == 1);
        }

        [TestMethod]
        public void List_ListOfTwoLists()
        {
            ListParser parser = new ListParser();
            string list = "(() ())";

            ListNode node = (ListNode)parser.Parse(list);

            Assert.IsTrue(node.Children.Count == 2);
        }

        [TestMethod]
        public void List_MixedList()
        {
            ListParser parser = new ListParser();
            string list = "(a (b c) () 1 ok 2)";

            ListNode node = (ListNode)parser.Parse(list);

            Assert.AreEqual(6, node.Children.Count);
        }
    }
}
