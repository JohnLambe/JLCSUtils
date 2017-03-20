using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.Reflection.LooselyCoupledEvent;
using JohnLambe.Util;

namespace JohnLambe.Tests.JLUtilsTest.Reflection.LooselyCoupledEvent
{
    [TestClass]
    public class EventChainTest
    {
        [TestCategory("Failing")]
        [TestMethod]
        public void SendEvent()
        {
            // Arrange:

            // a mutable string that handlers will log to:
            var log = new Var<string>("");

            // set up two handlers, that append to the string on call:
            Chain.Add(new TargetObject() { InstanceId = "H1", Log = log } );
            Chain.Add(new TargetObject() { InstanceId = "H2", Log = log }, 10000 );

            // Act:

            Chain.Invoke(new TestEvent()
            {
                Name = "test event"
            });

            // Assert:

            Assert.AreEqual("H1: Handler\nH2: Handler\n", log.Value);
        }

        protected EventChain<ITestEvent> Chain = new EventChain<ITestEvent>();
    }
}
