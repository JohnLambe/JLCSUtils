﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            Assert.IsTrue(x.HasAnyFlag(AttributeTargets.Class | AttributeTargets.Delegate));
            Assert.IsFalse(x.HasAnyFlag(AttributeTargets.Method | AttributeTargets.ReturnValue));
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
            Assert.IsTrue(x.HasAnyFlag(ConsoleModifiers.Control | ConsoleModifiers.Alt));
            Assert.IsFalse(x.HasAnyFlag(ConsoleModifiers.Alt));
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
        }
    }
}