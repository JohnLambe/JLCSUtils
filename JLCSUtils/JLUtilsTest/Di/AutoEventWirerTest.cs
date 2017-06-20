using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DiExtension;
using DiExtension.SimpleInject;
using DiExtension.Attributes;
using System.Reflection;

namespace JohnLambe.Tests.JLUtilsTest.Di
{
    [TestClass]
    public class AutoEventWirerTest
    {
        /*
        [TestMethod]
        public void T()
        {
            var a = new EventKey() { EventType = GetType() };
            var b = new EventKey() { EventType = GetType() };

            Assert.AreEqual(a, b);
        }
        protected struct EventKey
        {
            public Type EventType;
            public string Key;
        }
        */

        [TestMethod]
        public void AutoEventWirerTest1()
        {
            Console.WriteLine(this.GetType().GetMethod("X"));

            // Arrange:

            TestEventHandler.Log = "";    // the event handler modifies this

            //            EventAutoWirer.RegisterWith(_context.Container, _context);
            new EventAutoWirer(_context.Container).Assemblies = new Assembly[] { Assembly.GetExecutingAssembly() };
            // add EventAutoWirer to the container.


            _context.Container.Register(typeof(TestObjectWithEvent));

            // Act:

            var instance = _context.GetInstance<TestObjectWithEvent>(typeof(TestObjectWithEvent));
            instance.FireEvent();    // fire the event that should have been injected

            // Assert:

            Assert.AreEqual("TestEventHandler2(1,a)\nTestEventHandler(1,a)\nStaticHandlerClass(1,a)\nTestEventHandler2.Key1(1,a)\n", TestEventHandler.Log);
                // Check that the event handler ran by examining this static which it modifies.
        }

        [TestMethod]
        public void AutoEventByKey()
        {
            // Arrange:

            TestEventHandler.Log = "";    // the event handler modifies this

            //            EventAutoWirer.RegisterWith(_context.Container, _context);
            new EventAutoWirer(_context.Container).Assemblies = new Assembly[] { Assembly.GetExecutingAssembly() };
            // add EventAutoWirer to the container.


            _context.Container.Register(typeof(TestObjectWithEvent));

            // Act:

            var instance = _context.GetInstance<TestObjectWithEvent>(typeof(TestObjectWithEvent));
            instance.FireEvent_Key1();    // fire the event that should have been injected

            // Assert:

            Assert.AreEqual("TestEventHandler2.Key1(123,Key1)\n", TestEventHandler.Log);
            // Check that the event handler ran by examining this static which it modifies.
        }

        protected SiExtendedDiContext _context = new SiExtendedDiContext(new SimpleInjector.Container());
    }


    public delegate void TestDelegate(int param1, string param2);

    public class TestObjectWithEvent : IHasAutoWiredEvent
    {
        [Inject(Required = true)]
        public event TestDelegate OnTest;

        [Inject(Key = "Key1")]
        public event TestDelegate OnTest_Key1;

        public void FireEvent()
        {
            OnTest(1, "a");
        }
        public void FireEvent_Key1()
        {
            OnTest_Key1(123, "Key1");
        }
    }

    [HasAutoWiredEventHandler]
    public class TestEventHandler
    {
        [AutoEventHandler(typeof(TestDelegate))]
        public void HandlerMethod(int param1, string param2)
        {
            Log = Log + "TestEventHandler(" + param1 + "," + param2
                + ")\n";
        }

        public void NonHandlerMethod()  // test that unattributed method is NOT fired
        {
        }

        [AutoEventHandler(typeof(EventHandler))]
        public void OtherEventHandler()  // test that this method is NOT fired
        {
        }

        public static string Log;  // Event handlers append to this
    }

    [HasAutoWiredEventHandler]
    public class TestEventHandler2
    {
        Guid _guid = Guid.NewGuid();

        [AutoEventHandler(typeof(TestDelegate), -100)]   // Low Priority value: This fires first
        public virtual void Execute(int param1, string param2)
        {
            TestEventHandler.Log = TestEventHandler.Log + 
                GetType().Name + "(" + param1 + "," + param2
                + ")\n";
            Console.WriteLine("Execute invoked on " + _guid);
        }

        [AutoEventHandler(typeof(TestDelegate), 50000, Key = "Key1")]  // fires last
        public void Key1(int param1, string param2)
        {
            TestEventHandler.Log = TestEventHandler.Log +
                GetType().Name + ".Key1(" + param1 + "," + param2
                + ")\n";
            Console.WriteLine("Key1 invoked on " + _guid);    // will be fired on different instance to Execute
        }
    }


    public class TestEventHandler2B : TestEventHandler2  // Not registered because the HasAutoWiredEventHandler attribute is not inherited
    {
        [AutoEventHandler(typeof(TestDelegate), -100)]
        public void Execute1(int param1, string param2)
        {
            throw new NotImplementedException();   // shouldn't be called
        }
    }

    [HasAutoWiredEventHandler]
    public class TestEventHandler2C : TestEventHandler2  // Not registered because the AutoEventHandler attribute is not inherited
    {
        public override void Execute(int param1, string param2)
        {
            throw new NotImplementedException();   // shouldn't be called
        }
    }

    [HasAutoWiredEventHandler]
    public class TestEventHandler2D : TestEventHandler2  // Not registered because only declared methods are used.
    {
    }

    [HasAutoWiredEventHandler]
    public class StaticHandlerClass
    {
        [AutoEventHandler(typeof(TestDelegate), 1000)]   // High Priority value: This fires last
        public static void StaticHandler(int param1, string param2)
        {
            TestEventHandler.Log = TestEventHandler.Log +
               "StaticHandlerClass(" + param1 + "," + param2
                + ")\n";
        }
    }

    //TODO: Test two handlers on same instance.

}
