using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Validation;
using System.ComponentModel.DataAnnotations;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.Validation
{
    [TestClass]
    public class NumberValidationAttributeTest
    {
        [TestMethod]
        public void IsValid_MinimumValue()
        {
            var attrib = new NumberValidationAttribute()
            {
                MinimumValue = 2000
            };

            Multiple(
                () => Assert.IsFalse(attrib.IsValid(500)),
                () => Assert.IsTrue(attrib.IsValid(2000)),
                () => Assert.IsTrue(attrib.IsValid(3000))
            );
        }

        [TestMethod]
        public void IsValid_MaximumValue()
        {
            var attrib = new NumberValidationAttribute()
            {
                MaximumValue = -300
            };

            Multiple(
                () => Assert.IsFalse(attrib.IsValid(-100.5)),
                () => Assert.IsFalse(attrib.IsValid(2)),
                () => Assert.IsTrue(attrib.IsValid(-300)),
                () => Assert.IsTrue(attrib.IsValid(-800.2))
            );
        }

        [TestMethod]
        public void IsValid_LessThan()
        {
            var attrib = new NumberValidationAttribute()
            {
                LessThan = -1000
            };

            Multiple(
                () => Assert.IsFalse(attrib.IsValid(-1000)),
                () => Assert.IsTrue(attrib.IsValid(-1000.1)),
                () => Assert.IsFalse(attrib.IsValid(2000))
            );
        }

        [TestMethod]
        public void IsValid_GreaterThan()
        {
            var attrib = new NumberValidationAttribute()
            {
                GreaterThan = 8.5
            };

            Multiple(
                () => Assert.IsFalse(attrib.IsValid(8.5)),
                () => Assert.IsTrue(attrib.IsValid(8.500001)),
                () => Assert.IsFalse(attrib.IsValid(8.499999))
            );
        }

        [TestCategory("Failing")]
        [TestMethod]
        public void IsValid_RoundTo()
        {
            var attrib = new NumberValidationAttribute()
            {
                RoundTo = 3
            };

            Multiple(
                () =>
                {
                    var value = 5.1234567;
                    Assert.IsTrue(ValidatorEx.Instance.TryValidateValue(ref value, attrib));
                    Assert.AreEqual(5.123, value);
                },
                () =>
                {
                    var value = -32.787878;
                    Assert.IsTrue(ValidatorEx.Instance.TryValidateValue(ref value, attrib));
                    Assert.AreEqual(-32.788, value, "Should round away from 0");
                }
            );
        }

        [TestMethod]
        public void IsValid()
        {
            var attrib = new NumberValidationAttribute()
            {
                MinimumValue = 80.4,
                MaximumValue = 80.92
            };

            Multiple(
                () => Assert.IsFalse(attrib.IsValid(80.39)),
                () => Assert.IsTrue(attrib.IsValid(80.4)),
                () => Assert.IsTrue(attrib.IsValid(80.5)),
                () => Assert.IsFalse(attrib.IsValid(80.93))
            );
        }

    }
}
