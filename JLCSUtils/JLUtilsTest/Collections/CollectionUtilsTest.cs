using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.Collections;

namespace JohnLambe.Tests.JLUtilsTest.Collections
{
    [TestClass]
    public class CollectionUtilsTest
    {
        [TestMethod]
        public void CollectionToStringTest()
        {
            Assert.AreEqual("a, 123, , False", CollectionUtil.CollectionToString(new object[] { "a", 123, null, false }));
            Assert.AreEqual(null, CollectionUtil.CollectionToString<int>(null));
            Assert.AreEqual("", CollectionUtil.CollectionToString<float>(new float[] { }));
            Assert.AreEqual("1;;4.7;-23;9.00", CollectionUtil.CollectionToString(new decimal?[] { 1, null, 4.7m, -23m, 9.00m }, ";"));
        }

    }
}
