using System;
using JohnLambe.Util.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnLambe.Tests.JLUtilsTest.Text
{
    [TestClass]
    public class FormatUtilTest
    {
        [TestMethod]
        public void FormatObject()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual("10", FormatUtil.FormatObject(null, 10, null)),
                () => Assert.AreEqual("A2", FormatUtil.FormatObject("X2", 0xA2, null)),
                () => Assert.AreEqual("A2", FormatUtil.FormatObject("X2", (byte)0xA2, null))
                );
        }
    }
}
