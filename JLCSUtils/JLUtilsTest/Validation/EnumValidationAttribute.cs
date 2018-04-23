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
        [ExpectedException(typeof(ValidationException))]
        public void ValidateObject_Flags_Invalid()
        {
            var obj = new TestClassFlags() { Attrib = (System.IO.FileAttributes)0xA0000 };
            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateObject_WrongDeclaredType()
        {
            var obj = new TestClassNonEnum();
            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        [TestMethod]
        public void ValidateObject_Object_Valid()
        {
            var obj = new TestClassObject() { Value = DayOfWeek.Sunday };
            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        /// <summary>
        /// null is valid.
        /// </summary>
        [TestMethod]
        public void ValidateObject_Object_Null()
        {
            var obj = new TestClassObject() { Value = null };
            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateObject_Object_Invalid()
        {
            var obj = new TestClassObject() { Value = 'x' };
            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateObject_Object_Nullable_Invalid()
        {
            var obj = new TestClassNullable() { Value = System.IO.FileAttributes.Hidden | (System.IO.FileAttributes)0x1000000 };
            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        [TestMethod]
        public void ValidateObject_Object_Nullable_Null()
        {
            var obj = new TestClassNullable() {  };
            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        [TestMethod]
        public void ValidateObject_Object_Nullable_Valid()
        {
            var obj = new TestClassNullable() { Value = System.IO.FileAttributes.Directory | System.IO.FileAttributes.Compressed | System.IO.FileAttributes.Archive };
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

        public class TestClassNonEnum
        {
            [EnumValidation]
            public int Value { get; set; }
        }

        public class TestClassObject
        {
            [EnumValidation]
            public virtual object Value { get; set; }
        }

        public class TestClassNullable
        {
            [EnumValidation]
            public System.IO.FileAttributes? Value { get; set; }
        }
    }
}
