using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.DependencyInjection.AutoFactory;
using System.Collections.Generic;

// Test with a class with multiple constructors, and different numbers of arguments.

namespace JohnLambe.Tests.JLUtilsTest.AutoFactory
{
    [TestClass]
    public partial class AutoFactoryTest2 : AutoFactoryTestBase
    {
        /// <summary>
        /// Create an instance from a factory created with AutoFactory, using a default constructor.
        /// </summary>
        [TestMethod]
        public void CreateInstance_NoParams()
        {
            // Arrange:
            var factory = factoryFactory.CreateFactory<IFactory<ITestTargetClass2>>(typeof(TestTargetClass2));

            // Act:
            var instance = factory.Create();

            // Assert:
            Assert.AreEqual(typeof(TestTargetClass2), instance.GetType(), "Created instance is of wrong type");
        }

        /// <summary>
        /// Create an instance from a factory created with AutoFactory.
        /// </summary>
        [TestMethod]
        public void CreateInstance_2Params()
        {
            // Arrange:
            const string TestParam = "asdfg";
            const int TestParam2 = 10;
            var factory = factoryFactory.CreateFactory<IFactory<ITestTargetClass2, string, int>>(typeof(TestTargetClass2));

            // Act:
            var instance = factory.Create(TestParam, TestParam2);

            // Assert:
            Assert.AreEqual(typeof(TestTargetClass2), instance.GetType(), "Created instance is of wrong type");
            Assert.AreEqual(TestParam + "[" + TestParam2 + "]", instance.Value, "Created instance is wrongly populated");
            Assert.AreEqual(5, instance.Method(), "Wrong return value from method");
        }

        /// <summary>
        /// Create a factory, then an instance from it, with 3 constructor arguments including a generic type.
        /// </summary>
        [TestMethod]
        public void CreateInstance_3Params_Generic()
        {
            // Arrange:
            const string TestParam = "Param1";
            const int TestParam2 = 200;
            IList<int> TestParam3 = new List<int>() { 2,4,6 };

            var factory = factoryFactory.CreateFactory<IFactory<ITestTargetClass2, string, int, IList<int>>>(typeof(TestTargetClass2));

            // Act:
            var instance = factory.Create(TestParam, TestParam2, TestParam3);

            // Assert:
            Assert.AreEqual(typeof(TestTargetClass2), instance.GetType(), "Created instance is of wrong type");
            Assert.AreEqual(TestParam + " " + TestParam2 + " " + TestParam3, instance.Value, "Created instance is wrongly populated");
        }

        public ITestTargetClass2 Internal_4Params(string a, string b, string c, object d)
        {
            // Arrange:
            var factory = factoryFactory.CreateFactory<IFactory<ITestTargetClass2, string, string, string, object>>(typeof(TestTargetClass2));

            // Act:
            var instance = factory.Create(a,b,c,d);

            return instance;
        }

        [TestMethod]
        public void CreateInstance_4Params()
        {
            ITestTargetClass2 instance = Internal_4Params("one", "two", "three", "four");

            // Assert:
            Assert.AreEqual(typeof(TestTargetClass2), instance.GetType(), "Created instance is of wrong type");
            Assert.AreEqual("one two three four".ToUpperInvariant(), instance.Value, "Created instance is wrongly populated");
        }

        /// <summary>
        /// The invoked constructor throws an exception.
        /// </summary>
        [TestMethod]
        public void CreateInstance_Exception()
        {
            TestUtil.AssertThrows(typeof(NullReferenceException),
                () => Internal_4Params("1", "2", null, "4")
                );
        }

        [TestMethod]
        public void CreateInstance_5Params()
        {
            // Arrange:
            var factory = factoryFactory.CreateFactory <IFactory<ITestTargetClass2, TestTargetClass2, double, bool?, object, int>>(typeof(TestTargetClass2));

            // Act:
            var instance = factory.Create(new TestTargetClass2(), 5.1, true, null, 4);

            // Assert:
            Assert.AreEqual(typeof(TestTargetClass2), instance.GetType(), "Created instance is of wrong type");
            Assert.AreEqual("Default 5.1 True  4", instance.Value, "Created instance is wrongly populated");
        }

        /// <summary>
        /// Check that the correct exception is given on trying to create a factory with parameter types
        /// not supported by the target class.
        /// </summary>
        [TestMethod]
        public void Fail_InvalidArguments()
        {
            TestUtil.AssertThrowsContains(
                typeof(ArgumentException),
                "does not have a constructor",
                () => factoryFactory.CreateFactory<IFactory<ITestTargetClass2, int, int>>(typeof(TestTargetClass2))
                );
        }

        /// <summary>
        /// Check that the correct exception is given on trying to create a factory with a null target class.
        /// </summary>
        [TestMethod]
        public void Fail_NullTarget()
        {
            TestUtil.AssertThrows(
                typeof(ArgumentNullException),
                () => factoryFactory.CreateFactory<IFactory<ITestTargetClass2, int, int>>(null)
                );
        }
    }

    public interface ITestTargetClass2
    {
        string Value { get; }
        int Method();
    }

    public class TestTargetClass2 : ITestTargetClass2
    {
        public TestTargetClass2()
        {
            Value = "Default";
        }

        // Two two-parameter constructors (to test that the correct one is called):

        public TestTargetClass2(string value, float b)
        {
            Value = value;
        }

        public TestTargetClass2(string value, int b)
        {
            Value = value + "[" + b + "]";
        }

        public TestTargetClass2(string value, int b, IList<int> c)
        {
            Value = value + " " + b + " " + c;
        }

        /// <summary>
        /// Fails if passed a null value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public TestTargetClass2(string value, string b, string c, object d)
        {
            Value = value.ToUpperInvariant() + " " + b.ToUpperInvariant() + " " + c.ToUpperInvariant() + " " + d.ToString().ToUpperInvariant();
        }

        public TestTargetClass2(TestTargetClass2 value, double b, bool? c, object d, int e)
        {
            Value = value + " " + b + " " + c + " " + d + " " + e;
        }

        public string Value { get; private set; }

        public int Method()
        {
            return 5;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
