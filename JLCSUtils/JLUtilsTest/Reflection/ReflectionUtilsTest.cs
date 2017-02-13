using JohnLambe.Util.Collections;
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

        [TestMethod]
        public void ArrayOfTypes()
        {
            var result = ReflectionUtils.ArrayOfTypes(10, "a", null, new object(), DateTime.Now);
            Console.Out.WriteLine(CollectionUtils.CollectionToString(result));

            // Assert:
            Assert.AreEqual(CollectionUtils.CollectionToString(new Type[] { typeof(int), typeof(string), null, typeof(object), typeof(DateTime) }),
                CollectionUtils.CollectionToString(result));
        }

        [TestMethod]
        public void ArrayOfTypes_Null()
        {
            Assert.AreEqual(null, ReflectionUtils.ArrayOfTypes(null));
        }

    }

    public class ClassForTest
    {
        public ClassForTest Property1 { get; set; }
        public int Property2 { get; set; }
        public string Property3 { get; set; }
    }
}
