using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Reflection;
using JohnLambe.Util.PluginFramework.Attributes;

using JohnLambe.Tests.JLUtilsTest.Reflection.LooselyCoupledEvent;

namespace JohnLambe.Tests.JLUtilsTest.Reflection
{
    [TestClass]
    public class AttributeUtilTest
    {
        [TestMethod]
        public void GetCustomAttributes()
        {
            var obj = new TargetObject();
            var attribs = obj.GetType().GetCustomAttributes<EventHandlerAttribute>();

            var method = typeof(TargetObject).GetMethod("Handler");
            var attribs1 = method.GetCustomAttributes<EventHandlerAttribute>();

            //TODO
        }

        [TestMethod]
        public void GetCustomAttributes_Method()
        {

            var method = typeof(TargetObject).GetMethod("Handler");
            var attribs1 = method.GetCustomAttributes<EventHandlerAttribute>();

            Assert.AreEqual(attribs1.Count(),1);

        }
    }
}
