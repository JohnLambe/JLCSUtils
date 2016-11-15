using DiExtension.Attributes;
using DiExtension.SimpleInject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.Di
{
    [TestClass]
    public class ConstructorResolutionTest
    {
        [TestMethod]
        public void ChooseConstructorByAttribute()
        {
            // Arrange:
            _context.RegisterInstance("param", "value");

            // Act:
            var instance = _context.GetInstance<TestClassForConstructorResolution>();

            // Assert:
            Assert.AreEqual("string:value", instance.Value);
        }

        protected SiExtendedDiContext _context = new SiExtendedDiContext(new SimpleInjector.Container());
    }

    public class TestClassForConstructorResolution
    {
        public TestClassForConstructorResolution()
        {
            Value = "";
        }

        public TestClassForConstructorResolution(int x)
        {
            Value = "int";
        }

        [Inject]
        public TestClassForConstructorResolution([Inject("param")]string x)
        {
            Value = "string:" + x;
        }

        public TestClassForConstructorResolution(int x, int y)
        {
            Value = "int,int";
        }

        public string Value { get; protected set; }
    }
}
