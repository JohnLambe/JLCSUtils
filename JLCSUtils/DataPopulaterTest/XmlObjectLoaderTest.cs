using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.DataPopulater;
using JohnLambe.Util.Xml;

namespace JohnLambe.Test.DataPopulater
{
    [TestClass]
    public class XmlObjectLoaderTest
    {
        public XmlObjectLoaderTest()
        {
            _loader.Namespace = "JohnLambe.DataPopulater";
        }

        [TestMethod]
        public void Parse()
        {
            var result = _loader.Parse(XmlUtil.LoadFromFile(@"C:\Dev\Microsoft\DataPop\Config.xml"));

            Console.Out.WriteLine(result);
        }

        protected XmlObjectLoader _loader = new XmlObjectLoader();
    }

    public enum TestEnum
    {
        Option1,
        Option2,
        Option3,
    }
}
