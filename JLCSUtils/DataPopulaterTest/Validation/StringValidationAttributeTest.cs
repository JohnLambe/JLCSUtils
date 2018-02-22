using JohnLambe.Util.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.Validation
{
    [TestClass]
    public class StringValidationAttributeTest
    {
        [TestMethod]
        public void IsValid_Blank()
        {
            var attrib = new StringValidationAttribute()
            {
                MinimumLength = 1
            };

            Multiple(
                () => Assert.IsFalse(attrib.IsValid("")),
                () => Assert.IsTrue(attrib.IsValid("1")),
                () => Assert.IsTrue(attrib.IsValid("ab")),
                () => Assert.IsTrue(attrib.IsValid(" "))
            );
        }

        [TestMethod]
        public void IsValid_MaximumLength()
        {
            var attrib = new StringValidationAttribute()
            {
                MinimumLength = 2,
                MaximumLength = 10
            };

            Multiple(
                () => Assert.IsFalse(attrib.IsValid("")),
                () => Assert.IsFalse(attrib.IsValid("1")),
                () => Assert.IsTrue(attrib.IsValid("ab")),
                () => Assert.IsFalse(attrib.IsValid(" ")),
                () => Assert.IsTrue(attrib.IsValid("  ")),  // minimum length
                () => Assert.IsTrue(attrib.IsValid("12345")),
                () => Assert.IsTrue(attrib.IsValid("1234567890")),
                () => Assert.IsFalse(attrib.IsValid("1234567890a"))
            );
        }

        [TestMethod]
        [TestCategory("Failing")]
        public void IsValid_Truncate()
        {
            var attrib = new StringValidationAttribute()
            {
                MinimumLength = 2,
                MaximumLength = 10,
                Truncate = true
            };

            Multiple(
                () => Assert.IsFalse(attrib.IsValid("")),
                () => Assert.IsFalse(attrib.IsValid("1")),
                () => Assert.IsTrue(attrib.IsValid("ab")),  // minimum length
                () => Assert.IsTrue(attrib.IsValid("  ")),  // minimum length (check that it doesn't trim spaces)
                () => Assert.IsFalse(attrib.IsValid(" ")),  // too short
                () => Assert.IsTrue(attrib.IsValid("12345")),
                () => Assert.IsTrue(attrib.IsValid("1234567890")),
                () => Assert.IsTrue(attrib.IsValid("123456789012")),  // succeeds because it truncates
                () =>
                {
                    var value = "1234567890a";
                    Assert.IsTrue(ValidatorEx.Instance.TryValidateValue(ref value, attrib));
                    Assert.AreEqual("1234567890",value);
                }
            );
        }

        [TestMethod]
        [TestCategory("Failing")]
        public void IsValid_Capitalisation()
        {
            var attrib = new StringValidationAttribute()
            {
                Capitalisation = Util.Text.LetterCapitalizationOption.AllCapital,
                Correction = false
            };

            Multiple(
                () => Assert.IsTrue(attrib.IsValid(""),"Blank"),
                () => Assert.IsTrue(attrib.IsValid("ALL CAPS + Á")),
                () => Assert.IsFalse(attrib.IsValid("all lowercase é")),
                () => Assert.IsFalse(attrib.IsValid("MiXED CASE")),
                () => Assert.IsTrue(attrib.IsValid(" "))
            );
        }
    }


    [TestClass]
    public class RegexValidationAttributeTest
    {
        [TestMethod]
        public void IsValid()
        {
            var attrib = new RegexValidationAttribute("^([0-9]{3})$")
            {
                AllowBlank = false
            };

            Multiple(
                () => Assert.IsTrue(attrib.IsValid("123"), "Valid"),
                () => Assert.IsFalse(attrib.IsValid("1234")),
                () => Assert.IsFalse(attrib.IsValid("")),
                () => Assert.IsFalse(attrib.IsValid(null))
            );
        }

        [TestMethod]
        public void IsValid_AllowBlank()
        {
            var attrib = new RegexValidationAttribute("^([0-9]{3})$")
            {
                AllowBlank = true
            };

            Multiple(
                () => Assert.IsTrue(attrib.IsValid("000"), "Valid"),
                () => Assert.IsFalse(attrib.IsValid("A")),
                () => Assert.IsFalse(attrib.IsValid("9999")),
                () => Assert.IsFalse(attrib.IsValid("5")),
                () => Assert.IsTrue(attrib.IsValid("")),
                () => Assert.IsTrue(attrib.IsValid(null))
            );
        }

        [TestMethod]
        public void TestValid_Truncate()
        {
            var attrib = new RegexValidationAttribute("^([0-9]{3})$")
            {
                AllowBlank = false,
                Truncate = true,
                MaximumLength = 3
            };
            var context = new ValidationContext(this);
            ValidationContextExtension.SetSupportedFeatures(context, ValidationFeatures.All);

            Multiple(
                () => Assert.IsTrue(attrib.TestValid("123",context), "Valid"),
                () => Assert.IsTrue(attrib.TestValid("1234",context)),
                () => Assert.IsFalse(attrib.TestValid("1",context)),
                () => Assert.IsFalse(attrib.TestValid("",context))
            );
        }
    }
}
