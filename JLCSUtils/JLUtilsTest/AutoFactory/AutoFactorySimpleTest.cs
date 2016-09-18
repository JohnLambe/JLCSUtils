using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.DependencyInjection.AutoFactory;

namespace JohnLambe.Tests.JLUtilsTest.AutoFactory
{
    [TestClass]
    public partial class AutoFactorySimpleTest : AutoFactoryTestBase
    {
        public IFactory<ITestTargetClass,string> CreateFactory()
        {
            var result = factoryFactory.CreateFactory<IFactory<ITestTargetClass, string>>(typeof(TestTargetClass));
            Console.WriteLine("Factory created");
            return result;
        }

        /// <summary>
        /// Create a factory.
        /// </summary>
        [TestMethod]
        public void CreateFactoryTest()
        {
            // Arrange:

            // Act:
            var factory = CreateFactory();

            // Assert:
            Assert.AreEqual(typeof(AutoFactory<ITestTargetClass,string>), factory.GetType(), "Created factory is of wrong type");
        }

        /// <summary>
        /// Create an instance from a factory created with AutoFactory.
        /// </summary>
        [TestMethod]
        public void CreateInstance()
        {
            // Arrange:
            const string TestParam = "test";
            var factory = CreateFactory();

            // Act:
            var instance = factory.Create(TestParam);

            // Assert:
            Assert.AreEqual(typeof(TestTargetClass), instance.GetType(), "Created instance is of wrong type");
            Assert.AreEqual(TestParam, instance.Value, "Created instance is wrongly populated");
        }

    }

    public interface ITestTargetClass
    {
        string Value { get; }
    }

    public class TestTargetClass : ITestTargetClass
    {
        public TestTargetClass(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}
