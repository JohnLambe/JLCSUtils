using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.Math;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest
{
    /// <summary>
    /// Unit tests for <see cref="OrdinalUtil"/>.
    /// </summary>
    [TestClass]
    public class OrdinalUtilTest
    {
        [TestMethod]
        public void Ordinal()
        {
            Multiple(
                () => Assert.AreEqual("123456782nd", OrdinalUtil.Ordinal(123456782)),
                () => Assert.AreEqual("423rd", OrdinalUtil.Ordinal(423)),
                () => Assert.AreEqual("11th", OrdinalUtil.Ordinal(11)),
                () => Assert.AreEqual("91st", OrdinalUtil.Ordinal(91)),
                () => Assert.AreEqual("0th", OrdinalUtil.Ordinal(0)),
                () => Assert.AreEqual("-11th", OrdinalUtil.Ordinal(-11)),
                () => Assert.AreEqual("-2nd", OrdinalUtil.Ordinal(-2))
            );
        }

        [TestMethod]
        public void OrdinalSuffix()
        {
            Multiple(
                () => Assert.AreEqual("st", OrdinalUtil.OrdinalSuffix(1101)),
                () => Assert.AreEqual("th", OrdinalUtil.OrdinalSuffix(111))
            );
        }
    }
}
