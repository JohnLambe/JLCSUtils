using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Reflection;
using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.Reflection
{
    [TestClass]
    public class TypeUtilTest
    {
        #region Numeric

        [TestMethod]
        public void IsNumeric()
        {
            Multiple(
                () => Assert.IsTrue(TypeUtil.IsNumeric(56)),
                () => Assert.IsTrue(TypeUtil.IsNumeric(-23.34)),
                () => Assert.IsFalse(TypeUtil.IsNumeric("dsfd"))
            );
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
            Multiple(
                () => Assert.IsTrue(typeof(short).IsIntegerType()),
                () => Assert.IsTrue(typeof(long?).IsIntegerType(), "Nullable value type")
            );
        }

        #endregion

        #region Floating point

        [TestMethod]
        public void IsFloatingPointType()
        {
            Multiple(
                () => Assert.IsTrue(typeof(decimal).IsFloatingPointType()),
                () => Assert.IsTrue(typeof(Single?).IsFloatingPointType(), "Nullable value type")
            );
        }

        [TestMethod]
        public void IsTextType_Nullable()
        {
            Multiple(
                () => Assert.IsTrue(typeof(char?).IsTextType()),
                () => Assert.IsFalse(typeof(int?).IsTextType())
            );
        }

        #endregion

        #region IsMoreSpecific

        [TestMethod]
        public void IsMoreSpecific()
        {
            Multiple(
                () => Assert.AreEqual(0, TypeUtil.IsMoreSpecific(typeof(int),typeof(int))),
                () => AssertGreaterThan(0, TypeUtil.IsMoreSpecific(typeof(SubClass), typeof(BaseClass))),
                () => AssertLessThan(0, TypeUtil.IsMoreSpecific(typeof(BaseClass), typeof(SubClass))),

                () => AssertLessThan(0, TypeUtil.IsMoreSpecific(typeof(ITest), typeof(SubClass))),
                () => AssertLessThan(0, TypeUtil.IsMoreSpecific(typeof(ITest), typeof(BaseClass))),
                () => Assert.AreEqual(0, TypeUtil.IsMoreSpecific(typeof(ITest), typeof(ITest))),

                // null:
                () => Assert.AreEqual(0, TypeUtil.IsMoreSpecific(null, null)),
                () => AssertGreaterThan(0, TypeUtil.IsMoreSpecific(typeof(ITest), null)),
                () => AssertLessThan(0, TypeUtil.IsMoreSpecific(null, typeof(ITest))),

                () => Assert.AreEqual(0, TypeUtil.IsMoreSpecific(typeof(ITest), typeof(string)), "types with no relation to each other")
            );
        }


        public class BaseClass : ITest { }

        public class SubClass : BaseClass { }

        public interface ITest { }

        #endregion

    }
}
