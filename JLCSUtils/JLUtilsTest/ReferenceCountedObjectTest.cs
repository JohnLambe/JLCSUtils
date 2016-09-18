using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util;

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class ReferenceCountedObjectTest
    {
        [TestMethod]
        public void ReleaseRef()
        {
            var x = new ReferenceCountedObject();
            Assert.IsTrue(!x.IsDisposed);
            x.ReleaseRef();

            Assert.IsTrue(x.IsDisposed);
        }

        [TestMethod]
        public void AddAndReleaseRef()
        {
            var x = new ReferenceCountedObject();
            x.AddRef();
            x.ReleaseRef();
            Assert.IsTrue(!x.IsDisposed);
            x.ReleaseRef();

            Assert.IsTrue(x.IsDisposed);
        }

        [TestMethod]
        public void SubClass()
        {
            var x = new ReferenceCountedObjectSubclass();
            x.ReleaseRef();

            Assert.IsTrue(x.IsDisposed);
            Assert.AreEqual(x.Test,"Disposed");
        }

    }

    public class ReferenceCountedObjectSubclass : ReferenceCountedObject
    {
        public string Test { get; set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Test = "Disposed";
        }
    }
}
