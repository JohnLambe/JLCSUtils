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
            Assert.AreEqual("a, 123, , False", CollectionUtils.CollectionToString(new object[] { "a", 123, null, false }));
            Assert.AreEqual(null, CollectionUtils.CollectionToString<int>(null));
            Assert.AreEqual("", CollectionUtils.CollectionToString<float>(new float[] { }));
            Assert.AreEqual("1;;4.7;-23;9.00", CollectionUtils.CollectionToString(new decimal?[] { 1, null, 4.7m, -23m, 9.00m }, ";"));
        }

    }
}
