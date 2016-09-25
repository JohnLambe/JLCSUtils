using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.Text;

namespace JohnLambe.Tests.JLUtilsTest.Text
{
    [TestClass]
    public class CaptionUtilsTest
    {
        [TestMethod]
        public void PascalCaseToCaption()
        {
            Assert.AreEqual("Pascal Cased Name",CaptionUtils.PascalCaseToCaption("PascalCasedName"),"simple case");
            Assert.AreEqual("ATM Machine", CaptionUtils.PascalCaseToCaption("ATMMachine"),"multiple consecutive capitals at start");
            Assert.AreEqual("Get XML Document", CaptionUtils.PascalCaseToCaption("GetXMLDocument"),"multiple consecutive capitals");

            Assert.AreEqual("Get XML Document a", CaptionUtils.PascalCaseToCaption("Get XMLDocument a"), "spaces");
            Assert.AreEqual("Get XML Document B", CaptionUtils.PascalCaseToCaption("Get_XML_Document B"), "underscores and space");

            Assert.AreEqual("é Abcdé Fg Éf", CaptionUtils.PascalCaseToCaption("éAbcdéFgÉf"), "non-ASCII");

            Assert.AreEqual("number 1", CaptionUtils.PascalCaseToCaption("number1"), "digit after lower case");
            Assert.AreEqual("Number 100", CaptionUtils.PascalCaseToCaption("Number100"), "digits after lower case");
            Assert.AreEqual("Number 100 Bus", CaptionUtils.PascalCaseToCaption("Number100Bus"), "Word after digits");
            //TODO:            Assert.AreEqual("number 123b", CaptionUtils.PascalCaseToCaption("number123b"), "digit after lower case");
            Assert.AreEqual("number 1A23B", CaptionUtils.PascalCaseToCaption("number1A23B"), "digit after lower case");

            //TODO: ?
            //            Assert.AreEqual("RTÉ 1", CaptionUtils.PascalCaseToCaption("RTÉ1"), "digit after capital");

            Assert.AreEqual(null, CaptionUtils.PascalCaseToCaption(null), "null");
        }
    }
}
