using JohnLambe.Util.TimeUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.TimeUtilities
{
    [TestClass]
    public class EndDateTest
    {
        [TestMethod]
        public void EndDate_Assign()
        {
            Assert.AreEqual(new DateTime(2010, 11, 5, 23, 59, 59, 999), ((EndDate)new DateTime(2010, 11, 5, 20, 05, 10)).Value);
            Assert.AreEqual(new DateTime(2190, 1, 9, 23, 59, 59, 999), (DateTime)((EndDate)new DateTime(2190, 1, 9, 23, 59, 59, 999)));
        }
    }

    [TestClass]
    public class StartDateTest
    {
        [TestMethod]
        public void StartDate_Assign()
        {
            Assert.AreEqual(new DateTime(2010, 11, 5), ((StartDate)new DateTime(2010, 11, 5, 20, 05, 10)).Value);
            Assert.AreEqual(new DateTime(2190, 1, 9), ((StartDate)new DateTime(2190, 1, 9, 23, 59, 59, 999)).Value, "End of day");  // test that it doesn't round up
            Assert.AreEqual(new DateTime(11, 3, 20), ((StartDate)new DateTime(11, 3, 20)).Value, "No time part in original");
        }
    }
}
