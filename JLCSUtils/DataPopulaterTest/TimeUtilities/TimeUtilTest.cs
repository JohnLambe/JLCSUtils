using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.TimeUtilities;

namespace JohnLambe.Tests.JLUtilsTest.TimeUtilities
{
    [TestClass]
    public class TimeUtilTest
    {
        [TestMethod]
        public void TimeOfDayNullable()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(new TimeSpan(20, 30, 12), TimeUtil.TimeOfDay(new TimeSpan(76, 20, 30, 12)),"Common case"),
                () => Assert.AreEqual(TimeSpan.Zero, TimeUtil.TimeOfDay(TimeSpan.Zero),"Zero"),
                () => Assert.AreEqual(TimeSpan.Zero, TimeUtil.TimeOfDay(TimeSpan.Zero)),
                () => Assert.AreEqual(null, TimeUtil.TimeOfDay(null), "null")
                );
        }

        [TestMethod]
        public void TimeOfDay()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(new TimeSpan(20, 30, 12), TimeUtil.TimeOfDay(new TimeSpan(76, 20, 30, 12)), "Common case"),
                () => Assert.AreEqual(TimeSpan.Zero, TimeUtil.TimeOfDay(TimeSpan.Zero), "Zero"),
                () => Assert.AreEqual(TimeSpan.Zero, TimeUtil.TimeOfDay(TimeSpan.Zero))
                );
        }

        [TestMethod]
        public void IsTimeOfDay()
        {
            TestUtil.Multiple(
                () => Assert.IsFalse(TimeUtil.IsTimeOfDay(new TimeSpan(24,1,5))),
                () => Assert.IsFalse(TimeUtil.IsTimeOfDay(new TimeSpan(-1, 1, 5))),
                () => Assert.IsTrue(TimeUtil.IsTimeOfDay(new TimeSpan(23, 59, 59)))
                );
        }

        [TestMethod]
        public void IsDateOnly()
        {
            TestUtil.Multiple(
                () => Assert.IsTrue(TimeUtil.IsDateOnly(new DateTime(2020, 5, 2))),
                () => Assert.IsFalse(TimeUtil.IsDateOnly(new DateTime(1000, 5, 10, 0, 0, 1)))
            );
        }

        [TestMethod]
        public void ToDateTime_TimeSpan()
        {
            Assert.AreEqual(new DateTime(1,1,1,10,5,40),TimeUtil.ToDateTime(new TimeSpan(10,5,40)));
        }

        [TestMethod]
        public void ToDateTime()
        {
            Assert.AreEqual(new DateTime(2040, 8, 15, 10, 5, 40), TimeUtil.ToDateTime(new DateTime(2040,8,15), new TimeSpan(10, 5, 40)));
        }

        [TestMethod]
        public void EndOfDay()
        {
            TestUtil.Multiple(
                () =>
                {
                    DateTime? value = new DateTime(2011, 2, 5);
                    var result = value.EndOfDay();
                    Assert.AreEqual(new DateTime(2011, 2, 5) + TimeUtil.EndOfDayTime, result);
                    Assert.IsTrue(result > new DateTime(2011, 2, 5));
                },
                () =>
                {
                    Assert.AreEqual(new DateTime(2001, 12, 31) + TimeUtil.EndOfDayTime, new DateTime(2001, 12, 31, 11, 27, 45).EndOfDay());
                }
            );
        }

        [TestMethod]
        public void EndOfDay_Null()
        {
            Assert.AreEqual(null, TimeUtil.EndOfDay(null));
        }

        [TestMethod]
        public void SqlEndOfMonth()
        {
            DateTime value = new DateTime(2001, 12, 3, 11, 27, 45);
            Assert.AreEqual(new DateTime(2001, 12, 31) + TimeUtil.SqlEndOfDayTime, TimeUtil.SqlEndOfMonth(value));
        }

        [TestMethod]
        public void ChangeTime()
        {
            Assert.AreEqual(new DateTime(2020,5,2,15,37,50), TimeUtil.ChangeTime(new DateTime(2020,5,2, 5,6,7), new TimeSpan(15,37,50)));
        }
    }
}
