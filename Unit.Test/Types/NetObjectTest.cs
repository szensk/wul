using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wul.Interpreter.Types;

namespace Unit.Test.Types
{
    [TestClass]
    public class NetObjectTest
    {
        private class FieldClass
        {
            public string OK;

            public int Method()
            {
                return 5;
            }

            public int Method(string ok)
            {
                return 6;
            }
        }

        [TestMethod]
        public void NetObject_Get_Property_Point()
        {
            var point = new Point(10, 5);
            var netObj = new NetObject(point);
            
            var x = netObj.Get("X");
            var y = netObj.Get("Y");

            Assert.AreEqual(10, x.ToObject());
            Assert.AreEqual(5, y.ToObject());
        }

        [TestMethod]
        public void NetObject_Set_Property()
        {
            var point = new Point(10, 5);
            var netObj = new NetObject(point);

            netObj.Set("X", new NetObject(-10));
            var x = netObj.Get("X");

            Assert.AreEqual(-10, x.ToObject());
        }

        [TestMethod]
        public void NetObject_Get_Property()
        {
            DateTime now = DateTime.Now;
            var netObj = new NetObject(now);

            var date = netObj.Get("Date");

            Assert.AreEqual(now.Date, date.ToObject());
        }

        [TestMethod]
        public void NetObject_Get_Field()
        {
            string expected = "some string";
            var ok = new FieldClass { OK = expected };
            var netObj = new NetObject(ok);

            var actual = netObj.Get("OK");

            Assert.AreEqual(expected, actual.ToObject());
        }

        [TestMethod]
        public void NetObject_Set_Field()
        {
            string expected = "new string";
            var ok = new FieldClass { OK = "old string" };
            var netObj = new NetObject(ok);

            netObj.Set("OK", new NetObject(expected));
            var actual = netObj.Get("OK");

            Assert.AreEqual(expected, actual.ToObject());
        }

        [TestMethod]
        public void NetObject_Call_NoArguments()
        {
            string expected = "some string";
            var ok = new FieldClass { OK = expected };
            var netObj = new NetObject(ok);

            var actual = netObj.Call("Method");

            Assert.AreEqual(5, actual.ToObject());
        }

        [TestMethod]
        public void NetObject_Call_OneArgument()
        {
            string expected = "some string";
            var ok = new FieldClass { OK = expected };
            var netObj = new NetObject(ok);

            var actual = netObj.Call("Method", new NetObject("hmm"));

            Assert.AreEqual(6, actual.ToObject());
        }
    }
}
