using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.Reflection
{
    [TestClass]
    public class GenericTypeUtilTest
    {
        #region ChangeGenericParameterCount

        [TestMethod]
        public void ChangeGenericParameterCount()
        {
            Multiple(
                () =>
                {
                    // Act:
                    var type = GenericTypeUtil.ChangeGenericParameterCount(typeof(TestGenericType<,,>), 4);
                    // Assert:
                    Assert.AreEqual(typeof(TestGenericType<,,,>), type);
                },

                () =>
                {
                    // Act:
                    var typeFromConcrete = GenericTypeUtil.ChangeGenericParameterCount(typeof(TestGenericType<int, string, object, float>), 3);
                    // Assert:
                    Assert.AreEqual(typeof(TestGenericType<,,>), typeFromConcrete, "From concrete type");
                },

                () =>
                {
                    // Act:
                    var nonGenericType = GenericTypeUtil.ChangeGenericParameterCount(typeof(TestGenericType<,,>), 0);
                    // Assert:
                    Assert.AreEqual(typeof(TestGenericType), nonGenericType);
                }
            );
        }

        /// <summary>
        /// Fails because the generic type with the required number of parameters does not exist.
        /// </summary>
        [TestMethod]
        public void ChangeGenericParameterCount_Fail()
        {
            AssertThrows(typeof(Exception),
                () => GenericTypeUtil.ChangeGenericParameterCount(typeof(TestGenericType<,,>), 2));
        }

        #endregion

        #region ChangeGenericParameters

        [TestMethod]
        public void ChangeGenericParameters()
        {
            // Act:
            var result = GenericTypeUtil.ChangeGenericParameters(typeof(TestGenericType<int,string,int>), typeof(object), typeof(DateTime), typeof(byte), typeof(GenericTypeUtilTest));

            // Assert:
            Assert.AreEqual(typeof(TestGenericType<object, DateTime, byte, GenericTypeUtilTest>), result);
        }

        [TestMethod]
        public void ChangeGenericParameters_FromNonGeneric()
        {
            // Act:
            var result = GenericTypeUtil.ChangeGenericParameters(typeof(TestGenericType), typeof(short), typeof(TestGenericType<int,int,int>), typeof(double));

            // Assert:
            Assert.AreEqual(typeof(TestGenericType<short, TestGenericType<int,int,int>, double>), result);
        }

        [TestMethod]
        public void ChangeGenericParameters_ToNonGeneric()
        {
            // Act:
            var result = GenericTypeUtil.ChangeGenericParameters(typeof(TestGenericType<long,bool,bool>));

            // Assert:
            Assert.AreEqual(typeof(TestGenericType), result);
        }

        #endregion

        #region Nullable

        [TestMethod]
        public void IsNullableValueType()
        {
            Multiple(
                () => Assert.IsTrue(GenericTypeUtil.IsNullableValueType(typeof(int?))),
                () => Assert.IsTrue(GenericTypeUtil.IsNullableValueType(typeof(S1?)), "Nullable Struct"),
                () => Assert.IsTrue(GenericTypeUtil.IsNullableValueType(typeof(Nullable<float>))),

                () =>
                {
                    Type t = typeof(float);
                    Assert.IsFalse(t.IsNullableValueType());
                },

                () => Assert.IsFalse(GenericTypeUtil.IsNullableValueType(typeof(decimal))),
                () => Assert.IsFalse(GenericTypeUtil.IsNullableValueType(typeof(S1)), "Struct")
            );
        }

        /// <summary>
        /// Should return false if the type is not a value type.
        /// </summary>
        [TestMethod]
        public void IsNullableValueType_NotValueType()
        {
            Multiple(
                () => Assert.IsFalse(GenericTypeUtil.IsNullableValueType(typeof(object))),
                () => Assert.IsFalse(GetType().IsNullableValueType()),

                () => Assert.IsFalse(typeof(System.EnvironmentVariableTarget).IsNullableValueType(), "Enum")
            );
        }

        [TestMethod]
        public void MakeNullable()
        {
            Multiple(
                () => Assert.AreEqual(typeof(long?), typeof(long).MakeNullableValueType()),

                () =>
                {
                    Type t = typeof(S1);
                    Assert.AreEqual(typeof(S1?), t.MakeNullableValueType());
                },

                () => Assert.AreEqual(typeof(System.EnvironmentVariableTarget?), typeof(System.EnvironmentVariableTarget).MakeNullableValueType(), "Enum")
            );
        }

        [TestMethod]
        public void MakeNullable_Fail()
        {
            Multiple(
                () => TestUtil.AssertThrows<Exception>(() => typeof(long?).MakeNullableValueType(), "Already nullable"),
                () => TestUtil.AssertThrows<Exception>(() => typeof(object).MakeNullableValueType(), "Reference type")
            );
        }

        [TestMethod]
        public void EnsureNullable()
        {
            Multiple(
                () => Assert.AreEqual(typeof(bool?), typeof(bool).EnsureNullable(), "Non-nullable"),
                () => Assert.AreEqual(typeof(char?), typeof(char?).EnsureNullable(), "Nullable value type"),
                () => Assert.AreEqual(GetType(), GetType().EnsureNullable(), "Reference type")
            );
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
