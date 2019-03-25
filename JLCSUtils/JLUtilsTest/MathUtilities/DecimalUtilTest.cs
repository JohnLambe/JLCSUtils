using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.MathUtilities;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.MathUtilities
{
    [TestClass]
    public class DecimalUtilTest
    {
        [TestMethod]
        public void ToFloatingPointString()
        {
            Multiple(
                () => Assert.AreEqual("12500.02", DecimalUtil.ToFloatingPointString(12500.0200m)),
                () => Assert.AreEqual("123", DecimalUtil.ToFloatingPointString(123m)),
                () => Assert.AreEqual(decimal.MaxValue.ToString(), DecimalUtil.ToFloatingPointString(decimal.MaxValue)),
                () => Assert.AreEqual("0.001", DecimalUtil.ToFloatingPointString(0.001m)),
                () => Assert.AreEqual("0", DecimalUtil.ToFloatingPointString(0.0000m)),
                () => Assert.AreEqual("100", DecimalUtil.ToFloatingPointString(100.00m)),
                () => Assert.AreEqual("0.1234567890123456789012345678", DecimalUtil.ToFloatingPointString(0.1234567890123456789012345678m))
            );
        }

        [TestMethod]
        public void GetExponent()
        {
            Multiple(
                () => Assert.AreEqual(0, DecimalUtil.GetExponent(123m)),
                () => Assert.AreEqual(0, DecimalUtil.GetExponent(0m)),
                () => Assert.AreEqual(0, DecimalUtil.GetExponent(decimal.MaxValue)),
                () => Assert.AreEqual(0, DecimalUtil.GetExponent(decimal.MinValue)),
                () => Assert.AreEqual(3, DecimalUtil.GetExponent(0.001m)),
                () => Assert.AreEqual(4, DecimalUtil.GetExponent(0.0000m)),
                () => Assert.AreEqual(28, DecimalUtil.GetExponent(0.1234567890123456789012345678m)),
                () => Assert.AreEqual(6, DecimalUtil.GetExponent(15000.253781m))
            );
        }

    }
}
