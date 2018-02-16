using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Services;
using JohnLambe.Util.Types;

namespace JohnLambe.Tests.JLUtilsTest.Services
{
    [TestClass]
    public class RandomServiceTest
    {
        [TestMethod]
        public void RandomGuid()
        {
            // Act:
            var g1 = _randomService.RandomGuid();
            var g2 = _randomService.RandomGuid();

            // Assert:
            Assert.AreNotEqual(g1, g2, "Duplicate");
            Assert.AreEqual(4, g1.GetVersion(), "Wrong GUID version");
        }

        protected IRandomService _randomService = new RandomService(1234);
    }
}
