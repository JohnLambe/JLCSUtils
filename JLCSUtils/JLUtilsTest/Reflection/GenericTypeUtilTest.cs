using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnLambe.Tests.JLUtilsTest.Reflection
{
    [TestClass]
    public class GenericTypeUtilTest
    {
        #region ChangeGenericParameterCount

        [TestMethod]
        public void ChangeGenericParameterCount()
        {
            // Act:
            var type = GenericTypeUtils.ChangeGenericParameterCount(typeof(TestGenericType<,,>), 4);
            var typeFromConcrete = GenericTypeUtils.ChangeGenericParameterCount(typeof(TestGenericType<int,string,object,float>), 3);
            var nonGenericType = GenericTypeUtils.ChangeGenericParameterCount(typeof(TestGenericType<,,>), 0);

            // Assert:
            Assert.AreEqual(typeof(TestGenericType<,,,>), type);
            Assert.AreEqual(typeof(TestGenericType<,,>), typeFromConcrete, "From concrete type");
            Assert.AreEqual(typeof(TestGenericType), nonGenericType);
        }

        /// <summary>
        /// Fails because the generic type with the required number of parameters does not exist.
        /// </summary>
        [TestMethod]
        public void ChangeGenericParameterCount_Fail()
        {
            TestUtil.AssertThrows(typeof(Exception),
                () => GenericTypeUtils.ChangeGenericParameterCount(typeof(TestGenericType<,,>), 2));
        }

        #endregion

        #region ChangeGenericParameters

        [TestMethod]
        public void ChangeGenericParameters()
        {
            // Act:
            var result = GenericTypeUtils.ChangeGenericParameters(typeof(TestGenericType<int,string,int>), typeof(object), typeof(DateTime), typeof(byte), typeof(GenericTypeUtilTest));

            // Assert:
            Assert.AreEqual(typeof(TestGenericType<object, DateTime, byte, GenericTypeUtilTest>), result);
        }

        [TestMethod]
        public void ChangeGenericParameters_FromNonGeneric()
        {
            // Act:
            var result = GenericTypeUtils.ChangeGenericParameters(typeof(TestGenericType), typeof(short), typeof(TestGenericType<int,int,int>), typeof(double));

            // Assert:
            Assert.AreEqual(typeof(TestGenericType<short, TestGenericType<int,int,int>, double>), result);
        }

        [TestMethod]
        public void ChangeGenericParameters_ToNonGeneric()
        {
            // Act:
            var result = GenericTypeUtils.ChangeGenericParameters(typeof(TestGenericType<long,bool,bool>));

            // Assert:
            Assert.AreEqual(typeof(TestGenericType), result);
        }

        #endregion

        #region Nullable

        [TestMethod]
        public void IsNullableValueType()
        {
            Assert.IsTrue(GenericTypeUtils.IsNullableValueType(typeof(int?)));
            Assert.IsTrue(GenericTypeUtils.IsNullableValueType(typeof(S1?)), "Nullable Struct");
            Assert.IsTrue(GenericTypeUtils.IsNullableValueType(typeof(Nullable<float>)));

            Type t = typeof(float);
            Assert.IsFalse(t.IsNullableValueType());

            Assert.IsFalse(GenericTypeUtils.IsNullableValueType(typeof(decimal)));
            Assert.IsFalse(GenericTypeUtils.IsNullableValueType(typeof(S1)), "Struct");
        }

        /// <summary>
        /// Should return false if the type is not a value type.
        /// </summary>
        [TestMethod]
        public void IsNullableValueType_NotValueType()
        {
            Assert.IsFalse(GenericTypeUtils.IsNullableValueType(typeof(object)));
            Assert.IsFalse(GetType().IsNullableValueType());

            Assert.IsFalse(typeof(System.EnvironmentVariableTarget).IsNullableValueType(), "Enum");
        }

        [TestMethod]
        public void MakeNullable()
        {
            Assert.AreEqual(typeof(long?), typeof(long).MakeNullableValueType());

            Type t = typeof(S1);
            Assert.AreEqual(typeof(S1?), t.MakeNullableValueType());

            Assert.AreEqual(typeof(System.EnvironmentVariableTarget?), typeof(System.EnvironmentVariableTarget).MakeNullableValueType(), "Enum");
        }

        [TestMethod]
        public void MakeNullable_Fail()
        {
            TestUtil.AssertThrows<Exception>(() => typeof(long?).MakeNullableValueType(), "Already nullable");
            TestUtil.AssertThrows<Exception>(() => typeof(object).MakeNullableValueType(), "Reference type");
        }

        [TestMethod]
        public void EnsureNullable()
        {
            Assert.AreEqual(typeof(bool?), typeof(bool).EnsureNullable(), "Non-nullable");
            Assert.AreEqual(typeof(char?), typeof(char?).EnsureNullable(), "Nullable value type");
            Assert.AreEqual(GetType(), GetType().EnsureNullable(), "Reference type");
        }

        #endregion

        #region Types for testing

        public class TestGenericType   // non-generic with same name as generic types
        {
        }
        public class TestGenericType<A, B, C>
        {
        }
        public class TestGenericType<A, B, C, D>
        {
        }

        public struct S1
        {
            public int X;
        }

        #endregion
    }

}
