﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.TimeUtilities;

namespace JohnLambe.Tests.JLUtilsTest.TimeUtilities
{
    [TestClass]
    public class TimeUtilsTest
    {
        [TestMethod]
        public void TimeOfDayNullable()
        {
            Assert.AreEqual(new TimeSpan(20, 30, 12), TimeUtils.TimeOfDay(new TimeSpan(76, 20, 30, 12)),"Common case");
            Assert.AreEqual(TimeSpan.Zero, TimeUtils.TimeOfDay(TimeSpan.Zero),"Zero");
            Assert.AreEqual(TimeSpan.Zero, TimeUtils.TimeOfDay(TimeSpan.Zero));
            Assert.AreEqual(null, TimeUtils.TimeOfDay(null), "null");
        }

        [TestMethod]
        public void TimeOfDay()
        {
            Assert.AreEqual(new TimeSpan(20, 30, 12), TimeUtils.TimeOfDay(new TimeSpan(76, 20, 30, 12)), "Common case");
            Assert.AreEqual(TimeSpan.Zero, TimeUtils.TimeOfDay(TimeSpan.Zero), "Zero");
            Assert.AreEqual(TimeSpan.Zero, TimeUtils.TimeOfDay(TimeSpan.Zero));
        }

        [TestMethod]
        public void IsTimeOfDay()
        {
            Assert.IsFalse(TimeUtils.IsTimeOfDay(new TimeSpan(24,1,5)));
            Assert.IsFalse(TimeUtils.IsTimeOfDay(new TimeSpan(-1, 1, 5)));
            Assert.IsTrue(TimeUtils.IsTimeOfDay(new TimeSpan(23, 59, 59)));
        }

        [TestMethod]
        public void IsDateOnly()
        {
            Assert.IsFalse(TimeUtils.IsDateOnly(new DateTime(1000,5,10,0,0,1)));
            Assert.IsTrue(TimeUtils.IsDateOnly(new DateTime(2000, 5, 10, 0, 0, 0)));
        }

        [TestMethod]
        public void ToDateTime_TimeSpan()
        {
            Assert.AreEqual(new DateTime(1,1,1,10,5,40),TimeUtils.ToDateTime(new TimeSpan(10,5,40)));
        }

        [TestMethod]
        public void ToDateTime()
        {
            Assert.AreEqual(new DateTime(2040, 8, 15, 10, 5, 40), TimeUtils.ToDateTime(new DateTime(2040,8,15), new TimeSpan(10, 5, 40)));
        }
    }
}
