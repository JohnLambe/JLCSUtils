﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.Text;
using System.Dynamic;
using static JohnLambe.Tests.JLUtilsTest.TestUtil;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace JohnLambe.Tests.JLUtilsTest.Text
{
    [TestClass]
    public class CaptionUtilTest
    {
        [TestMethod]
        public void PascalCaseToCaption()
        {
            Multiple(
                () => Assert.AreEqual("Pascal Cased Name",CaptionUtil.PascalCaseToCaption("PascalCasedName"),"simple case"),
                () => Assert.AreEqual("ATM Machine", CaptionUtil.PascalCaseToCaption("ATMMachine"),"multiple consecutive capitals at start"),
                () => Assert.AreEqual("Get XML Document", CaptionUtil.PascalCaseToCaption("GetXMLDocument"),"multiple consecutive capitals"),

                () => Assert.AreEqual("Get XML Document a", CaptionUtil.PascalCaseToCaption("Get XMLDocument a"), "spaces"),
                () => Assert.AreEqual("Get XML Document B", CaptionUtil.PascalCaseToCaption("Get_XML_Document B"), "underscores and space"),

                () => Assert.AreEqual("é Abcdé Fg Éf", CaptionUtil.PascalCaseToCaption("éAbcdéFgÉf"), "non-ASCII"),

                () => Assert.AreEqual("number 1", CaptionUtil.PascalCaseToCaption("number1"), "digit after lower case"),
                () => Assert.AreEqual("Number 100", CaptionUtil.PascalCaseToCaption("Number100"), "digits after lower case"),
                () => Assert.AreEqual("Number 100 Bus", CaptionUtil.PascalCaseToCaption("Number100Bus"), "Word after digits"),
                //TODO:            Assert.AreEqual("number 123b", CaptionUtils.PascalCaseToCaption("number123b"), "digit after lower case");
                () => Assert.AreEqual("number 1A23B", CaptionUtil.PascalCaseToCaption("number1A23B"), "digit after lower case"),

                //TODO: ?
                //            Assert.AreEqual("RTÉ 1", CaptionUtils.PascalCaseToCaption("RTÉ1"), "digit after capital");

                () => Assert.AreEqual(null, CaptionUtil.PascalCaseToCaption(null), "null")
            );
        }

        [TestMethod]
        public void GetDisplayName_Property()
        {
            Multiple(
                () => Assert.AreEqual("Test Property 2", CaptionUtil.GetDisplayName(GetType().GetProperty("TestProperty2")), "from property name"),
                () => Assert.AreEqual("Test Property 1", CaptionUtil.GetDisplayName(GetType().GetProperty("TestProperty1")), "has DescriptionAttribute"),
                () => Assert.AreEqual(null, CaptionUtil.GetDisplayName((System.Reflection.PropertyInfo)null), "null")
            );
        }

        [TestMethod]
        public void GetDescriptionFromAttribute_Property()
        {
            Multiple(
                () => Assert.AreEqual("Description of TestProperty1", CaptionUtil.GetDescriptionFromAttribute(GetType().GetProperty("TestProperty1")), "from DescriptionAttribute"),
                () => Assert.AreEqual(null, CaptionUtil.GetDescriptionFromAttribute(GetType().GetProperty("TestProperty2")), "no attribute")
            );
        }

        [TestMethod]
        public void GetDisplayName_Class()
        {
            Multiple(
                () => Assert.AreEqual("test display name", CaptionUtil.GetDisplayName(typeof(TestClass)), "from attribute on class"),
                () => Assert.AreEqual("test display name", CaptionUtil.GetDisplayName(typeof(TestSubClass)), "from attribute on base class"),
                () => Assert.AreEqual("Caption Util Test", CaptionUtil.GetDisplayName(GetType()), "from class name")
            );
        }


        #region GetDisplayNameForObject

        [TestMethod]
        public void GetDisplayNameForObject_NoProperty()
        {
            Assert.AreEqual(ToString(),CaptionUtil.GetDisplayNameForObject(this));
        }

        [TestMethod]
        public void GetDisplayNameForObject_Description()
        {
            var testObject = new TestClass()
            {
                Description = "the description"
            };
            Assert.AreEqual("the description", CaptionUtil.GetDisplayNameForObject(testObject));
        }

        [TestMethod]
        public void GetDisplayNameForObject_Name()
        {
            TestUtil.Multiple(
                () =>
                {
                    var testObject = new TestClassWithName()
                    {
                        Name = 10
                    };
                    Assert.AreEqual("10", CaptionUtil.GetDisplayNameForObject(testObject));
                },
                () =>
                {
                    var testObject1 = new TestClassWithName();
                    Assert.AreEqual("Description", CaptionUtil.GetDisplayNameForObject(testObject1));
                }
            );
        }


        /* TODO: GetDisplayNameForObject doesn't support dynamic properties yet.
        [TestMethod]
        public void GetDisplayNameForObject_Dynamic()
        {
            // Arrange:
            dynamic x = new ExpandoObject();
            x.Name = "name";

            // Act/Assert:
            Assert.AreEqual("name", x);
        }
        */

        #endregion

        [DisplayName("test display name")]
        public class TestClass
        {
            public string Description { get; set; }
        }

        public class TestSubClass : TestClass
        {
        }

        [System.ComponentModel.Description("Description of TestProperty1")]
        public string TestProperty1 { get; set; }

        public virtual int TestProperty2 { get; }


        public class TestClassWithName
        {
            public int? Name { get; set; }
            public string Description => "Description";
        }

    }
}
