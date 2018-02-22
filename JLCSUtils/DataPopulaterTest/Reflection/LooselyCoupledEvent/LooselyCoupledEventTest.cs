using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.PluginFramework.Interfaces;
using JohnLambe.Util.PluginFramework.Attributes;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Reflection.LooselyCoupledEvent;


namespace JohnLambe.Tests.JLUtilsTest.Reflection.LooselyCoupledEvent
{
    [TestClass]
    public class LooselyCoupledEventTest
    {
        // Arrange:
        LooselyCoupledEventProcessor<IPluginEvent> proc = new LooselyCoupledEventProcessor<IPluginEvent>();
        object target = new TargetObject();


        #region GetMethodForEvent

        [TestMethod]
        public void GetMethodForEvent_Simple()
        {
            // Act:
            var eventHandlerMethod = proc.GetMethodForEvent(typeof(TargetObject), typeof(ITestEvent));
            Console.WriteLine(eventHandlerMethod);

            // Assert:
            Assert.AreEqual("Handler", eventHandlerMethod.Method.Name);
            Assert.AreEqual(typeof(ITestEvent), eventHandlerMethod.EventInterface);
            Assert.AreEqual(typeof(ITestEvent), eventHandlerMethod.HandledEventType);
            Assert.AreEqual(typeof(ITestEvent), eventHandlerMethod.HandlerAttribute.EventType);
            Assert.AreEqual(true, eventHandlerMethod.IsValid);
        }

        [TestMethod]
        public void GetMethodForEvent_MultipleAttributes()
        {
            // Act:
            var eventHandlerMethod = proc.GetMethodForEvent(typeof(TargetObject), typeof(ITestEvent5a));
            Console.WriteLine(eventHandlerMethod);

            // Assert:
            Assert.AreEqual("Handler5", eventHandlerMethod.Method.Name);
            Assert.AreEqual(typeof(ITestEvent5a), eventHandlerMethod.EventInterface);
            Assert.AreEqual(typeof(ITestEvent5), eventHandlerMethod.HandledEventType);
            Assert.AreEqual(typeof(ITestEvent5), eventHandlerMethod.HandlerAttribute.EventType);
            Assert.AreEqual(true, eventHandlerMethod.IsValid);
        }

        [TestMethod]
        public void GetMethodForEvent_HandlesBaseEvent()
        {
            // Act:
            var eventHandlerMethod = proc.GetMethodForEvent(typeof(TargetObject), typeof(ITestEvent2));
            Console.WriteLine(eventHandlerMethod);

            // Assert:
            Assert.AreEqual("Handler_TestEventBase", eventHandlerMethod.Method.Name);
            Assert.AreEqual(typeof(ITestEvent2), eventHandlerMethod.EventInterface);
            Assert.AreEqual(typeof(ITestEventBase), eventHandlerMethod.HandledEventType);
            Assert.AreEqual(typeof(ITestEventBase), eventHandlerMethod.HandlerAttribute.EventType);
            Assert.AreEqual(true, eventHandlerMethod.IsValid);
        }

        [TestMethod]
        public void GetMethodForEvent_Ambiguous()
        {
            // Act:
            TestUtil.AssertThrows(typeof(HandlerResolutionFailedException),
                () => proc.GetMethodForEvent(typeof(TargetObject), typeof(ITestEvent3)));
        }

        [TestMethod]
        public void GetMethodForEvent_NoHandler()
        {
            // Act:
            var handler = proc.GetMethodForEvent(typeof(TargetObject), typeof(IUnhandledEvent));

            // Assert:
            Assert.AreEqual(null, handler);
        }

        [TestMethod]
        public void GetMethodForEvent_ByParameterType()
        {
            // Act:
            var eventHandlerMethod = proc.GetMethodForEvent(typeof(TargetObject), typeof(ITestEvent4));
            Console.WriteLine(eventHandlerMethod);

            // Assert:
            Assert.AreEqual("Handler4", eventHandlerMethod.Method.Name);
            Assert.AreEqual(typeof(ITestEvent4), eventHandlerMethod.EventInterface);
            Assert.AreEqual(typeof(ITestEvent4), eventHandlerMethod.HandledEventType);
            Assert.AreEqual(null, eventHandlerMethod.HandlerAttribute.EventType);
            Assert.AreEqual(true, eventHandlerMethod.IsValid);
        }

        #endregion

        #region Invoke

        [TestMethod]
        public void Invoke_NoArgs()
        {
            // Arrange:
            var evt = new TestEvent()
            {
                IntProperty = 1001,
                StringProperty = "str",
            };

            // Act:
            var result = proc.Invoke<object>(target,evt);
                // should invoke 'Handler', mapped from ITestEvent, which is implemented by TestEvent.

            // Assert:
            Assert.AreEqual("Handler", result);
        }

        [TestMethod]
        public void Invoke_Args()
        {
            // Arrange:
            var evt = new TestEvent4()
            {
                IntProperty = 250,
                StringProperty = "event4",
            };

            // Act:
            var result = proc.Invoke<object>(target, evt);

            // Assert:
            Assert.AreEqual("Handler4: 250; 1000", result);
        }

        #endregion
    }

}
