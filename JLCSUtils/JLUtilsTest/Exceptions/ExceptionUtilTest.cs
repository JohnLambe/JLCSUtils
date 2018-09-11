using JohnLambe.Util.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JohnLambe.Tests.JLUtilsTest.Exceptions
{
    [TestClass]
    public class ExceptionUtilTest
    {
        [TestMethod]
        public void ExtractException_Null()
        {
            Assert.AreEqual(null, ExceptionUtil.ExtractException(null));
        }

        [TestMethod]
        public void ExtractException_NonWrapped()
        {
            // Arranage:
            var ex = new InvalidOperationException("test", new InvalidCastException());

            // Act/Assert:
            Assert.AreEqual(ex, ExceptionUtil.ExtractException(ex));
        }

        [TestMethod]
        public void ExtractException_Wrapped()
        {
            // Act/Assert:
            Assert.AreEqual(ex, ExceptionUtil.ExtractException(ex2));
        }

        [TestMethod]
        public void ExtractExceptionOfType()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(ex, ExceptionUtil.ExtractExceptionOfType(ex2, typeof(ArgumentException), typeof(InvalidOperationException))),
                () => Assert.AreEqual(ex2, ExceptionUtil.ExtractExceptionOfType(ex2, typeof(ArgumentException), typeof(Exception))),    // matches the original exception (subclass)
                () => Assert.AreEqual(ex2, ExceptionUtil.ExtractExceptionOfType(ex2, typeof(TargetInvocationException))),    // matches the original exception
                () => Assert.AreEqual(ex, ExceptionUtil.ExtractExceptionOfType(ex2, typeof(SystemException)))    // the returned exception is a subclass of the required one
            );
        }

        [TestMethod]
        public void ExtractExceptionOfType_NotFound()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(null, ExceptionUtil.ExtractExceptionOfType(ex2, typeof(ArgumentException), typeof(FileNotFoundException))),
                () => Assert.AreEqual(null, ExceptionUtil.ExtractExceptionOfType(ex2))
            );
        }

        [TestMethod]
        public void ExtractExceptionOfType_Null()
        {
            TestUtil.Multiple(
                () => Assert.AreEqual(null, ExceptionUtil.ExtractExceptionOfType(null, typeof(ArgumentException), typeof(InvalidOperationException))),
                () => Assert.AreEqual(null, ExceptionUtil.ExtractExceptionOfType(null))
            );
        }

        static Exception ex = new InvalidOperationException("test", new InvalidCastException("test2", new TargetInvocationException(new SystemException())));  // exception to be extracted (which has inner exceptions itself)
        static Exception ex2 = new TargetInvocationException("to be removed", new TargetInvocationException("to be removed 2", ex));  // nested two levels deep
    }
}
