using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.DependencyInjection;
using JohnLambe.Util.DependencyInjection.SimpleInject;
using JohnLambe.Util.DependencyInjection.Attributes;
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


            _context.Container.Register(typeof(TestObjectWithEvent));

            // Act:

            var instance = _context.GetInstance<TestObjectWithEvent>(typeof(TestObjectWithEvent));
            instance.FireEvent();

            // Assert:

            Assert.AreEqual("TestEventHandler2(1,a)\nTestEventHandler(1,a)\n", TestEventHandler.Log);

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

    [AutoEventHandler(typeof(TestDelegate))]
    public class TestEventHandler
    {
        public void Execute(int param1, string param2)
        {
            Log = Log + "TestEventHandler(" + param1 + "," + param2
                + ")\n";
        }

        public static string Log;  // Event handlers append to this
    }

    [AutoEventHandler(typeof(TestDelegate),-100)]   // Low Priority value: This fires first
    public class TestEventHandler2
    {
        public void Execute(int param1, string param2)
        {
            TestEventHandler.Log = TestEventHandler.Log + 
                "TestEventHandler2(" + param1 + "," + param2
                + ")\n";
        }
    }


    public class TestEventHandler2B : TestEventHandler2  // Not registered because the attribute is hot inherited
    {
    }


}
