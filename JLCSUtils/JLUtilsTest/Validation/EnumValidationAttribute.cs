using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;
using JohnLambe.Util.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnLambe.Tests.JLUtilsTest.Validation.EnumValidationeAttributeTest
{
    [TestClass]
    public class EnumValidationeAttributeTest
    {
        /// <summary>
        /// The value is invalid according to the source attribute.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateObject_Invalid()
        {
            var obj = new TestClass() { Day = (DayOfWeek)8 };

            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        [TestMethod]
        public void ValidateObject_Valid()
        {
            var obj = new TestClass() { Day = DayOfWeek.Thursday };

            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        [TestMethod]
        public void ValidateObject_Flags_Valid()
        {
            var obj = new TestClassFlags();
            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        [TestMethod]
        public void ValidateObject_Flags_Invalid()
        {
            var obj = new TestClassFlags() { Attrib = (System.IO.FileAttributes)0xA0000 };
            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        public class TestClass
        {
            [EnumValidation]
            public DayOfWeek Day { get; set; }
        }

        public class TestClassFlags
        {
            [EnumValidation]
            public System.IO.FileAttributes Attrib { get; set; } = System.IO.FileAttributes.System | System.IO.FileAttributes.Device;
        }
    }
}
