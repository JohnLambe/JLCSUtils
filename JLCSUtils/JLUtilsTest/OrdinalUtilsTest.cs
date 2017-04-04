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
            Assert.AreEqual("423rd", OrdinalUtil.Ordinal(423));
            Assert.AreEqual("11th", OrdinalUtil.Ordinal(11));
            Assert.AreEqual("91st", OrdinalUtil.Ordinal(91));
            Assert.AreEqual("0th", OrdinalUtil.Ordinal(0));
            Assert.AreEqual("-11th", OrdinalUtil.Ordinal(-11));
//            Assert.AreEqual("-2nd", OrdinalUtils.Ordinal(-2));
        }
    }
}
