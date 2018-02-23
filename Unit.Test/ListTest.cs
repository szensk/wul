using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wul.Parser.Nodes;
using Wul.Parser.Parsers;

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

        [TestMethod]
        public void List_IncludesRange()
        {
            ListParser parser = new ListParser();
            string list = "(def l (list [0 4]))";

            ListNode node = (ListNode) parser.Parse(list);

            Assert.IsTrue(node.Children[2] is ListNode);
        }

        [TestMethod]
        public void List_IncludesGarbage()
        {
            ListParser parser = new ListParser();
            string list = "(kdi i4ji 4k)";

            ListNode node;
            try
            {
                node = (ListNode) parser.Parse(list);
            }
            catch
            {
                return;
            }
            Assert.Fail("Should throw garbage exception");
        }
    }
}
