using JohnLambe.Util.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.Collections
{
    [TestClass]
    public class EnumerationUtilsTest
    {
        [TestMethod]
        public void Count()
        {
            Assert.AreEqual(10, EnumeratorUtils.Count("0123456789".GetEnumerator()));
            Assert.AreEqual(0, EnumeratorUtils.Count(new LinkedList<int>().GetEnumerator()), "empty list");
            Assert.AreEqual(0, EnumeratorUtils.Count<string>(null), "null");
        }
    }
}
