using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util;
using JohnLambe.Util.Misc;

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class DynamicProxyTest
    {
        [TestMethod]
        public void InvokeMethodAndProperty()
        {
            // Arrange:
            TestObject obj = new TestObject()
            {
                StringProperty = "str1"
            };
//            DynamicProxyBase<TestObject> proxy = new DynamicProxyBase<TestObject>(obj);  // have to declare with "dynamic" keyword when in a different assembly to declaration.
            dynamic proxy = new DynamicProxyBase<TestObject>(obj);

            // Assert:
            Assert.AreEqual("str1", proxy.StringProperty, "Get property failed");
            Assert.AreEqual(101, proxy.Method(), "Invoke method failed");
            TestUtil.AssertThrows(typeof(Exception), () => proxy.NonExistantMethod());
            TestUtil.AssertThrows(typeof(Exception), () => Console.WriteLine(proxy.NonExistantProperty));
        }

        [TestMethod]
        public void FailedCalls()
        {
            TestFailedCalls(new DynamicProxyBase<object>(new object()));
        }

        public static void TestFailedCalls(dynamic proxy)
        {
            TestUtil.AssertThrows(typeof(Exception),
                () => Console.WriteLine(proxy.InvalidProperty),
                "Exception not thrown on trying to get invalid property value"
                );

            TestUtil.AssertThrows(typeof(Exception),
                () => proxy.InvalidProperty2 = "value",
                "Exception not thrown on trying to set invalid property value"
                );

            TestUtil.AssertThrows(typeof(Exception),
                () => Console.WriteLine(proxy.NonExistantMethod()),
                "Exception not thrown on trying to invoke invalid method"
                );
        }

    }

    [TestClass]
    public class LazyPopulatedProxyTest
    {
        [TestMethod]
        public void InvokeMethodAndProperty()
        {
            TestObject.CreatedCount = 0;

            dynamic proxy = new LazyPopulatedProxy<TestObject>( () => new TestObject()
            {
                StringProperty = "value"
            });

            // Assert:
            Assert.AreEqual(0, TestObject.CreatedCount, "Delegate invoked too early");

            Assert.AreEqual("value", proxy.StringProperty, "Get property failed");
            Assert.AreEqual(1, TestObject.CreatedCount, "Delegate not invoked?? or invoked multiple times for one call");

            Assert.AreEqual(101, proxy.Method(), "Invoke method failed");

            TestUtil.AssertThrows(typeof(Exception), () => proxy.NonExistantMethod());
            TestUtil.AssertThrows(typeof(Exception), () => Console.WriteLine(proxy.NonExistantProperty));

            Assert.AreEqual(TestObject.CreatedCount, 1, "Delegate invoked multiple times");
        }

        [TestMethod]
        public void FailedCalls()
        {
            DynamicProxyTest.TestFailedCalls(new DynamicProxyBase<string>("string"));
        }
    }

    [TestClass]
    public class NotifyOnChangeProxyTest
    {
        public NotifyOnChangeProxyTest()
        {
            proxy.ValueChanged += new NotifyOnChangeProxy<TestObject>.ChangeEvent(X_OnChange);
        }

        [TestMethod]
        public void NotifyOnSetter()
        {
            // Act:

            proxy.StringProperty = "new value";

            // Assert:

            Assert.AreEqual(_notification, "StringProperty:=new value");
        }

        /// <summary>
        /// Calling methods with FireOnCall==null.
        /// </summary>
        [TestMethod]
        public void NotifyOnMethodCall()
        {
            Assert.AreEqual(proxy.FireOnCall, null);  // check that the default configuration hasn't changed

            // Act:

            proxy.Method();
            Assert.AreEqual(null, _notification, "Event fired when shouldn't have");

            try
            {
                proxy.InvalidMethod();
                Assert.Fail("Allowed invalid method call");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception (expected): " + ex);
            }
            Assert.AreEqual(null, _notification, "Event fired on failed invocation");

            Console.WriteLine("" + proxy.StringProperty);
            Assert.AreEqual(null, _notification, "Getter fired event");

            proxy.Method2();   // this should fire it

            // Assert:

            Assert.IsTrue(_notification.Contains("Method2"));
        }

        /// <summary>
        /// Calling methods with FireOnCall==true.
        /// </summary>
        [TestMethod]
        public void NotifyOnMethodCall2()
        {
            // Arrange:

            proxy.FireOnCall = true;
            proxy.FireOnGet = false;

            // Act:

            proxy.Method();
            Assert.AreNotEqual(null, _notification, "Event didn't fire");

            _notification = null;   // reset for next test

            Console.WriteLine("" + proxy.StringProperty);
            Assert.AreEqual(null, _notification, "Getter fired event");

            proxy.Method2();   // this should fire it

            // Assert:

            Assert.IsTrue(_notification.Contains("Method2"));
        }

        /// <summary>
        /// Enable events for all getters and invoke one.
        /// </summary>
        [TestMethod]
        public void GetterFiresEvent()
        {
            // Arrange:

            proxy.FireOnGet = true;

            // Act:

            proxy.Method();
            Assert.AreEqual(null, _notification);

            Console.WriteLine("" + proxy.StringProperty);    // should fire event

            // Assert:
            Assert.AreNotEqual(null, _notification);
        }

        /// <summary>
        /// Enable events for all getters and invoke one.
        /// </summary>
        [TestMethod]
        public void GetterFiresEvent2()
        {
            // Arrange:

            Assert.AreEqual(proxy.FireOnGet,null);   // check that configuration hasn't changed

            // Act:

            proxy.Method();
            Assert.AreEqual(null, _notification);

            Console.WriteLine("" + proxy.SideEffectProperty);    // should fire event

            // Assert:
            Assert.AreNotEqual(null, _notification);
        }

        /// <summary>
        /// Calling methods with events on calling getters and methods enabled.
        /// </summary>
        [TestMethod]
        public void EventsEnabled()
        {
            // Arrange:

            proxy.FireOnCall = true;
            proxy.FireOnGet = true;

            // Act:

            proxy.Method();     // should fire event (even though it's not attributed)
            Assert.AreNotEqual(null, _notification);

            Console.WriteLine("" + proxy.StringProperty);
            Assert.AreNotEqual(null, _notification, "Getter didn't fire event");

            proxy.Method2();   // this should fire it

            // Assert:

            Assert.AreNotEqual(null, _notification);
        }

        /// <summary>
        /// Call a method explicitly flagged as not affecting state.
        /// </summary>
        [TestMethod]
        public void NoStateChangeAttribute()
        {
            // Arrange:
            Assert.AreEqual(proxy.FireOnCall, null);   // check that configuration hasn't changed

            // Act:
            proxy.NoStateChange();

            // Assert:
            Assert.AreEqual(null, _notification);
        }

        [TestMethod]
        public void FailedCalls()
        {
            DynamicProxyTest.TestFailedCalls(new DynamicProxyBase<TestObject>(new TestObject()));
        }

        private void X_OnChange(NotifyOnChangeProxy<TestObject> sender, string name, object value)
        {
            _notification = name + ":=" + value;
            Console.WriteLine(_notification);
        }

        protected dynamic proxy = new NotifyOnChangeProxy<TestObject>(new TestObject());
        protected string _notification;
    }

    public class TestObject
    {
        public TestObject()
        {
            CreatedCount++;
        }
        public static int CreatedCount;

        public int Method()
        {
            Console.WriteLine("Method invoked");
            return 101;
        }

        [StateChange]
        public void Method2()
        {
            StringProperty2 = "Method2 called";
        }

        [StateChange(false)]
        public virtual int NoStateChange()
        {
            return 5;
        }

        public virtual string StringProperty { get; set; }

        public virtual string StringProperty2 { get; set; }

        [StateChange]
        public virtual string SideEffectProperty
        {
            get
            {
                StringProperty2 = "SideEffectProperty called";
                return "SideEffectProperty";
            }
        }
    }
}
