using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiExtension;
using JohnLambe.Util.PluginFramework;
using JohnLambe.Util.PluginFramework.Interfaces;
using JohnLambe.Util.PluginFramework.Attributes;
using JohnLambe.Util.PluginFramework.Events;
using Microsoft.Practices.Unity;

namespace JohnLambe.Tests.JLUtilsTest.PluginFramework
{
//    [DiRegisterInstance(Priority = 20000)]
    [Plugin("Plugin1", InitialiseOnStart = true)]
    public class TestPlugin1
    {
        public TestPlugin1()
        {
            Console.WriteLine("ctor: " + ToString());
        }

        [EventHandler(typeof(IPluginInitialiseEvent))]
        public void Init()
        {
            Console.WriteLine(ToString() + " received IPluginInitialiseEvent");
        }

        [Dependency("Config:IntValue")]
        public int IntValue
        {
            get { return _intValue; }
            set
            {
                Console.WriteLine("IntValue := " + value);
                _intValue = value;
            }
        }
        protected int _intValue;
    }

    [Plugin(InitialiseOnStart = true)]
    [PluginRequirement("Plugin1", Name = "Plugin 1")]
    public class TestPlugin2
    {
        public TestPlugin2()
        {
            Console.WriteLine("ctor: " + ToString());
        }

        [EventHandler(typeof(IPluginInitialiseEvent))]
        public void Init()
        {
            Console.WriteLine(ToString() + " received IPluginInitialiseEvent");
        }
    }

    [Plugin]
    public class TestPlugin3
    {
        public TestPlugin3()
        {
            Console.WriteLine("ctor: " + ToString());
        }

        [EventHandler(typeof(IPluginInitialiseEvent))]
        public void Init()
        {
            Console.WriteLine(ToString() + " received IPluginInitialiseEvent");
        }
    }
}
