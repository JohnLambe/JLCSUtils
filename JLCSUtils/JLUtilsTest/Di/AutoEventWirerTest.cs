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
        [TestMethod]
        public void AutoEventWirerTest1()
        {
            Console.WriteLine(this.GetType().GetMethod("X"));

            // Arrange:

            TestEventHandler.Log = "";

            //            EventAutoWirer.RegisterWith(_context.Container, _context);
            new EventAutoWirer(_context.Container).Assemblies = new Assembly[] { Assembly.GetExecutingAssembly() };
            // add EventAutoWirer to the container.


            _context.Container.Register(typeof(TestObjectWithEvent));

            // Act:

            var instance = _context.GetInstance<TestObjectWithEvent>(typeof(TestObjectWithEvent));
            instance.FireEvent();

            // Assert:

            Assert.AreEqual("TestEventHandler2(1,a)\nTestEventHandler(1,a)\nStaticHandlerClass(1,a)\n", TestEventHandler.Log);

        }

        protected SiExtendedDiContext _context = new SiExtendedDiContext(new SimpleInjector.Container());
    }


    public delegate void TestDelegate(int param1, string param2);

    public class TestObjectWithEvent : IHasAutoWiredEvent
    {
        [Inject]
        public event TestDelegate OnTest;

        public void FireEvent()
        {
            OnTest(1, "a");
        }

    }

    //[AutoEventHandler(typeof(TestDelegate))]
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

    //[AutoEventHandler(typeof(TestDelegate),-100)]   // Low Priority value: This fires first
    [HasAutoWiredEventHandler]
    public class TestEventHandler2
    {
        [AutoEventHandler(typeof(TestDelegate), -100)]   // Low Priority value: This fires first
        public void Execute(int param1, string param2)
        {
            TestEventHandler.Log = TestEventHandler.Log + 
                GetType().Name + "(" + param1 + "," + param2
                + ")\n";
        }
    }


    public class TestEventHandler2B : TestEventHandler2  // Not registered because the HasAutoWiredEventHandler attribute is hot inherited
    {
    }

    [HasAutoWiredEventHandler]
    public class TestEventHandler2C : TestEventHandler2  // Not registered because the HasAutoWiredEventHandler attribute is hot inherited
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
