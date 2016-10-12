using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.DependencyInjection.SimpleInject;
using JohnLambe.Util.DependencyInjection.Attributes;

namespace JohnLambe.Tests.JLUtilsTest.Di
{
    [TestClass]
    public class ParameterInjectionTest
    {
        //TODO:
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
}
