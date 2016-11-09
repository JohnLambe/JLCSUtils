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
        [TestMethod]
        public void ChangeGenericParameterCount()
        {
            // Act:
            var type = GenericTypeUtils.ChangeGenericParameterCount(typeof(TestGenericType<,,>), 4);
            var typeFromConcrete = GenericTypeUtils.ChangeGenericParameterCount(typeof(TestGenericType<int,string,object,float>), 3);

            // Assert:
            Assert.AreEqual(typeof(TestGenericType<,,,>), type);
            Assert.AreEqual(typeof(TestGenericType<,,>), typeFromConcrete, "From concrete type");
        }

        [TestMethod]
        public void ChangeGenericParameterCount_Fail()
        {
            TestUtil.AssertThrows(typeof(Exception),
                () => GenericTypeUtils.ChangeGenericParameterCount(typeof(TestGenericType<,,>), 2));
        }
    }

    public class TestGenericType<A,B,C>
    {
    }
    public class TestGenericType<A, B, C, D>
    {
    }
}
