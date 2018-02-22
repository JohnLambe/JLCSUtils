using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util;
using JohnLambe.Util.Types;
using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace ExperimentalTest
{
    [TestClass]
    public class IDTest
    {
        [TestMethod]
        public void IntConversion()
        {
            IntID x;
            x = (IntID)5;
            x = (IntID)((int)x + 1);

            Assert.AreEqual(6, (int)x);
        }

        [TestMethod]
        public void Constructor()
        {
            const int Value = -1234567;

            // Act:
            IntID x = new IntID(Value);
            IntID y = new IntID();

            // Assert:
            Multiple(
                () => Assert.AreEqual(Value, (int)x),
                () => Assert.AreEqual(0, (int)y)
            );
        }

        [TestMethod]
        public void Comparison()
        {
            IntID x = (IntID)5, y = (IntID)10, z = (IntID)5;
            IntID a = x;

            Multiple(
                () => Assert.AreEqual(x, z),
                () => Assert.AreEqual(x, a),
                () => Assert.AreNotEqual(x, y)
            );
        }
    }
}
