using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Text;
using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.Text
{
    [TestClass]
    public class StringCharacterSetTest
    {
        [TestMethod]
        public void Create()
        {
            StringCharacterSet s = new StringCharacterSet();
            s.StringValue = "asdfga";

            Assert.AreEqual(5, s.Count);
            Assert.AreEqual("sdfga", s.StringValue);   // duplicate removed
                    // an implementation could break this
        }

        [TestMethod]
        public void Contains()
        {
            StringCharacterSet s = new StringCharacterSet();
            s.StringValue = "ABCDEFGH--IJK";

            Multiple(
                () => Assert.AreEqual(true, s.Contains('A')),
                () => Assert.AreEqual(false, s.Contains('Z')),
                () => Assert.AreEqual(true, s.Contains('-'), "duplicate")
            );
        }
    }
}
