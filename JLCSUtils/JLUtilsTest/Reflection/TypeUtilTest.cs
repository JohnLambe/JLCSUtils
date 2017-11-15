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
        [TestMethod]
        public void IsNullable()
        {
            Multiple(
                () => Assert.AreEqual(false, TypeUtil.IsNullable(typeof(int)), "primitive"),
                () => Assert.AreEqual(true, TypeUtil.IsNullable(typeof(int?)), "nullable primitive"),
                () => Assert.AreEqual(true, TypeUtil.IsNullable(typeof(object)), "class"),
                () => Assert.AreEqual(true, TypeUtil.IsNullable(this.GetType()), "class"),
                () => Assert.AreEqual(true, TypeUtil.IsNullable(typeof(Type)), "Type (class)"),
                () => Assert.AreEqual(false, TypeUtil.IsNullable(typeof(System.UriKind)), "enum"),
                () => Assert.AreEqual(true, TypeUtil.IsNullable(typeof(System.UriKind?)), "nullable enum"),
                () => Assert.AreEqual(true, TypeUtil.IsNullable(typeof(System.IConvertible)), "interface"),
                () => Assert.AreEqual(false, TypeUtil.IsNullable(typeof(TestStruct)), "struct"),
                () => Assert.AreEqual(true, TypeUtil.IsNullable(typeof(TestStruct?)), "nullable struct"),
                () => Assert.AreEqual(false, TypeUtil.IsNullable(typeof(void)), "void (primitive)"),
                () => Assert.AreEqual(false, TypeUtil.IsNullable(typeof(System.Environment)), "static class"),
                () => Assert.AreEqual(false, TypeUtil.IsNullable(null), "null")
                );
        }

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

        [TestMethod]
        public void TypeNameOrVoid()
        {
            Multiple(
                () => Assert.AreEqual("System.Void", TypeUtil.TypeNameOrVoid(null)),   // typeof(void).ToString()
                () => Assert.AreEqual(GetType().ToString(), TypeUtil.TypeNameOrVoid(GetType())),
                () => Assert.AreEqual("System.Int32", TypeUtil.TypeNameOrVoid(typeof(int)))
                );
        }

        [TestMethod]
        public void GetTypeDefaultValue()
        {
            Multiple(
                () => Assert.AreEqual(0, TypeUtil.GetTypeDefaultValue(typeof(int))),
                () => Assert.AreEqual(null, TypeUtil.GetTypeDefaultValue(typeof(decimal?))),
                () => Assert.AreEqual(null, TypeUtil.GetTypeDefaultValue(typeof(object))),
                () => Assert.AreEqual(null, TypeUtil.GetTypeDefaultValue(typeof(System.Action)))
            );
        }

        public struct TestStruct
        {
            int Field1;
        }
    }
}
