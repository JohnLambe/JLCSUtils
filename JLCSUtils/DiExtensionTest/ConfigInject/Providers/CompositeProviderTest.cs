using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DiExtension.ConfigInject.Providers;

namespace Test.DiExtensionTest.ConfigInject.Providers
{
    [TestClass]
    public class CompositeProviderTest
    {
        [TestMethod]
        public void ThreeLevel()
        {
            // Arrange:
            var obj = new TestClass()
            {
                Property1 = new TestClass()
                {
                    Property2 = new TestClass()
                    {
                        Property3 = 1234
                    }
                }
            };
            Provider = new CompositeProvider(new ObjectValueProvider(obj));

            // Act:
            int value;
            var result = Provider.GetValue<int>("Property1.Property2.Property3", typeof(int), out value);

            // Assert:
            Assert.IsTrue(result);
            Assert.AreEqual(1234,value);
        }

        [TestMethod]
        public void FailsOnLevel1()
        {
            // Arrange:
            var obj = new TestClass()
            {
                Property1 = new TestClass()
                {
                    Property2 = new TestClass()
                    {
                        Property3 = 1234
                    }
                }
            };
            Provider = new CompositeProvider(new ObjectValueProvider(obj));

            // Act:
            int value;
            var result = Provider.GetValue<int>("Property1.Property1.Property3", typeof(int), out value);

            // Assert:
            Assert.IsFalse(result);
            Assert.AreEqual(0, value);
        }

        [TestMethod]
        public void MultipleObjTypes()
        {
            // Arrange:
            var obj = new TestClass()
            {
                Property1 = new TestClass()
                {
                    Property4 = new TestClass2()
                    {
                        A = new TestClass()
                        {
                            Property2 = "test"
                        }
                    }
                }
            };
            Provider = new CompositeProvider(new ObjectValueProvider(obj));

            // Act:
            string value;
            var result = Provider.GetValue<string>("Property1.Property4.A.Property2", typeof(string), out value);

            // Assert:
            Assert.IsTrue(result);
            Assert.AreEqual("test", value);
        }

        [TestMethod]
        public void NonObjProperty()
        {
            // Arrange:
            var obj = new TestClass()
            {
                Property1 = new TestClass()
                {
                    Property2 = new TestClass()
                    {
                        Property3 = 1234,           // we'll try to evaluate a property on this
                        Property2 = new TestClass()
                        {
                            Property2 = 10
                        }
                    }
                }
            };
            Provider = new CompositeProvider(new ObjectValueProvider(obj));

            // Act:
            object value;
            var result = Provider.GetValue<object>("Property1.Property2.Property3.Property2", typeof(int), out value);

            // Assert:
            Assert.IsFalse(result);
            Assert.AreEqual(null, value);
        }

        public CompositeProvider Provider { get; set; }

        public class TestClass
        {
            public TestClass Property1 { get; set; }
            public object Property2 { get; set; }
            public int Property3 { get; set; }
            public TestClass2 Property4 { get; set; }
        }

        public class TestClass2
        {
            public object A { get; set; }
            public string B { get; set; }
        }

    }
}
