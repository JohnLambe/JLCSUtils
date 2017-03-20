using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Reflection;

namespace JohnLambe.Tests.JLUtilsTest.Reflection
{
    [TestClass]
    public class TypeUtilTest
    {
        [TestMethod]
        public void IsNumeric()
        {
            Assert.IsTrue(TypeUtil.IsNumeric(56));
            Assert.IsTrue(TypeUtil.IsNumeric(-23.34));
            Assert.IsFalse(TypeUtil.IsNumeric("dsfd"));
        }

        [TestMethod]
        public void IsNumeric_Nullable()
        {
            int? x = 45;
            Assert.IsTrue(TypeUtil.IsNumeric(x));
        }

        [TestMethod]
        public void IsIntegerType()
        {
            Assert.IsTrue(typeof(short).IsIntegerType());
            Assert.IsTrue(typeof(long?).IsIntegerType(), "Nullable value type");
        }

        [TestMethod]
        public void IsFloatingPointType()
        {
            Assert.IsTrue(typeof(decimal).IsFloatingPointType());
            Assert.IsTrue(typeof(Single?).IsFloatingPointType(), "Nullable value type");
        }

        [TestMethod]
        public void IsTextType_Nullable()
        {
            Assert.IsTrue(typeof(char?).IsTextType());
            Assert.IsFalse(typeof(int?).IsTextType());
        }

    }
}
