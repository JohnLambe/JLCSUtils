using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.MathUtilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnLambe.Tests.JLUtilsTest.MathUtilities
{
    [TestClass]
    public class MathUtilTest
    {
        [TestMethod]
        public void RoundToMultiple()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(200, MathUtil.RoundToMultiple(151, 100) ),   // int
                () => Assert.AreEqual(0.5m, MathUtil.RoundToMultiple(0.523m, 0.1m)),
                () => Assert.AreEqual(40, MathUtil.RoundToMultiple(45m, 10m)),
                () => Assert.AreEqual(50, MathUtil.RoundToMultiple(45m, 10m, MidpointRounding.AwayFromZero))
                );
        }

        [TestMethod]
        public void Round_Nullable()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(10, MathUtil.Round(10.5)),
                () => Assert.AreEqual(null, MathUtil.Round((double?)null, 2))
                );
        }
    }
}
