using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DiExtension.AutoFactory;

namespace JohnLambe.Tests.JLUtilsTest.AutoFactory
{
    [TestClass]
    public class AutoFactoryGenericTest : AutoFactoryTestBase
    {
        /// <summary>
        /// Create factory and use it to create an instance,
        /// where the target type and the returned interface are generic.
        /// </summary>
        [TestMethod]
        public void CreateFactoryAndInstance_Generic()
        {
            // Arrange:
            const string TestParam = "param1";

            // Act:
            var factory = factoryFactory.CreateFactory<IFactory<ITestTargetClass<string>, string>>(typeof(TestGenericTargetClass<string, int>));
            var instance = factory.Create(TestParam);

            // Assert:
            Assert.AreEqual(typeof(TestGenericTargetClass<string,int>), instance.GetType(), "Created instance is of wrong type");
            Assert.AreEqual(TestParam, instance.Value1, "Created instance is wrongly populated");
        }
    }

    public interface ITestTargetClass<T>
    {
        T Value1 { get; }
    }

    public class TestGenericTargetClass<T1,T2> : ITestTargetClass<T1>
    {
        public TestGenericTargetClass(T1 value)
        {
            Value1 = value;
        }

        public T1 Value1 { get; private set; }
        public T2 Value2 { get; private set; }
    }

}
