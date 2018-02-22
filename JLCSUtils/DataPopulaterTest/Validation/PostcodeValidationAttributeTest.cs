using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Validation;

namespace JohnLambe.Tests.JLUtilsTest.Validation
{
    [TestClass]
    public class EircodeValidationAttributeTest
    {
        [TestMethod]
        public void ValidateEircode_Valid_Reformat()
        {
            string value = " d12f1C8 ";
            Assert.IsTrue(EircodeValidationAttribute.ValidateEircode(ref value));
            Assert.AreEqual("D12 F1C8", value);
        }

        [TestMethod]
        public void ValidateEircode_Valid()
        {
            const string initialValue = "D01 F5P2";
            string value = initialValue;
            Assert.IsTrue(EircodeValidationAttribute.ValidateEircode(ref value));
            Assert.AreEqual(initialValue, value);
        }

        [TestMethod]
        public void ValidateEircode_Invalid_WrongSeparator()
        {
            ValidateFail("D12-F1C8");
        }

        [TestMethod]
        public void ValidateEircode_Invalid_WrongLength()
        {
            ValidateFail("D01 F1P25");
        }

        [TestMethod]
        public void ValidateEircode_Invalid()
        {
            ValidateFail("D01F 1P2");   // space in wrong place
        }

        public void ValidateFail(string initialValue)
        {
            string value = initialValue;
            Assert.IsFalse(EircodeValidationAttribute.ValidateEircode(ref value));
            Assert.AreEqual(initialValue, value, "Value should be unmodified");
        }
    }
}
