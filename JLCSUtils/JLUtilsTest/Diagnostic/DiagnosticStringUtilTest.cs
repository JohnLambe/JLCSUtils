using JohnLambe.Util.Diagnostic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.Diagnostic
{
    [TestClass]
    public class DiagnosticStringUtilTest
    {
        #region ObjectToString

        [TestMethod]
        public void ObjectToString_Struct()
        {
            object p = new Point(50, 10);

            Assert.AreEqual("System.Drawing.Point(IsEmpty=False X=50 Y=10)", DiagnosticStringUtil.ObjectToString(p));
        }

        [TestMethod]
        public void ObjectToString_Primitive()
        {
            int x = 100;

            Assert.AreEqual("Int32(100)", DiagnosticStringUtil.ObjectToString(x));
        }

        [TestMethod]
        public void ObjectToString_Null()
        {
            System.ArgumentException x = null;

            Assert.AreEqual("System.ArgumentException(null)", DiagnosticStringUtil.ObjectToString(x));
        }

        [TestMethod]
        public void ObjectToString_AnonymousClass()
        {
            var x = new
            {
                Property1 = 50,
                Property2 = "string"
            };

            Assert.AreEqual("(Property1=50 Property2=string)", DiagnosticStringUtil.ObjectToString(x));
        }

        #endregion
    }
}
