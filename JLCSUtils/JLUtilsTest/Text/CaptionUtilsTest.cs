﻿using System;
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
            Assert.AreEqual("Pascal Cased Name",CaptionUtil.PascalCaseToCaption("PascalCasedName"),"simple case");
            Assert.AreEqual("ATM Machine", CaptionUtil.PascalCaseToCaption("ATMMachine"),"multiple consecutive capitals at start");
            Assert.AreEqual("Get XML Document", CaptionUtil.PascalCaseToCaption("GetXMLDocument"),"multiple consecutive capitals");

            Assert.AreEqual("Get XML Document a", CaptionUtil.PascalCaseToCaption("Get XMLDocument a"), "spaces");
            Assert.AreEqual("Get XML Document B", CaptionUtil.PascalCaseToCaption("Get_XML_Document B"), "underscores and space");

            Assert.AreEqual("é Abcdé Fg Éf", CaptionUtil.PascalCaseToCaption("éAbcdéFgÉf"), "non-ASCII");

            Assert.AreEqual("number 1", CaptionUtil.PascalCaseToCaption("number1"), "digit after lower case");
            Assert.AreEqual("Number 100", CaptionUtil.PascalCaseToCaption("Number100"), "digits after lower case");
            Assert.AreEqual("Number 100 Bus", CaptionUtil.PascalCaseToCaption("Number100Bus"), "Word after digits");
            //TODO:            Assert.AreEqual("number 123b", CaptionUtils.PascalCaseToCaption("number123b"), "digit after lower case");
            Assert.AreEqual("number 1A23B", CaptionUtil.PascalCaseToCaption("number1A23B"), "digit after lower case");

            //TODO: ?
            //            Assert.AreEqual("RTÉ 1", CaptionUtils.PascalCaseToCaption("RTÉ1"), "digit after capital");

            Assert.AreEqual(null, CaptionUtil.PascalCaseToCaption(null), "null");
        }

        [TestMethod]
        public void GetDisplayName_Property()
        {
            Assert.AreEqual("Test Property 2", CaptionUtil.GetDisplayName(GetType().GetProperty("TestProperty2")), "from property name");
            Assert.AreEqual("Test Property 1", CaptionUtil.GetDisplayName(GetType().GetProperty("TestProperty1")), "has DescriptionAttribute");
            Assert.AreEqual(null, CaptionUtil.GetDisplayName((System.Reflection.PropertyInfo)null), "null");
        }

        [TestMethod]
        public void GetDescriptionFromAttribute_Property()
        {
            Assert.AreEqual("Description of TestProperty1", CaptionUtil.GetDescriptionFromAttribute(GetType().GetProperty("TestProperty1")), "from DescriptionAttribute");
            Assert.AreEqual(null, CaptionUtil.GetDescriptionFromAttribute(GetType().GetProperty("TestProperty2")), "no attribute");
        }

        [System.ComponentModel.Description("Description of TestProperty1")]
        public string TestProperty1 { get; set; }

        public virtual int TestProperty2 { get; }
    }
}
