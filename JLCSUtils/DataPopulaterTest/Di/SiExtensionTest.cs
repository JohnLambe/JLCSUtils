using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiExtension;
using DiExtension.SimpleInject;

namespace JohnLambe.Tests.JLUtilsTest.Di
{
    [TestClass]
    public class SiExtensionTest
    {
        [TestMethod]
        public void SiExtensionTest1()
        {
            //TODO: Separate into individual tests.

            // Arrange:

            Setup();


            // Act:

            TestInjectedObject1 instance = _context.Container.GetInstance<TestInjectedObject1>();

            // Assert:

            Assert.AreEqual("(Global Value)", instance.GlobalValue1);
            Assert.AreEqual("(Global Value 2)", instance.GlobalValue2);

            // Non-injected properties:
            Assert.AreEqual(TestInjectedObject1.OptionalDependencyDefaultValue, instance.UnresolvedOptionalDependency);
            Assert.AreEqual(null, instance.UnresolvedOptionalNullableDependency);
            Assert.AreEqual(null, instance.NotInjected);

        }

        [TestMethod]
        public void InjectRegisteredObject()
        {
            // Arrange:

            Setup();


            // Act:

            TestInjectedObject1 instance = _context.Container.GetInstance<TestInjectedObject1>();

            // Assert:

            Assert.AreEqual(_registeredObject, instance.RegisteredObject);

            Assert.AreEqual(500, instance.ResolvedOptionalDependency);

        }


        [TestMethod]
        public void RegisterSingleton()
        {
            // Arrange:
            var expectedInstance = new TestRegisteredObject1() { Property1 = "test" };
            _context.RegisterInstance(expectedInstance);

            // Act:
            var resolvedInstance = _context.Container.GetInstance<TestRegisteredObject1>();

            // Assert:
            Assert.AreEqual(expectedInstance, resolvedInstance);
            Assert.AreEqual("test", resolvedInstance.Property1);
        }

        [TestMethod]
        public void RegisterSingleton_WrongCompileTimeType()
        {
            // Arrange:
            object expectedInstance = new TestRegisteredObject1() { Property1 = "test" };
            _context.RegisterInstance(expectedInstance);

            // Act:
            var resolvedInstance = _context.Container.GetInstance<TestRegisteredObject1>();
            // Does not return the registered instance because it is registered for Type object.
            // Returns a new instance of TestRegisteredObject1.

            // Assert:
            Assert.AreNotEqual(expectedInstance, resolvedInstance);
            Assert.AreNotEqual("test", resolvedInstance.Property1);
        }

        [TestMethod]
        public void RegisterInstanceByRuntimeType()
        {
            // Arrange:
            object expectedInstance = new TestRegisteredObject1() { Property1 = "test" };
            _context.RegisterInstanceByRuntimeType(expectedInstance);

            // Act:
            var resolvedInstance = _context.Container.GetInstance<TestRegisteredObject1>();

            // Assert:
            Assert.AreEqual(expectedInstance, resolvedInstance);
            Assert.AreEqual("test", resolvedInstance.Property1);
        }

        public void Setup()
        {
            _context.RegisterInstance("GlobalValue", "(Global Value)");
            _context.RegisterInstance("GlobalValue2", "(Global Value 2)");
            _context.RegisterInstance("ResolvedOptionalDependency", 500);
            _context.RegisterInstance("RegisteredObject", _registeredObject);

            _context.Container.Register(typeof(TestInjectedObject1));
            _context.Container.Register(typeof(TestRegisteredObject1));
        }

        protected SiExtendedDiContext _context = new SiExtendedDiContext(new SimpleInjector.Container());
        protected TestRegisteredObject1 _registeredObject = new TestRegisteredObject1();

    }
}
