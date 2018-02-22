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
    public class FlowControlUtilTest
    {
        [TestMethod]
        public void ForEach()
        {
            // Arrange:
            string output = "";
            char[] values = { 'a', 'x', 'g', 'z' };

            // Act:
            Assert.AreEqual(true,FlowControlUtil.ForEach(values, (i,v) => { output += $"({i},{v}) "; return true; } ));

            // Assert:
            Assert.AreEqual("(0,a) (1,x) (2,g) (3,z) ",output);
        }

        [TestMethod]
        public void ForEach_Void()
        {
            // Arrange:
            string output = "";
            char[] values = { 'a', 'x', 'g', 'z' };

            // Act:
            Assert.AreEqual(true, FlowControlUtil.ForEach(values, (i, v) => { output += $"({i},{v}) "; }, 10, 2));

            // Assert:
            Assert.AreEqual("(10,a) (12,x) (14,g) (16,z) ", output);
        }
    }
}
