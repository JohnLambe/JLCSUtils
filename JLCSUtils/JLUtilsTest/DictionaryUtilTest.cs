using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util;
using JohnLambe.Util.Collections;
using System.Collections;

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class DictionaryUtilTest
    {
        [TestMethod]
        public void RemoveAllByKV()
        {
            // Arrange:
            IDictionary d = new Dictionary<int, string>();

            d[10] = "asd";
            d[20] = "10x20";
            d[35] = "123";
            d[12345] = "x";

            // Act:
            d.RemoveAllByKV<int, string>( (k,v) => v.Contains("x") );

            // Assert:
            Assert.AreEqual("asd", d[10], "Wrong value removed");
            Assert.AreEqual("123", d[35], "Wrong value removed (35)");
            Assert.AreEqual(2, d.Count, "Wrong number of items removed");

        }
    }
}
