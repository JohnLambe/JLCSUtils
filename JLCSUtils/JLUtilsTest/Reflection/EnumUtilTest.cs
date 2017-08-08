using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JohnLambe.Tests.JLUtilsTest.Reflection
{
    [TestClass]
    public class EnumUtilTest
    {
        [TestMethod]
        public void HasAnyFlag_Integer()
        {
            System.AttributeTargets x = AttributeTargets.Class | AttributeTargets.Assembly;
            Assert.IsTrue(x.HasAnyFlag((int)AttributeTargets.Class));
        }

        [TestMethod]
        public void HasAnyFlag()
        {
            System.AttributeTargets x = AttributeTargets.Class | AttributeTargets.Assembly;

            Multiple(
                () => Assert.IsTrue(x.HasAnyFlag(AttributeTargets.Class | AttributeTargets.Delegate)),
                () => Assert.IsFalse(x.HasAnyFlag(AttributeTargets.Method | AttributeTargets.ReturnValue))
            );
        }

        /// <summary>
        /// Test with the wrong type for the flags.
        /// </summary>
        [TestMethod]
        public void HasAnyFlag_Fails()
        {
            System.Base64FormattingOptions x = Base64FormattingOptions.InsertLineBreaks;
            TestUtil.AssertThrows<Exception>( () => x.HasAnyFlag(System.DayOfWeek.Wednesday), "Different enum types" );
        }

        /// <summary>
        /// Non-Flags enum.
        /// </summary>
        [TestMethod]
        public void HasAnyFlagValidated_Fail()
        {
            TestUtil.AssertThrows<Exception>(() => System.DayOfWeek.Wednesday.HasAnyFlagValidated(System.DayOfWeek.Wednesday), "Non-Flags enum");
        }

        [TestMethod]
        public void HasAnyFlagValidated()
        {
            System.ConsoleModifiers x = ConsoleModifiers.Control;
            Multiple(
                () => Assert.IsTrue(x.HasAnyFlag(ConsoleModifiers.Control | ConsoleModifiers.Alt)),
                () => Assert.IsFalse(x.HasAnyFlag(ConsoleModifiers.Alt))
            );
        }

        [TestMethod]
        public void ValidateEnum_Valid()
        {
            DayOfWeek x = DayOfWeek.Sunday;
            Assert.IsTrue(x.ValidateEnumValue());
        }

        [TestMethod]
        public void ValidateEnum_Invalid()
        {
            DayOfWeek y = (DayOfWeek)125;
            Assert.IsFalse(y.ValidateEnumValue());
        }

        [TestMethod]
        public void GetIntegerValue()
        {
            Assert.AreEqual(2, DayOfWeek.Tuesday.GetIntegerValue());
        }

        [TestMethod]
        public void Enum()
        {
            Enum x = DayOfWeek.Monday;
            int i = (int)System.Convert.ChangeType(x, typeof(int));
            Console.WriteLine(i);
            Assert.AreEqual(1, i);
        }

        [TestMethod]
        public void GetDisplayName()
        {
            Multiple(
                () => Assert.AreEqual("Monday", DayOfWeek.Monday.GetDisplayName()),

                () =>
                {
                    Enum x1 = AttributeTargets.ReturnValue;
                    Assert.AreEqual("Return Value", x1.GetDisplayName());
                },

                () => Assert.AreEqual("display name from attribute", TestEnumType.T1.GetDisplayName())
            );
        }

        #region ConvertEnum

        [TestMethod]
        public void ConvertEnum()
        {
            Assert.AreEqual(DayOfWeek2.Friday, EnumUtil.ConvertEnum<DayOfWeek2>(DayOfWeek.Friday));
            Assert.AreEqual(DayOfWeek2.Wednesday, EnumUtil.ConvertEnum<DayOfWeek2>(DayOfWeek.Wednesday));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertEnum_Fail()
        {
            Console.Out.WriteLine(EnumUtil.ConvertEnum<DayOfWeek2>(DayOfWeek.Monday));  // using Monday because it has the same integer value as a defined value in the target type
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertEnum_Fail_CaseSensitive()
        {
            Console.Out.WriteLine(EnumUtil.ConvertEnum<DayOfWeek2>(DayOfWeek.Thursday));
        }

        [TestMethod]
        public void ConvertEnum_IgnoreCase()
        {
            Assert.AreEqual(DayOfWeek2.THURSDAY, EnumUtil.ConvertEnum<DayOfWeek2>(DayOfWeek.Thursday,true));
        }

        [TestMethod]
        public void ConvertEnum_Default()
        {
            Assert.AreEqual(DayOfWeek2.A, EnumUtil.ConvertEnum<DayOfWeek2>(DayOfWeek.Thursday, DayOfWeek2.A), "uses default");
            Assert.AreEqual(DayOfWeek2.Friday, EnumUtil.ConvertEnum<DayOfWeek2>(DayOfWeek.Friday, DayOfWeek2.A, false), "shouldn't retun default");
        }

        // no tests for invalid types

        [TestMethod]
        public void ConvertEnum_Nullable()
        {
            Assert.AreEqual(null, EnumUtil.ConvertEnum<DayOfWeek2?>(null), "null");
            Assert.AreEqual(DayOfWeek2.Friday, EnumUtil.ConvertEnum<DayOfWeek2?>(DayOfWeek.Friday, DayOfWeek2.A, false), "nullable but not null");
        }

        enum DayOfWeek2 { Wednesday = 1, Friday = 2000, THURSDAY = 6, A = 50 }

        #endregion

        public enum TestEnumType
        {
            [Display(Name = "display name from attribute")]
            T1,
            T2,
            T3
        }

    }
}