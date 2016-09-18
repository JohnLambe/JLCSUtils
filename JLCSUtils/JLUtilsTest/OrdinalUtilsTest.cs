using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util;

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class OrdinalUtilsTest
    {
        [TestMethod]
        public void Ordinal()
        {
            Assert.AreEqual("423rd", OrdinalUtils.Ordinal(423));
            Assert.AreEqual("11th", OrdinalUtils.Ordinal(11));
            Assert.AreEqual("91st", OrdinalUtils.Ordinal(91));
            Assert.AreEqual("0th", OrdinalUtils.Ordinal(0));
            Assert.AreEqual("-11th", OrdinalUtils.Ordinal(-11));
//            Assert.AreEqual("-2nd", OrdinalUtils.Ordinal(-2));
        }
    }
}
