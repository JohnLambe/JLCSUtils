using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util;
using System.Runtime.InteropServices;

// V. 0.1

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class ByteArrayTest
    {

//        [TestMethod]
        public void InvalidIntPtrWrite()
        {
            var p1 = Marshal.AllocHGlobal(2);
            var p2 = Marshal.AllocHGlobal(256);
            Marshal.WriteByte(p1, 103, 123);
        }


        /// <summary>
        /// Test that data is preserved on reallocating.
        /// </summary>
        [TestMethod]
        public void ReallocHGlobal()
        {
            var p1 = Marshal.AllocHGlobal(256);
            var p2 = Marshal.AllocHGlobal(1024);   // hopefully allocated just above b, so that be cannot grow in place.

            var p1Original = p1;

            Marshal.WriteInt32(p1, 0, 1234);
            Assert.AreEqual(1234, Marshal.ReadInt32(p1,0), "Failed before reallocation");
            Console.Out.WriteLine("Data written and read successfully before growing.");

            var p1New = Marshal.ReAllocHGlobal(p1, new IntPtr(1024 * 1024 * 600));

            if (p1New == p1Original)
            {
                Assert.Inconclusive("Same address - memory was not moved. This test is inconclusive.");
            }
            else
            {
                Console.Out.WriteLine("Different - memory was moved");
            }

            Assert.AreEqual(1234, Marshal.ReadInt32(p1New, 0), "Data is different after reallocation");
        }

        [TestMethod]
        public void ReallocHGlobalFail()
        {
            var p1 = Marshal.AllocHGlobal(256);
            var p2 = Marshal.AllocHGlobal(1024);   // hopefully allocated just above b, so that be cannot grow in place.

            var p1Original = p1;

            Marshal.WriteInt32(p1, 0, 1234);
            Assert.AreEqual(1234, Marshal.ReadInt32(p1, 0), "Failed before reallocation");
            Console.Out.WriteLine("Data written and read successfully before growing.");

            p1 = Marshal.ReAllocHGlobal(p1, new IntPtr(1024 * 1024 * 600));

            if (p1 == p1Original)
            {
                Assert.Inconclusive("Same address - memory was not moved. This test is inconclusive.");
            }
            else
            {
                Console.Out.WriteLine("Different - memory was moved");
            }

            Assert.AreEqual(1234, Marshal.ReadInt32(p1, 0), "Data is different after reallocation");
        }
 
       
        /// <summary>
        /// Test that data is preserved on reallocating.
        /// </summary>
        [TestMethod]
        public void ReallocCopyTest()
        {
            var b = new ByteArray(0,256);
            var b2 = new ByteArray(1024);   // hopefully allocated just above b, so that b cannot grow in place.

            var p1 = b.AsIntPtr;    // save address before resizing.

            b.WriteInt32(0, 1234);
            Assert.AreEqual(1234, b.ReadInt32(0), "Failed before reallocation");
            Console.Out.WriteLine("Data written and read successfully before growing.");

            b.AllocatedSize = 1024 * 1024 * 600;

            if(p1 == b.AsIntPtr)
            {
                Assert.Inconclusive("Same address - memory was not moved. This test is inconclusive.");
            }
            else
            {
                Console.Out.WriteLine("Different - memory was moved");
            }

            Assert.AreEqual(1234, b.ReadInt32(0), "Data is different after reallocation");
        }

        [TestMethod]
        public void ReadAndWrite()
        {
            var b = new ByteArray(256,256);
            b.WriteInt64(100, 0x0102030405060708);
            Assert.AreEqual(0x0102030405060708, b.ReadInt64(100));
            Assert.AreEqual(0x01020304, b.ReadInt32(104));
            Assert.AreEqual(0x0102, b.ReadInt16(106));
            Assert.AreEqual(0x01, b.ReadByte(107));

            b.WriteChar8(100, 'x');
            Assert.AreEqual('x', b.ReadChar8(100));

            b.WriteChar(100, 'á');
            Assert.AreEqual('á', b.ReadChar(100));
        }

        [TestMethod]
        public void ReadAndWriteChar()
        {
            using (var b = new ByteArray(0,256))
            {
                b.WriteChar8(127, 'a');
                Assert.AreEqual('a', b.ReadChar8(127));
            }
        }

        [TestMethod]
        public void ReadAndWriteCharUnicode()
        {
            using (var b = new ByteArray(256,256))
            {
                b.WriteChar8(127, 'í');
                Assert.AreEqual((byte)'í', b.ReadChar8(127));
            }
        }

        [TestMethod]
        public void ReadAndWriteString()
        {
            const string TestString = "asdfghjkl";

            using (var b = new ByteArray(0,2000))
            {
                b.WriteTString8(1500,TestString);
//                Assert.AreEqual(TestString, b.ReadTString8(1500));
            }
        }

        [TestMethod]
        public void Append()
        {
            const string TestString = "Xasdfghjkl";

            using (var b = new ByteArray())
            {
                b.AppendBool(false);
                b.AppendChar8('a');
                b.AppendInt32(123456);
                b.AppendUInt16(12345);
                b.AppendTString8(TestString);

                Assert.AreEqual(false, b.CursorReadBool());
                Assert.AreEqual('a', b.CursorReadChar8());
                Assert.AreEqual(123456, b.CursorReadInt32());
                Assert.AreEqual(12345, b.CursorReadUInt16());
                Assert.AreEqual(TestString, b.CursorReadTString8());
            }
        }

        [TestMethod]
        public void CreateSubsetReferenceTest()
        {
            const string TestString = "0123456789abcdefghijklmnopqrstuvwxyz";
            using (var b = new ByteArray(2048))
            {
                b.AppendTString8(TestString);
                var subset = b.CreateReferenceSubset(8,5);

                Assert.AreEqual(TestString.Substring(8,5), subset.ReadTString8(0));

                subset.Dispose();
            }

        }

        [TestMethod]
        public void Equals()
        {
            const string TestString = "Xasdfghjkl0123456789ABCDEF-qwertyuiop";

            using (ByteArray b = new ByteArray(), b2 = new ByteArray(4096))
            {
                b.AppendTString8(TestString);
                b2.AppendTString8(TestString);

                Assert.IsTrue(b.Equals(b2));
                Assert.AreEqual(b, b2);

                b2.WriteByte(5, 64);

                Assert.AreNotEqual(b, b2);
            }
        }

    }
}
