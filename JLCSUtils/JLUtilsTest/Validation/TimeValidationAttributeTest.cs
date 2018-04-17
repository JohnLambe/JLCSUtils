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
    public class TimeValidationAttributeTest
    {
        [TestMethod]
        public void IsValid_MinimumMaximum()
        {
            var attrib = new DateTimeValidationAttribute()
            {
                Minimum = new DateTime(2010,10,5),
                Maximum = new DateTime(2020,1,20),
                Options = TimeValidationOptions.None
            };

            Multiple(
                () => Assert.IsFalse(attrib.IsValid(new DateTime(2010, 10, 4))),
                () => Assert.IsTrue(attrib.IsValid(new DateTime(2010, 10, 5))),
                () => Assert.IsTrue(attrib.IsValid(new DateTime(2010, 10, 6))),
                () => Assert.IsTrue(attrib.IsValid(new DateTime(2015, 3, 31))),
                () => Assert.IsTrue(attrib.IsValid(new DateTime(2020, 1, 20))),
                () => Assert.IsFalse(attrib.IsValid(new DateTime(2020, 1, 21)))
            );
        }

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

    }
}
