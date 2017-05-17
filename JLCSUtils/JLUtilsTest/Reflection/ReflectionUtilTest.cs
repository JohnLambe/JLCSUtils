using JohnLambe.Util.Collections;
using JohnLambe.Util.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JohnLambe.Tests.JLUtilsTest.TestUtil;

namespace JohnLambe.Tests.JLUtilsTest.Reflection
{
    [TestClass]
    public class ReflectionUtilTest
    {
        protected ClassForTest CreateTestObject()
        {
            var x = new ClassForTest();
            x.Property1 = new ClassForTest();
            x.Property2 = 900;
            x.Property1.Property2 = 12345;
            x.Property1.Property1 = new ClassForTest();
            x.Property1.Property1.Property3 = "asdf";

            return x;
        }

        [TestMethod]
        public void GetPropertyValue()
        {
            // Arrange:
            var x = CreateTestObject();

            // Act / Assert:

            Assert.AreEqual(900, ReflectionUtil.TryGetPropertyValue<int>(x, "Property2"), "Failed on first level");
            Assert.AreEqual(12345, ReflectionUtil.TryGetPropertyValue<int>(x, "Property1.Property2"), "Failed on second level");
            Assert.AreEqual(4, ReflectionUtil.TryGetPropertyValue<int>(x, "Property1.Property1.Property3.Length"));
        }

        [TestMethod]
        public void GetPropertyValue_Nullability_PropertyDoesNotExist()
        {
            // Arrange:
            var x = CreateTestObject();

            // Act / Assert:

            // Nullable:
            Multiple(
                // Primitive value:

                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<int?>(x, "Property2.NonExistant"), "first level"),
                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<int?>(x, "Property2.NonExistant.NonExistant2"), "2nd level"),
                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<int?>(x, "Property2?.NonExistant.NonExistant2"), "3rd level"),
                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<int?>(x, "Property2?.NonExistant?.NonExistant2"), "3rd level"),

                // Non-nullable first part:
                () => AssertThrows<KeyNotFoundException>(() => ReflectionUtil.TryGetPropertyValue<int?>(x, "Property2!.NonExistant")),
                () => AssertThrows<KeyNotFoundException>(() => ReflectionUtil.TryGetPropertyValue<int?>(x, "Property2!.NonExistant.NonExistant2")),
                () => AssertThrows<KeyNotFoundException>(() => ReflectionUtil.TryGetPropertyValue<int?>(x, "Property2!.NonExistant?.NonExistant2")),

                () =>
                    AssertThrows<KeyNotFoundException>(() =>
                        ReflectionUtil.TryGetPropertyValue<int?>(x, "Property2@.NonExistant")),
                () =>
                AssertThrows<KeyNotFoundException>(() => ReflectionUtil.TryGetPropertyValue<int?>(x, "Property2@.NonExistant.NonExistant2")),

                // Object value:

                // Nullable:
                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1.NonExistant"), "first level"),
                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1.NonExistant.NonExistant2"), "2nd level"),
                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1?.NonExistant.NonExistant2"), "3rd level"),
                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1?.NonExistant?.NonExistant2"), "3rd level"),

                // Non-nullable first part:
                () => AssertThrows<KeyNotFoundException>(() => ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1!.NonExistant")),
                () => AssertThrows<KeyNotFoundException>(() => ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1!.NonExistant.NonExistant2")),
                () => AssertThrows<KeyNotFoundException>(() => ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1!.NonExistant?.NonExistant2")),

                () => AssertThrows<KeyNotFoundException>(() => ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1@.NonExistant")),
                () => AssertThrows<KeyNotFoundException>(() => ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1@.NonExistant.NonExistant2"))
            );
        }

        [TestMethod]
        public void GetPropertyValue_Nullability_Null()
        {
            // Arrange:
            var x = CreateTestObject();
            x.Property1 = null;

            // Act / Assert:

            Multiple(
                // Primitive value:

                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<object>(x, "Property1@.Property2"), "first level"),
                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<ClassForTest>(x, "Property1@.Property1@.NonExistant2"), "2nd level"),
                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1?.NonExistant!.NonExistant2"), "3rd level"),

                // These succeed because the non-nullable part is not reached:
                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1?.NonExistant@.NonExistant2"), "3rd level"),
                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1?.NonExistant.NonExistant2!"), "3rd level"),

                // Non-nullable first part: (similar to other tests in GetPropertyValue_Nullability_PropertyDoesNotExist, except this time it should throw NullReferenceException rather than KeyNotFoundException).
                () => AssertThrows<NullReferenceException>(() => ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1!.NonExistant")),
                () => AssertThrows<NullReferenceException>(() => ReflectionUtil.TryGetPropertyValue<int?>(x, "Property1!.NonExistant.NonExistant2")),

                () => Assert.AreEqual(null, ReflectionUtil.TryGetPropertyValue<object>(null, "property") )
            );

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

            ReflectionUtil.TrySetPropertyValue(x, "Property1.Property1.Property3", "new value");

            // Assert:

            Assert.AreEqual("new value", x.Property1.Property1.Property3);
        }

        #region ArrayOfTypes

        [TestMethod]
        public void ArrayOfTypes()
        {
            var result = ReflectionUtil.ArrayOfTypes(10, "a", null, new object(), DateTime.Now);
            Console.Out.WriteLine(CollectionUtil.CollectionToString(result));

            // Assert:
            Assert.AreEqual(CollectionUtil.CollectionToString(new Type[] { typeof(int), typeof(string), null, typeof(object), typeof(DateTime) }),
                CollectionUtil.CollectionToString(result));
        }

        [TestMethod]
        public void ArrayOfTypes_Null()
        {
            Assert.AreEqual(null, ReflectionUtil.ArrayOfTypes(null));
        }

        #endregion

        #region Call Method

        [TestMethod]
        public void CallStaticMethod()
        {
            Assert.AreEqual(1234, ReflectionUtil.CallStaticMethod<int>(GetType(), "TestStaticMethod", 1000, 234));
        }

        /// <summary>
        /// Call a static method, giving an instance target.
        /// </summary>
        [TestMethod]
        public void CallMethod_Static()
        {
            Assert.AreEqual(1234, ReflectionUtil.CallMethod<int>(new ReflectionUtilTest(), "TestStaticMethod", 1000, 234));
        }

        /// <summary>
        /// Call an instance method.
        /// </summary>
        [TestMethod]
        public void CallMethod()
        {
            Assert.AreEqual("str50", ReflectionUtil.CallMethod<string>(new ReflectionUtilTest(), "TestInstanceMethod", "str", 50));
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
            Assert.AreEqual(30, ReflectionUtil.CallStaticMethodVarArgs<int>(GetType(), "TestStaticMethod", 10, 20));
        }

        /// <summary>
        /// Call a static method, using the parameter default value for one parameter.
        /// </summary>
        [TestMethod]
        public void CallStaticMethodVarArgs_DefaultParam()
        {
            Assert.AreEqual(1010, ReflectionUtil.CallStaticMethodVarArgs<int>(GetType(), "TestStaticMethod", 10));  // only one argument supplied
        }

        [TestMethod]
        public void CallMethodVarArgs()
        {
            Assert.AreEqual("a100", ReflectionUtil.CallMethodVarArgs<string>(this, "TestInstanceMethod", "a", 100));
        }

        /// <summary>
        /// A parameter without a default value is not supplied.
        /// </summary>
        [TestMethod]
        public void CallMethodVarArgs_MissingParam()
        {
            TestUtil.AssertThrows(typeof(Exception),
                    () => ReflectionUtil.CallMethodVarArgs<string>(this, "TestInstanceMethod", "a"));
        }

        //TODO: Several cases not covered.

        #endregion

        [TestMethod]
        public void GetTypeHierarchy()
        {
            // Act:
            var result = ReflectionUtil.GetTypeHeirarchy(typeof(ClassForTest));
            Console.WriteLine(result.Count());
            
            // Assert:
            Assert.IsTrue(result.SequenceEqual(new Type[] { typeof(object), typeof(ClassForTestBase), typeof(ClassForTest) }));
        }

        [TestMethod]
        public void InstanceInvoke()
        {
            // This would be equivalent but gives a compilation error:
            //   Assert.AreEqual("Property2", nameof(default(ClassForTest).Property2);

            Assert.AreEqual("Property2", ReflectionUtil.InstanceInvoke<ClassForTest>(x => nameof(x.Property2)));
        }

    }

    public class ClassForTestBase
    {
    }
    public class ClassForTest : ClassForTestBase
    {
        public ClassForTest Property1 { get; set; }
        public int Property2 { get; set; }
        public virtual string Property3 { get; set; }  // it shouldn't make any difference whether the property is virtual.
    }
}
