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
    public class GenericTypeUtilsTest
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
            var result = GenericTypeUtils.ChangeGenericParameters(typeof(TestGenericType<int,string,int>), typeof(object), typeof(DateTime), typeof(byte), typeof(GenericTypeUtilsTest));

            // Assert:
            Assert.AreEqual(typeof(TestGenericType<object, DateTime, byte, GenericTypeUtilsTest>), result);
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

        #region Types for testing

        public class TestGenericType   // non-generic
        {
        }
        public class TestGenericType<A, B, C>
        {
        }
        public class TestGenericType<A, B, C, D>
        {
        }

        #endregion
    }

}
