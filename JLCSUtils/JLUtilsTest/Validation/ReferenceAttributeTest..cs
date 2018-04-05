using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;
using JohnLambe.Util.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnLambe.Tests.JLUtilsTest.Validation
{
    [TestClass]
    public class ReferenceAttributeTest
    {
        /// <summary>
        /// The value is invalid according to the source attribute.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateObject_Invalid()
        {
            var obj = new Referrer() { Property2 = 50 };

            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateObject_ValidationAttributeBase_Invalid()
        {
            var obj = new Referrer() { Phone = "invalid", Property2 = 110 };

            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        [TestMethod]
        public void ValidateObject_ValidationAttributeBase_Valid()
        {
            var obj = new Referrer() { Phone = "01-1234567", Property2 = 120 };

            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        [TestMethod]
        public void ValidateObject_Valid()
        {
            var obj = new Referrer() { Property2 = 150 };

            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }

        /// <summary>
        /// The property specified in the attribute is invalid.
        /// Should evaluate as invalid.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateObject_InvalidReference()
        {
            var obj = new Referrer2() { Property2 = 150 };

            Validator.ValidateObject(obj, new ValidationContext(obj), true);
        }


        public class Source
        {
            [Range(100,200)]
            public int Property { get; set; }

            [PhoneNumberValidation]
            public virtual string Phone { get; set; }
        }

        public class Referrer
        {
            [Reference(typeof(Source),nameof(Source.Property))]
            public int Property2 { get; set; }

            [Reference(typeof(Source), nameof(Source.Phone))]
            public virtual string Phone { get; set; }
        }

        public class Referrer2
        {
            [Reference(typeof(Source), "NonExistantProperty")]
            public int Property2 { get; set; }
        }

        //TODO: Multiple attributes on referenced item.

    }
}
