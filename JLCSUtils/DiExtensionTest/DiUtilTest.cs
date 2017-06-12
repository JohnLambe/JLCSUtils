using DiExtension.SimpleInject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiExtension;
using DiExtension.Attributes;

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


    [TestClass]
    public class DiUtilTest2
    {
        [TestMethod]
        public void CallMethod()
        {
            // Arrange:
            context.RegisterInstance("A", 1234);
            context.RegisterInstance("Parameter2", "<b>");

            // Act:
            var result = DiUtil.CallMethod<string>(context, GetType().GetMethod("TestMethod"), this);

            // Assert:
            Assert.AreEqual("1234<b>", result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="b">The first letter is capitalised for the key.</param>
        /// <returns></returns>
        public virtual string TestMethod([Inject("A")] int x, [InjectByName] string parameter2)
        {
            return x + parameter2;
        }

        protected SiExtendedDiContext context = new SiExtendedDiContext();
    }
}
