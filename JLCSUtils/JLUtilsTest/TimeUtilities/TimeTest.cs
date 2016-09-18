using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.TimeUtilities;

namespace JohnLambe.Tests.JLUtilsTest.TimeUtilities
{
    [TestClass]
    public class TimeTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            Assert.AreEqual(new TimeSpan(0, 1, 2, 3), new Time(1, 2, 3));
            Assert.AreEqual(new TimeSpan(50000), new Time(50000));
        }

        [TestMethod]
        public void LessThan()
        {
            Assert.IsTrue(new Time(20, 10, 20) < new Time(20, 10, 21));
            Assert.IsFalse(new Time(20, 1, 5) < new Time(20, 1, 5), "Equal");
            Assert.IsFalse(new Time(20, 10, 5) < new Time(20, 9, 15), "Higher");
        }

        [TestMethod]
        public void GreaterThan()
        {
            Assert.IsFalse(new Time(20, 9, 29) > new Time(20, 10, 21));
            Assert.IsFalse(new Time(20, 1, 5) > new Time(20, 1, 5), "Equal");
            Assert.IsTrue(new Time(20, 10, 5) > new Time(20, 9, 15), "Higher");
        }

        [TestMethod]
        public void Add()
        {
            Assert.AreEqual(new Time(16, 10, 30), new Time(15, 10, 30).Add(new TimeSpan(0, 1, 0, 0)));
        }

        [TestMethod]
        public void Subtract_Negative()
        {
            Assert.AreEqual(new Time(10, 1, 1), new Time(09, 1, 1).Subtract(new TimeSpan(0, -1, 0, 0)));
        }

        /*
        [TestMethod]
        public void Kind()
        {
            Assert.AreEqual(DateTimeKind.Utc, new Date(1200 * TimeSpan.TicksPerDay, DateTimeKind.Utc).Kind);
        }

        [TestMethod]
        public void Equals()
        {
            // DateTime.Equals does not include the Kind:
            Assert.AreEqual(new DateTime(9999, 12, 31, 0, 0, 0, DateTimeKind.Utc), new DateTime(9999, 12, 31, 0, 0, 0, DateTimeKind.Local));

            // So Date doesn't either:
            Assert.IsTrue(new Date(9999, 12, 31, DateTimeKind.Local).Equals(new DateTime(9999, 12, 31, 0, 0, 0, DateTimeKind.Utc)));

            Assert.IsFalse(new Date(1, 1, 1, DateTimeKind.Local).Equals(new Date(1, 1, 2)));
        }
*/
        [TestMethod]
        public void AddHours()
        {
            Assert.AreEqual(_time2005.AddHours(2), new Time(22,05));
        }
        [TestMethod]
        public void AddMinutes()
        {
            Assert.AreEqual(_time2005.AddMinutes(-2), new Time(20, 3));
        }
        [TestMethod]
        public void AddSeconds()
        {
            Assert.AreEqual(_time2005.AddSeconds(50), new Time(20, 5, 50));
        }
        [TestMethod]
        public void AddMilliseconds()
        {
            Assert.AreEqual(_time2005.AddMilliseconds(150), new Time(20, 5, 0, 150));
        }

        [TestMethod]
        public void Minus()
        {
            Assert.AreEqual(new TimeSpan(0, 3, 0, 0), new Time(23, 5) - _time2005);
        }

        [TestMethod]
        public void ToStringTest()
        {
            string s = _time2005.ToString();
            Console.WriteLine(s);

            Assert.AreEqual(s, new DateTime(1950,3,2, 20,05,0).ToShortTimeString());
        }
/*
        [TestMethod]
        public void BinaryDate()
        {
            var date = Date.FromBinaryDate(_time2005.ToBinaryDate());
            Assert.AreEqual(_time2005, date);
            Assert.AreEqual(_time2005.Kind, date.Kind);
        }

        [TestMethod]
        public void ToBinary()
        {
            var date = Date.FromBinary(_time2005.ToBinary());
            Assert.AreEqual(_time2005, date);
            Assert.AreEqual(_time2005.Kind, date.Kind);
        }
        *
        */

        protected Time _time2005 = new Time(20, 5);
    }
}
