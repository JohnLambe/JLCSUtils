using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.Collections;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.Collections
{
    [TestClass]
    public class CachedSimpleLookupTest
    {
        [TestMethod]
        public void BasicLookup()
        {
            Setup();

            // Assert:

            Assert.AreEqual("Value100", Lookup[100]);
            Assert.AreEqual("Value-5", Lookup[-5]);
        }

        /// <summary>
        /// Test that cache is used.
        /// </summary>
        [TestMethod]
        public void CacheLookup()
        {
            Setup();

            // Assert:
            Multiple(
                () => Assert.AreEqual("Value10", Lookup[10]),
                () => Assert.AreEqual(1, TimesFired),
                () => Assert.AreEqual("Value10", Lookup[10]),     // look up again
                () => Assert.AreEqual(1, TimesFired),             // didn't reevaluate

                () => Assert.AreEqual("Value11", Lookup[11]),     // look up a different value
                () => Assert.AreEqual(2, TimesFired)            
            );
        }

        protected void Setup()
        {
            Lookup = new CachedSimpleLookup<int, string>(
                k => { TimesFired++; return "Value" + k; });
            TimesFired = 0;
        }

        protected ISimpleLookup<int, string> Lookup;
        protected int TimesFired;
    }
}
