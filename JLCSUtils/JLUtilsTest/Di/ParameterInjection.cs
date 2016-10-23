using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiExtension.SimpleInject;
using DiExtension.Attributes;

namespace JohnLambe.Tests.JLUtilsTest.Di
{
    [TestClass]
    public class ParameterInjectionTest
    {
        //TODO:
        /// <summary>
        /// ConfigInject value injected into primitive contructor parameter.
        /// </summary>
        [TestMethod]
        public void ParameterInjection()
        {
            // Arrange:

            _context.Container.Register(typeof(TestClassForParameterInjection));
            _context.RegisterInstance("param1", 100);

            // Act:

            TestClassForParameterInjection instance = _context.Container.GetInstance<TestClassForParameterInjection>();

            // Assert:
            Assert.AreEqual(100, instance.Param1);
        }

        /// <summary>
        /// ConfigInject value injected into non-primitive contructor parameter.
        /// </summary>
        [TestMethod]
        public void ParameterObjectInjection()
        {
            // Arrange:

            var expectedInstance = new TestInjectedObjectClass1();

            _context.Container.Register(typeof(TestClassForParameterObjectInjection));
            _context.RegisterInstance("param1", expectedInstance);

            // Act:

            var instance = _context.Container.GetInstance<TestClassForParameterObjectInjection>();

            // Assert:
            Assert.AreEqual(expectedInstance, instance.Param1);
        }

        protected SiExtendedDiContext _context = new SiExtendedDiContext(new SimpleInjector.Container());
    }


    public class TestClassForParameterInjection
    {
        public TestClassForParameterInjection([Inject(InjectAttribute.CodeName)] int param1)
        {
            Console.WriteLine("TestClassForParameterInjection ctor parameters: " + param1);
            Param1 = param1;
        }

        public readonly int Param1;
    }

    public class TestClassForParameterObjectInjection
    {
        public TestClassForParameterObjectInjection([Inject(InjectAttribute.CodeName)] TestInjectedObjectClass1 param1)
        {
            Console.WriteLine("TestClassForParameterInjection ctor parameters: " + param1);
            Param1 = param1;
        }

        public readonly TestInjectedObjectClass1 Param1;
    }

    public class TestInjectedObjectClass1
    {
        public static int counter = 0;

        protected readonly int _instance = counter++;

        public override string ToString()
        {
            return base.ToString() + "#" + _instance;
        }
    }
}
