using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnLambe.Tests.JLUtilsTest.Misc
{
    [TestClass]
    public class RangeTest
    {
        [TestMethod]
        public void Constrain()
        {
            // Arrange:
            var range = new Range<int>(10, 20);

            // Act/Assert:
            TestUtil.Multiple(
                () => Assert.AreEqual(12, range.Constrain(12)),
                () => Assert.AreEqual(10, range.Constrain(10)),
                () => Assert.AreEqual(10, range.Constrain(-4)),
                () => Assert.AreEqual(20, range.Constrain(int.MaxValue))
                );
        }

    }
}
