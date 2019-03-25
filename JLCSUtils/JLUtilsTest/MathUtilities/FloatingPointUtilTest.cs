using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.MathUtilities;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.MathUtilities
{
    [TestClass]
    public class FloatingPointUtilTest
    {
        [TestMethod]
        public void NumberOfDigits()
        {
            Multiple(
                () => Assert.AreEqual(3, FloatingPointUtil.NumberOfDigits(123)),
                () => Assert.AreEqual(5, FloatingPointUtil.NumberOfDigits(54321.012m)),
                () => Assert.AreEqual(7, FloatingPointUtil.NumberOfDigits(0xABCDEF0,16)),
                () => Assert.AreEqual(1, FloatingPointUtil.NumberOfDigits(0, 8)),
                () => Assert.AreEqual(2, FloatingPointUtil.NumberOfDigits(-15)),
                () => Assert.AreEqual(15, FloatingPointUtil.NumberOfDigits(100000000000000m)),
                () => Assert.AreEqual(14, FloatingPointUtil.NumberOfDigits(100000000000000m-1)),
                () => Assert.AreEqual(28, FloatingPointUtil.NumberOfDigits(9999999999999999999999999999m)),
                () => Assert.AreEqual(29, FloatingPointUtil.NumberOfDigits(decimal.MaxValue))
            );
        }
    }
}
