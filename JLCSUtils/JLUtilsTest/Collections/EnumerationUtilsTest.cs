using JohnLambe.Util.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;


namespace JohnLambe.Tests.JLUtilsTest.Collections
{
    [TestClass]
    public class EnumerationUtilsTest
    {
        [TestMethod]
        public void Count()
        {
            Multiple(
                () => Assert.AreEqual(10, EnumeratorUtil.Count("0123456789".GetEnumerator())),
                () => Assert.AreEqual(0, EnumeratorUtil.Count(new LinkedList<int>().GetEnumerator()), "empty list"),
                () => Assert.AreEqual(0, EnumeratorUtil.Count<string>(null), "null")
            );
        }
    }
}
