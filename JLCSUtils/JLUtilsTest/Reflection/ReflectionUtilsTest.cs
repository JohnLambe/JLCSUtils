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

        #region ArrayOfTypes

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

        #endregion

        #region Call Method

        [TestMethod]
        public void CallStaticMethod()
        {
            Assert.AreEqual(1234, ReflectionUtils.CallStaticMethod<int>(GetType(), "TestStaticMethod", 1000, 234));
        }

        /// <summary>
        /// Call a static method, giving an instance target.
        /// </summary>
        [TestMethod]
        public void CallMethod_Static()
        {
            Assert.AreEqual(1234, ReflectionUtils.CallMethod<int>(new ReflectionUtilsTest(), "TestStaticMethod", 1000, 234));
        }

        /// <summary>
        /// Call an instance method.
        /// </summary>
        [TestMethod]
        public void CallMethod()
        {
            Assert.AreEqual("str50", ReflectionUtils.CallMethod<string>(new ReflectionUtilsTest(), "TestInstanceMethod", "str", 50));
        }

        public static int TestStaticMethod(int x, int y = 1000)
        {
            return x + y;
        }

        public string TestInstanceMethod(string x, int y)
        {
            return x + y;
        }

        #endregion

        #region CallMethodVarArgs

        [TestMethod]
        public void CallStaticMethodVarArgs()
        {
            Assert.AreEqual(30, ReflectionUtils.CallStaticMethodVarArgs<int>(GetType(), "TestStaticMethod", 10, 20));
        }

        /// <summary>
        /// Call a static method, using the parameter default value for one parameter.
        /// </summary>
        [TestMethod]
        public void CallStaticMethodVarArgs_DefaultParam()
        {
            Assert.AreEqual(1010, ReflectionUtils.CallStaticMethodVarArgs<int>(GetType(), "TestStaticMethod", 10));  // only one argument supplied
        }

        [TestMethod]
        public void CallMethodVarArgs()
        {
            Assert.AreEqual("a100", ReflectionUtils.CallMethodVarArgs<string>(this, "TestInstanceMethod", "a", 100));
        }

        /// <summary>
        /// A parameter without a default value is not supplied.
        /// </summary>
        [TestMethod]
        public void CallMethodVarArgs_MissingParam()
        {
            TestUtil.AssertThrows(typeof(Exception),
                    () => ReflectionUtils.CallMethodVarArgs<string>(this, "TestInstanceMethod", "a"));
        }

        //TODO: Several cases not covered.

        #endregion
    }

    public class ClassForTest
    {
        public ClassForTest Property1 { get; set; }
        public int Property2 { get; set; }
        public string Property3 { get; set; }
    }
}
