﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Validation;
using System.ComponentModel.DataAnnotations;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.Validation.CompareProperty
{
    [TestClass]
    public class ComparePropertyValidationAttributeTest
    {
        [TestMethod]
        public void IsValid_MinimumMaximum()
        {
            var validator = new ValidatorEx(ValidationFeatures.All);
            var instance = new ClassForTest()
            {
                Minimum = 100,
                Maximum = 200
            };

            /*
            instance.Value = -100;
            validator.ValidateObject(instance);
            */

            Multiple(
                () =>
                {
                    instance.Value = 100;
                    validator.ValidateObject(instance);
                },
                () =>
                {
                    instance.Value = 199;
                    validator.ValidateObject(instance);
                },
                () =>
                {
                    instance.Value = 200;
                    TestUtil.AssertThrows<ValidationException>(() => validator.ValidateObject(instance));
                },
                () =>
                {
                    instance.Value = -100;
                    TestUtil.AssertThrows<ValidationException>(() => validator.ValidateObject(instance));
                },
                () =>
                {
                    instance.Value = -100;
                    TestUtil.AssertThrows<ValidationException>(() => validator.ValidateObject(instance));
                },
                () =>
                {
                    instance.Value = -100;
                    TestUtil.AssertThrows<ValidationException>(() => validator.ValidateObject(instance));
                }
            );
        }

        [TestMethod]
        public void IsValid_Object()
        {
            var validator = new ValidatorEx(ValidationFeatures.All);
            var instance = new ClassForTest2();

            Multiple(
                () =>
                {
                    instance.Minimum = 100;
                    instance.Maximum = 100;
                    validator.ValidateObject(instance);
                },
                () =>
                {
                    instance.Minimum = null;
                    instance.Maximum = -1234;
                    validator.ValidateObject(instance);
                },
                () =>
                {
                    instance.Minimum = 1;
                    instance.Maximum = -1;
                    TestUtil.AssertThrows<ValidationException>(() => validator.ValidateObject(instance));
                },
                () =>
                {
                    instance.Minimum = "100";
                    instance.Maximum = 101;
                    TestUtil.AssertThrows<Exception>(() => validator.ValidateObject(instance));    // fails because the types can't be compared
                }
            );
        }


        [TestMethod]
        public void IsValid_Null()
        {
            var validator = new ValidatorEx(ValidationFeatures.All);

            Multiple(
                () =>
                {
                    var instance = new ClassForTest3();
                    instance.X = 100;
                    validator.ValidateObject(instance);
                },
                () =>
                {
                    var instance = new ClassForTest4();
                    validator.ValidateObject(instance);
                }
            );
        }

        /*
        [TestMethod]
        public void IsValid_Options_NonLive()
        {
            var attrib = new DateTimeValidationAttribute()
            {
                Options = TimeValidationOptions.None
            };

            Multiple(
                () => Assert.IsTrue(attrib.IsValid(new DateTime(2010, 10, 4)))  // succeeds because this is not live input
            );
        }
*/

    }

    public class ClassForTest
    {
//        [ComparePropertyValidation(PropertyName = nameof(Minimum), Operator = ComparisonOperator.GreaterThanOrEqual)]
        [ComparePropertyValidation(PropertyName = nameof(Maximum), Operator = ComparisonOperator.LessThan)]
//        [ComparePropertyValidation(PropertyName = nameof(Maximum), Operator = ComparisonOperator.Any)]
//        [NumberValidation(MinimumValue = -1000)]
//        [NumberValidation(MinimumValue = -1200)]
        public int Value { get; set; }

        public int Minimum { get; set; }
        public int Maximum { get; set; }
    }

    public class ClassForTest2
    {
        [ComparePropertyValidation(PropertyName = nameof(Maximum), Operator = ComparisonOperator.LessThanOrEqual)]
        public object Minimum { get; set; }

        [ComparePropertyValidation(PropertyName = nameof(Minimum), Operator = ComparisonOperator.GreaterThanOrEqual)]
        public int Maximum { get; set; }
    }

    public class ClassForTest3
    {
        [ComparePropertyValidation(PropertyName = nameof(Y), Operator = ComparisonOperator.LessThanOrEqual)]
        public int X { get; set; }

        public int? Y { get; set; }
    }

    public class ClassForTest4
    {
        public int X { get; set; }

        [ComparePropertyValidation(PropertyName = nameof(X), Operator = ComparisonOperator.LessThanOrEqual)]
        public int? Y { get; set; }
    }
}
