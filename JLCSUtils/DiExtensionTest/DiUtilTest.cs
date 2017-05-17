using DiExtension.SimpleInject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiExtension;

namespace Test.DiExtensionTest
{
    [TestClass]
    public class DiUtilTest
    {
        [TestMethod]
        public void RegisterProperties()
        {
            // Arrange:
            ClassForTest c = new ClassForTest();

            // Act:
            context.RegisterProperties(c);

            // Assert:
            Assert.AreEqual(100, context.GetInstance<ClassToBeRegistered>().Value);
        }

        public class ClassForTest
        {
            public string Property1 => "test";

            public ClassToBeRegistered Property2 => new ClassToBeRegistered() { Value = 100 };
        }

        public class ClassToBeRegistered
        {
            public int Value { get; set; }
        }

        protected SiDiContext context = new SiExtendedDiContext();
    }
}
