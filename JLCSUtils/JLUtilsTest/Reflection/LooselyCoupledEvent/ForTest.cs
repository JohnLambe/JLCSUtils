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
using JohnLambe.Util;

// Clasees / interface for use in testing LooselyCoupledEvent:

namespace JohnLambe.Tests.JLUtilsTest.Reflection.LooselyCoupledEvent
{
    #region Target classes
    // Classes that can receive events.

    /// <summary>
    /// Class for receiving events in tests.
    /// </summary>
    public class TargetObject
    {
        [EventHandler(typeof(ITestEventBase))]
        public int Handler_TestEventBase()
        {
            return 100;
        }

        [EventHandler(typeof(ITestEvent))]
        public virtual string Handler()
        {
            Console.WriteLine("Handler");
            Log.Value = Log.Value + InstanceId + ": Handler\n";
            return "Handler";
        }

        [EventHandler(typeof(ITestEvent3))]
        public void Handler3()
        {
            Console.WriteLine("Handler3");
        }

        [EventHandler(typeof(ITestEvent3))]   // ambiguous handler specification
        public void Handler3Duplicate()
        {
            Console.WriteLine("Handler3Dup");
        }

        [EventHandler]   // event type determined by parameters
        public string Handler4(
            [EventHandlerParameter(Required=false)]int arg1 = 1000,
            ITestEvent4 eventDetails = null)
        {
            var message = "Handler4: " + eventDetails.IntProperty + "; " + arg1;
            Console.WriteLine(message);
            Log.Value = message;
            return message;
        }

        [EventHandler(typeof(ITestEvent3b))]
        [EventHandler(typeof(IDisposable))]  // invalid
        [EventHandler(typeof(ITestEvent5))]
        public void Handler5()
        {
            Console.WriteLine("Handler5");
        }

        public void NotAHandler()
        {
        }

        [EventHandler(typeof(IOtherEvent))]
        public void OtherHandler()
        {
        }

        [EventHandler(typeof(IDisposable))]  // invalid
        public void OtherHandler2()
        {
        }

        public Var<string> Log { get; set; } = "";
        public virtual string InstanceId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TestDerivedObject : TargetObject
    {

    }

    #endregion

    #region Event interfaces

    public interface ITestEventBase : IPluginEvent
    {
        string CustomMethod();
        int IntProperty { get; }
        string SettableProperty { get; set; }
    }

    public interface ITestEvent : ITestEventBase
    {
        string StringProperty { get; }
    }

    public interface ITestEvent2 : ITestEventBase
    {
    }

    public interface ITestEvent3 : ITestEventBase
    {
    }

    public interface ITestEvent3a : ITestEvent3
    {
    }

    public interface ITestEvent3b : ITestEvent3
    {
    }

    public interface ITestEvent4 : ITestEventBase
    {
    }

    public interface ITestEvent5 : ITestEventBase
    {
    }

    public interface ITestEvent5a : ITestEvent5
    {
    }

    public interface IOtherTestSubEvent : ITestEvent2
    {
    }

    public interface IUnhandledEvent : IPluginEvent
    {
    }

    public interface IOtherEvent : IPluginEvent
    {
        string CustomMethod();
    }

    #endregion

    #region Event implementation

    public class TestEvent : ITestEvent
    {
        public string EventTypeId { get; set; }

        public int IntProperty { get; set; }

        public string Name { get; set; }

        public string SettableProperty { get; set; }

        public string StringProperty { get; set; }

        public string CustomMethod()
        {
            throw new NotImplementedException();
        }
    }

    public class TestEvent4AsSubclass : TestEvent, ITestEvent4
    {
    }

    public class TestEvent4 : ITestEvent4
    {
        public string EventTypeId { get; set; }

        public int IntProperty { get; set; }

        public string Name { get; set; }

        public string SettableProperty { get; set; }

        public string StringProperty { get; set; }

        public string CustomMethod()
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
