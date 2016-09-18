using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util;

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class MiscUtilTest
    {
        [TestMethod]
        public void IfNotNull()
        {
            string s = "asd  ";
            Assert.AreEqual(s, MiscUtil.IfNotNull(s, x => x.ToString()), "Returns same value");

            Assert.AreEqual(s.Trim(), s.IfNotNull(x => x.Trim()), "Returns same type");

            Assert.AreEqual(s.Length, MiscUtil.IfNotNull(s, x => x.Length), "non-null, primitive return type.");

            Assert.AreEqual(s.Trim().Length, s.IfNotNull(x => x.Trim()).IfNotNull(x => x.Length), "non-null: object, then primitive");

            s = null;
            Assert.AreEqual(s, MiscUtil.IfNotNull(s, x => x.ToString()), "null: same type.");

            Assert.AreEqual(null, s.IfNotNull(x => x.Trim()), "null: Returns same type");

            Assert.AreEqual(0, MiscUtil.IfNotNull(s, x => x.Length), "null: primitive return type.");

            Assert.AreEqual(0,
                s.IfNotNull(x => x.Trim()).IfNotNull(x => x.Length),
                "null: object, then primitive");
            Assert.AreEqual(0,
                s.IfNotNull(x => x.Trim(), x => x.Length),
                "null: object, then primitive; 2-delegate overload");
            // equivalent to:
            int len =
                s == null ? 0 : (s.Trim() == null ? 0 : s.Trim().Length);

        }

        [TestMethod]
        public void DisposeAndNull_Disposable()
        {
            // Arrange:
            var a = new DisposableTestObject();
            var a_copy = a;
            Assert.IsTrue(a._disposed == false);

            // Act:
            MiscUtil.DisposeAndNull(ref a);

            // Assert:
            Assert.AreEqual(null, a, "Not set to null");
            Assert.AreEqual(true, a_copy._disposed, "Not disposed");
        }

        [TestMethod]
        public void DisposeAndNull_NonDisposable()
        {
            // Arrange:
            var a = new Object();

            // Act:
            MiscUtil.DisposeAndNull(ref a);

            // Assert:
            Assert.AreEqual(null, a, "Not set to null");
        }

    }

    public class DisposableTestObject : IDisposable
    {
        public bool _disposed = false;  // to test that Dispose is called.

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
