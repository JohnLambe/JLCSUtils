using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util;

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class ObjectUtilTest
    {
        [TestMethod]
        public void CompareEqual()
        {
            TestUtil.Multiple(
               () => Assert.IsTrue(ObjectUtil.CompareEqual(100, 100)),
               () => Assert.IsTrue(ObjectUtil.CompareEqual(null, null)),
               () => Assert.IsFalse(ObjectUtil.CompareEqual(null, 'Z')),
               () => Assert.IsFalse(ObjectUtil.CompareEqual(null, "")),
               () =>
               {
                   int? x = null;
                   Assert.IsFalse(ObjectUtil.CompareEqual('c', x));
               },
               () => Assert.IsFalse(ObjectUtil.CompareEqual('c', null)),
               () =>
               {
                   int? n = null;
                   char? c = null;
                   Assert.IsTrue(ObjectUtil.CompareEqual(c, n));
               }
            );
        }
    }
}
