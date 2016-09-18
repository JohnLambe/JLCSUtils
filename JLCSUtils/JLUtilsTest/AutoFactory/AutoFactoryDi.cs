using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Microsoft.Practices.Unity;

using JohnLambe.Util.DependencyInjection.AutoFactory;
using JohnLambe.Util.DependencyInjection.AutoFactory.Unity;

namespace JohnLambe.Tests.JLUtilsTest.AutoFactory
{
    [TestClass]
    public class AutoFactoryDi
    {
        [TestInitialize]
        public void SetupDiContainer()
        {
            Container = new UnityContainer();

        }

        public UnityContainer Container { get; protected set; }

        public TestClassForDi TestSubjectInstance = new TestClassForDi();  // class to run DI on

        [TestMethod]
        public void NonAutoInstanceRegistration()
        {
            // Arrange:
            var factory = new AutoFactoryFactory().CreateFactory<IFactory<ITestTargetClass,string>>(typeof(TestTargetClass));
            Container.RegisterInstance(typeof(IFactory<ITestTargetClass,string>), factory);

            // Act:
            Container.BuildUp(TestSubjectInstance);

            Console.WriteLine(TestSubjectInstance.TestProperty);

            // Assert:
            Assert.AreNotEqual(null, TestSubjectInstance.TestProperty, "Property was not injected");
            Assert.AreEqual(factory, TestSubjectInstance.TestProperty, "Wrong value injected");
//            Assert.AreEqual(typeof(TestTargetClass), instance.TestProperty.GetType(), "Wrong type injected");
        }

/*
        [TestMethod]
        public void NonAutoTypeRegistration()
        {
            // Arrange:
            var factory = new AutoFactoryFactory().CreateFactory<IFactory<TestTargetClass, string>>(typeof(TestTargetClass));
            Container.RegisterType(typeof(IFactory<TestTargetClass, string>), typeof(AutoFactory<TestTargetClass, string>));
                     // also needs the parameter to the factory constructor - the target type

            // Act:
            var instance = new TestClassForDi();

            Container.BuildUp(instance);

            Console.WriteLine(instance.TestProperty);

            // Assert:
            Assert.AreNotEqual(null, instance.TestProperty, "Property was not injected");
            Assert.AreEqual(typeof(AutoFactory<TestTargetClass, string>), instance.TestProperty.GetType(), "Wrong type injected");
        }
*/
        /// <summary>
        /// Set up a mapping for the factory's return type,
        /// and test generation of the factory and injection of the factory.
        /// No name is used in the registration.
        /// </summary>
        [TestMethod]
        public void AutoByTypeMapping()
        {
            // Arrange:
            {
                Container.AddExtension(new AutoFactoryUnityExtension());    // register our extension
                Container.RegisterType(typeof(ITestTargetClass), typeof(TestTargetClass));  // map the return type to a concrete class

                //            var factory = new AutoFactoryFactory().CreateFactory<IFactory<TestTargetClass, string>>(typeof(TestTargetClass));
                //            Container.RegisterInstance(typeof(IFactory<TestTargetClass, string>), factory);
            }

            // Act:
            {
                Container.BuildUp(TestSubjectInstance);

                Console.WriteLine(TestSubjectInstance.TestProperty);
            }

            // Assert:
            {
                Assert.AreNotEqual(null, TestSubjectInstance.TestProperty, "Property was not injected");
                Assert.AreEqual(typeof(AutoFactory<ITestTargetClass, string>), TestSubjectInstance.TestProperty.GetType(), "Wrong type injected");

                // invoke injected factory:
                Assert.IsTrue(TestSubjectInstance.TestProperty.Create("arg") is TestTargetClass, "Injected factory returned wrong type");
                Assert.AreEqual("arg2", TestSubjectInstance.TestProperty.Create("arg2").Value, "Injected factory returned wrongly populated object");
            }
        }

        /// <summary>
        /// Register an instance with the factory's return type,
        /// and test generation of the factory.
        /// No name is used in the registration.
        /// </summary>
        [TestMethod]
        public void AutoReturnRegisteredInstance()
        {
            // Arrange:
            Container.AddExtension(new AutoFactoryUnityExtension());    // register our extension

            var registeredInstance = new TestTargetClass("registered instance");
            Container.RegisterInstance(typeof(ITestTargetClass), registeredInstance);  // register an instance for this interface

            // Act:
            Container.BuildUp(TestSubjectInstance);

            Console.WriteLine(TestSubjectInstance.TestProperty);

            // Assert:
            Assert.AreNotEqual(null, TestSubjectInstance.TestProperty, "Property was not injected");
            Assert.AreEqual(typeof(AutoFactory<ITestTargetClass, string>), TestSubjectInstance.TestProperty.GetType(), "Wrong type injected");

            Assert.AreEqual(registeredInstance, TestSubjectInstance.TestProperty.Create("a"), "Injected factory returned wrong instance");  // invoke the factory to test that it is correctly configured
        }
    }

    public class TestClassForDi
    {
        [Dependency]
        public IFactory<ITestTargetClass,string> TestProperty { get; set; }
    }
}
