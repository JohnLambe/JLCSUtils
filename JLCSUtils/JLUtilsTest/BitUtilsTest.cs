using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.Math;

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class BitUtilsTest
    {
        [TestMethod]
        public void LowSetBit_32Bit()
        {
            Assert.AreEqual(25, BitUtil.LowSetBit(3 << 25));

            // each bit in a byte:
            Assert.AreEqual(8 + 0, BitUtil.LowSetBit(0x3F << 8));
            Assert.AreEqual(8 + 1, BitUtil.LowSetBit(0x12 << 8));
            Assert.AreEqual(8 + 2, BitUtil.LowSetBit(0x24 << 8));
            Assert.AreEqual(8 + 3, BitUtil.LowSetBit(0x18 << 8));
            Assert.AreEqual(8 + 4, BitUtil.LowSetBit(0xA010 << 8));
            Assert.AreEqual(8 + 5, BitUtil.LowSetBit(0x220 << 8));
            Assert.AreEqual(8 + 6, BitUtil.LowSetBit(0xC0 << 8));
            Assert.AreEqual(8 + 7, BitUtil.LowSetBit(0x380 << 8));

            // one case for each byte:
            Assert.AreEqual(4, BitUtil.LowSetBit(0xF2B501F0));
            Assert.AreEqual(10, BitUtil.LowSetBit(0x34 << 8));
            Assert.AreEqual(16, BitUtil.LowSetBit(0xF10000));
            Assert.AreEqual(28, BitUtil.LowSetBit(0x90000000));

            Assert.AreEqual(0, BitUtil.LowSetBit(0xFFFFFFFF), "All bits set");
            Assert.AreEqual(-1, BitUtil.LowSetBit(0), "Zero value");  // special case

            // each bit:
            for(int index = 0; index < 32; index++)
            {
                Assert.AreEqual(index, BitUtil.LowSetBit( 1u << index), "single bit set: " + index);
            }
        }

        [TestMethod]
        public void ByteLowSetBit()
        {
            // each bit:
            Assert.AreEqual(0, BitUtil.ByteLowSetBit(0xFF));
            Assert.AreEqual(1, BitUtil.ByteLowSetBit(0x52));
            Assert.AreEqual(2, BitUtil.ByteLowSetBit(0x44));
            Assert.AreEqual(3, BitUtil.ByteLowSetBit(0xE8));
            Assert.AreEqual(4, BitUtil.ByteLowSetBit(0x10));
            Assert.AreEqual(5, BitUtil.ByteLowSetBit(0xA0));
            Assert.AreEqual(6, BitUtil.ByteLowSetBit(0x40));
            Assert.AreEqual(7, BitUtil.ByteLowSetBit(0x80));
            // Special case:
            Assert.AreEqual(-1, BitUtil.ByteLowSetBit(0), "Zero value");
        }

        [TestMethod]
        public void AllSet()
        {
            Assert.IsFalse(BitUtil.AllSet(0x12345678,0xF00));
            Assert.IsTrue(BitUtil.AllSet(0x12345678, 0x600));

            Assert.IsTrue(BitUtil.AllSet(0xF2345678, 0x80000000), "uint");
            Assert.IsTrue(BitUtil.AllSet(0xF2345678, 0));

            Assert.IsTrue(BitUtil.AllSet(0xFFFFFFFFF2345678, 0x80000008), "ulong");
        }
    }
}
