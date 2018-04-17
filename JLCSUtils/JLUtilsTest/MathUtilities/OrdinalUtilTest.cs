using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.MathUtilities;

namespace JohnLambe.Tests.JLUtilsTest.MathUtilities
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
            TestUtil.Multiple(
                () => Assert.AreEqual("0th", OrdinalUtil.Ordinal(0)),
                () => Assert.AreEqual("1st", OrdinalUtil.Ordinal(1)),
                () => Assert.AreEqual("932nd", OrdinalUtil.Ordinal(932)),
                () => Assert.AreEqual("11th", OrdinalUtil.Ordinal(11)),
                () => Assert.AreEqual("1003rd", OrdinalUtil.Ordinal(1003)),
                () => Assert.AreEqual("-2nd", OrdinalUtil.Ordinal(-2)),

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
            TestUtil.Multiple(
                () => Assert.AreEqual("th", OrdinalUtil.OrdinalSuffix(0)),
                () => Assert.AreEqual("st", OrdinalUtil.OrdinalSuffix(21)),
                () => Assert.AreEqual("nd", OrdinalUtil.OrdinalSuffix(2)),
                () => Assert.AreEqual("th", OrdinalUtil.OrdinalSuffix(12)),
                () => Assert.AreEqual("rd", OrdinalUtil.OrdinalSuffix(106753003)),
                () => Assert.AreEqual("nd", OrdinalUtil.OrdinalSuffix(-902)),

                () => Assert.AreEqual("st", OrdinalUtil.OrdinalSuffix(1101)),
                () => Assert.AreEqual("th", OrdinalUtil.OrdinalSuffix(111))
            );
        }
    }
}
