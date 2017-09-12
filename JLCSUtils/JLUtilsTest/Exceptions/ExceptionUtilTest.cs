using JohnLambe.Util.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            Assert.AreEqual(null, ExceptionUtil.ExtractException(null));
        }

        [TestMethod]
        public void ExtractException_Wrapped()
        {
            // Arranage:
            var ex = new InvalidOperationException("test", new InvalidCastException("test2", new TargetInvocationException(new SystemException())));
            var ex2 = new TargetInvocationException("to be removed", new TargetInvocationException("to be removed 2", ex));

            // Act/Assert:
            Assert.AreEqual(ex, ExceptionUtil.ExtractException(ex2));
        }
    }
}
