using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.TimeUtilities;
using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.TimeUtilities
{
    [TestClass]
    public class DateTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            Assert.AreEqual(new DateTime(1, 2, 3), new Date(1, 2, 3));
            //Assert.AreEqual(new DateTime(), new Date(2003, -2, -5));
            Assert.AreEqual(new DateTime(50000 * TimeSpan.TicksPerDay), new Date(50000 * TimeSpan.TicksPerDay));
        }

        [TestMethod]
        public void LessThan()
        {
            Multiple(
                () => Assert.IsTrue(new Date(2016,10,20) < new Date(2016, 10, 21)),
                () => Assert.IsFalse(new Date(2020, 1, 5) < new Date(2020, 1, 5), "Equal"),
                () => Assert.IsFalse(new Date(2020, 1, 5) < new Date(2019, 9, 15), "Higher")
            );
        }

        [TestMethod]
        public void GreaterThan()
        {
            Multiple(
                () => Assert.IsFalse(new Date(2016, 9, 29) > new Date(2016, 10, 21)),
                () => Assert.IsFalse(new Date(2020, 1, 5) > new Date(2020, 1, 5), "Equal"),
                () => Assert.IsTrue(new Date(2020, 1, 5) > new Date(2019, 9, 15), "Higher")
            );
        }

        [TestMethod]
        public void Subtract()
        {
            Assert.AreEqual(new Date(2016, 10, 25), new Date(2016, 10, 30).Subtract(new TimeSpan(5,0,0,0)));
        }

        [TestMethod]
        public void Subtract_Negative()
        {
            Assert.AreEqual(new Date(2010, 1, 1), new Date(2009, 12, 31).Subtract(new TimeSpan(-1, 0, 0, 0)));
        }

        [TestMethod]
        public void Kind()
        {
            Assert.AreEqual(DateTimeKind.Utc,new Date(1200*TimeSpan.TicksPerDay, DateTimeKind.Utc).Kind);
        }

        [TestMethod]
        public void Equals()
        {
            // DateTime.Equals does not include the Kind:
            Assert.AreEqual(new DateTime(9999, 12, 31, 0, 0, 0, DateTimeKind.Utc), new DateTime(9999, 12, 31, 0, 0, 0, DateTimeKind.Local));

            // So Date doesn't either:
            Assert.IsTrue(new Date(9999,12,31, DateTimeKind.Local).Equals( new DateTime(9999,12,31, 0,0,0, DateTimeKind.Utc)) );

            Assert.IsFalse(new Date(1, 1, 1, DateTimeKind.Local).Equals(new Date(1,1,2)));
        }

        [TestMethod]
        public void AddDays()
        {
            Assert.AreEqual(_date2005.AddDays(31), new Date(2005, 2, 1));
        }
        [TestMethod]
        public void AddMonths()
        {
            Assert.AreEqual(_date2005.AddMonths(-2), new Date(2004,11,1));
        }
        [TestMethod]
        public void AddYears()
        {
            Assert.AreEqual(_date2005.AddYears(50), new Date(2055, 1, 1));
        }

        [TestMethod]
        public void Minus()
        {
            Assert.AreEqual( new TimeSpan(5,0,0,0), new Date(2005,1,6) - _date2005);
        }

        [TestMethod]
        public void ToStringTest()
        {
            string s = _date2005.ToString();
            Console.WriteLine(s);

            Assert.AreEqual(s, new DateTime(2005,1,1).ToShortDateString());
        }

        [TestMethod]
        public void CombineTime()
        {
            var date = _date2005.CombineTime(new Time(17,30));
            Assert.AreEqual(date, new DateTime(2005, 1, 1, 17, 30,0, DateTimeKind.Local));
        }

        [TestMethod]
        public void BinaryDate()
        {
            var date = Date.FromBinaryDate(_date2005.ToBinaryDate());
            Assert.AreEqual(_date2005, date);
            Assert.AreEqual(_date2005.Kind, date.Kind);
        }

        [TestMethod]
        public void ToBinary()
        {
            var date = Date.FromBinary(_date2005.ToBinary());
            Assert.AreEqual(_date2005, date);
            Assert.AreEqual(_date2005.Kind, date.Kind);
        }

        protected Date _date2005 = new Date(2005, 1, 1, DateTimeKind.Local);
    }
}
