using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.Xml;
using System.Xml;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;


namespace JohnLambe.Tests.JLUtilsTest.Xml
{
    [TestClass]
    public class XmlUtilTest
    {
        public XmlUtilTest()
        {
            _document.LoadXml("<?xml version='1.0'?><root>"
                + "<a>"
                + "<intValue>01234</intValue>"
                + "<floatValue>-45.432</floatValue>"
                + "</a>"
                + "</root>");
        }

        #region GetXpathParent

        [TestCategory("NotImplemented")]
        [TestMethod]
        public void GetXpathParent()
        {
            string leaf;

            Multiple(
                () =>
                {
                    Assert.AreEqual("a/b/c/d", XmlUtil.GetXpathParent("a/b/c/d/e", out leaf));
                    Assert.AreEqual("e", leaf);
                },

                () => Assert.AreEqual("a/b/c/d", XmlUtil.GetXpathParent("a/b/c/d/e")),

                () =>
                {
                    Assert.AreEqual("", XmlUtil.GetXpathParent("onelevel", out leaf));
                    Assert.AreEqual("onelevel", leaf);
                },

                () =>
                {
                    Assert.AreEqual("root/level1//two//three", XmlUtil.GetXpathParent("root/level1//two//three/leaf", out leaf));
                    Assert.AreEqual("leaf", leaf);
                }
            );
        }

        [TestCategory("NotImplemented")]
        [TestMethod]
        public void GetXpathParent_Fail()
        {
            TestUtil.AssertThrows<ArgumentException>(() => XmlUtil.GetXpathParent("root//leaf"));
        }

        [TestMethod]
        public void GetXpathParent_NoParent()
        {
            string leaf;
            Assert.AreEqual(null, XmlUtil.GetXpathParent("", out leaf));
            Assert.AreEqual(leaf, "");
        }

        [TestCategory("NotImplemented")]
        [TestMethod]
        public void GetXpathParent_Null()
        {
            //            TestUtil.AssertThrows<ArgumentNullException>(() => XmlUtil.GetXpathParent(null));
            string leaf;
            Assert.AreEqual(null, XmlUtil.GetXpathParent(null, out leaf));
            Assert.AreEqual(leaf, "");
        }

        #endregion

        #region GetValue

        [TestMethod]
        public void GetValue()
        {
            // Act:
            int x = _document.SelectSingleNode("root/a/intValue").GetValue<int>();

            // Assert:
            Assert.AreEqual(1234, x);
        }

        #endregion

        #region GetSubNodeValue

        [TestMethod]
        public void GetSubNodeValue()
        {
            Multiple(
                () => Assert.AreEqual(1234, _document.GetSubNodeValueDefault<int>("root/a/intValue")),
                () => Assert.AreEqual(-45.432m, _document.GetSubNodeValueDefault<decimal>("root/a/floatValue")),

                () => Assert.AreEqual(1234m, _document.GetSubNodeValueDefault<decimal>("root/a/intValue")),
                () => Assert.AreEqual("01234", _document.GetSubNodeValueDefault<string>("root/a/intValue"))
            );
        }

        [TestMethod]
        public void GetSubNodeValue_Null()
        {
            Assert.AreEqual(5, XmlUtil.GetSubNodeValueDefault<int>(null,"",5));
        }

        [TestMethod]
        public void GetSubNodeValue_Default()
        {
            Multiple(
                () => Assert.AreEqual(85.4m, _document.GetSubNodeValueDefault<decimal>("root/notfound/floatValue", 85.4m), "default given"),
                () => Assert.AreEqual(null, _document.GetSubNodeValueDefault<double?>("root/notfound/floatValue"), "nullable"),
                () => Assert.AreEqual(0, _document.GetSubNodeValueDefault<int>("root/notfound/floatValue"))
                );
        }

        #endregion

        #region SetSubNodeValue

        /*
        [TestMethod]
        public void SetValue()
        {
            // Act:
            _document.SelectSingleNode("root/a/floatValue").SetValue(200);

            Console.Out.WriteLine(_document.OuterXml);

            // Assert:
            Assert.AreEqual(8, _document.GetSubNodeValue<int>("root/a/floatValue"));
            Assert.AreEqual('x', _document.GetSubNodeValue<char?>("root/a/intValue"));
        }
        */

        [TestMethod]
        public void SetSubNodeValue()
        {
            // Act:
            _document.SetSubNodeValue<int>("root/a/floatValue", 8);
            _document.SetSubNodeValue<char?>("root/a/intValue", 'x');

            Console.Out.WriteLine(_document.OuterXml);

            // Assert:
            Assert.AreEqual(8, _document.GetSubNodeValue<int?>("root/a/floatValue"));
            Assert.AreEqual('x', _document.GetSubNodeValue<char?>("root/a/intValue"));
        }

        [TestMethod]
        public void SetSubNodeValue_ToNull()
        {
            _document.SetSubNodeValue<double?>("root/a/floatValue", null);
            Console.Out.WriteLine(_document.OuterXml);

            // Assert:
            Assert.AreEqual("", _document.GetSubNodeValue<string>("root/a/floatValue"));
            Assert.AreEqual(null, _document.GetSubNodeValue<int?>("root/a/floatValue"));
        }

        #endregion

        protected XmlDocument _document = new XmlDocument();
    }
}
