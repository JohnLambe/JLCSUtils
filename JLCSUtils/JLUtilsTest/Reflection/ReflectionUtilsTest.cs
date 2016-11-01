using JohnLambe.Util.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.Reflection
{
    [TestClass]
    public class ReflectionUtilsTest
    {
        [TestMethod]
        public void GetPropertyValue()
        {
            // Arrange:

            var x = new ClassForTest();
            x.Property1 = new ClassForTest();
            x.Property2 = 900;
            x.Property1.Property2 = 12345;
            x.Property1.Property1 = new ClassForTest();
            x.Property1.Property1.Property3 = "asdf";

            // Act / Assert:

            Assert.AreEqual(900, ReflectionUtils.TryGetPropertyValue<int>(x, "Property2"), "Failed on first level");
            Assert.AreEqual(12345, ReflectionUtils.TryGetPropertyValue<int>(x, "Property1.Property2"), "Failed on second level");
            Assert.AreEqual(4, ReflectionUtils.TryGetPropertyValue<int>(x, "Property1.Property1.Property3.Length"));
        }

        [TestMethod]
        public void SetPropertyValue()
        {
            // Arrange:

            var x = new ClassForTest();
            x.Property1 = new ClassForTest();
            x.Property2 = 900;
            x.Property1.Property2 = 12345;
            x.Property1.Property1 = new ClassForTest();
            x.Property1.Property1.Property3 = "asdf";

            // Act:

            ReflectionUtils.TrySetPropertyValue(x, "Property1.Property1.Property3", "new value");

            // Assert:

            Assert.AreEqual("new value", x.Property1.Property1.Property3);
        }


    }

    public class ClassForTest
    {
        public ClassForTest Property1 { get; set; }
        public int Property2 { get; set; }
        public string Property3 { get; set; }
    }
}
