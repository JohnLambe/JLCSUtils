using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Encoding;

namespace JohnLambe.Tests.JLUtilsTest.Encoding
{
    [TestClass]
    public class EncodingExtensionTest
    {
        #region GetChar

        [TestMethod]
        public void GetChar_Ascii()
        {
            System.Text.Encoding e = global::System.Text.Encoding.UTF8;
            Assert.AreEqual('A', e.GetChar(65));

            e = global::System.Text.Encoding.ASCII;
            Assert.AreEqual('\t', e.GetChar(9));
        }

        [TestMethod]
        public void GetChar()
        {
            System.Text.Encoding e = global::System.Text.Encoding.GetEncoding(437);
            /*
             * The .NET encoder does not do the following. It interprets the value as its ASCII control code.
            Assert.AreEqual((char)'\u2660', e.GetChar(6));
            */

            Assert.AreEqual((char)'\u2567', e.GetChar(207));
        }

        #endregion

        #region GetByte

        [TestMethod]
        public void GetByte_Ascii()
        {
            System.Text.Encoding e = global::System.Text.Encoding.GetEncoding(1252);
            Assert.AreEqual(126, e.GetByte('~'));
        }

        [TestMethod]
        public void GetByte()
        {
            System.Text.Encoding e = global::System.Text.Encoding.GetEncoding(1252);
            Assert.AreEqual(0xE0, e.GetByte('à'));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetByte_Multiple()
        {
            System.Text.Encoding e = global::System.Text.Encoding.UTF8;
            Console.WriteLine(e.GetByte('é'));
        }

        #endregion
    }
}
