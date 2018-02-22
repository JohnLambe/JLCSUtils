
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
using JohnLambe.Util.PluginFramework.Host;
using System.Reflection;

namespace JohnLambe.Tests.JLUtilsTest.PluginFramework
{
    [TestClass]
    public class PluginHostTest
    {
        [TestMethod]
        public void StartFramework()
        {
            var loader = new PluginLoader(Host);
            loader.ScanAssembly(Assembly.GetExecutingAssembly());

        }

        protected PluginHost Host = new PluginHost();
    }
}
