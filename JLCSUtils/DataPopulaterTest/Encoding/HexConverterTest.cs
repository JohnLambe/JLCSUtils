using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.Encoding;
using JohnLambe.Util;

namespace JohnLambe.Tests.JLUtilsTest.Encoding
{
    [TestClass]
    public class HexConverterTest
    {
        [TestMethod]
        public void ByteArrayToHex()
        {
            var bytes = new byte[] { 0x12, 0x23, 0x45, 0xAB, 0x00, 0xFF };
            Assert.AreEqual("122345AB00FF", HexConverter.ToHex(bytes));

            Assert.AreEqual("00".Repeat(1099), HexConverter.ToHex(new byte[1099]));
        }

        [TestMethod]
        public void ByteArrayToHex_EmptyAndNull()
        {
            Assert.AreEqual("", HexConverter.ToHex(new byte[] { }), "Empty");

            Assert.AreEqual(null, HexConverter.ToHex(null), "null");
        }
    }
}
