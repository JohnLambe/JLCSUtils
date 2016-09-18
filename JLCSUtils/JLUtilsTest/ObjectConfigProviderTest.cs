using JohnLambe.Util.DependencyInjection.ConfigInject;
using JohnLambe.Util.DependencyInjection.ConfigInject.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest
{
    [TestClass]
    public class ObjectConfigProviderTest
    {
        [TestMethod]
        public void ReadValue()
        {
            Assert.AreEqual(200, Provider.GetValue<int>("ReadOnlyIntProperty"));
            Assert.AreEqual("str", Provider.GetValue<string>("StringProperty"));
            Assert.AreEqual("200", Provider.GetValue<string>("ReadOnlyIntProperty"), "Reading int property as string");

            string value;
            Assert.AreEqual(false, Provider.GetValue<string>("InvalidProperty",typeof(string),out value));

            Assert.AreEqual(null, Provider.GetValue<DateTime?>("InvalidProperty"));

            /*
            TestUtil.AssertThrows(typeof(KeyNotFoundException),
                () => Provider.GetValue<string>("InvalidProperty")
                );
                */
        }

        protected IConfigProvider Provider = new ObjectValueProvider(
            new TestObjectForValueProvider()
            {
                StringProperty = "str",
            }
            );
    }

    public class TestObjectForValueProvider
    {
        public string StringProperty { get; set; }
        public int ReadOnlyIntProperty { get; } = 200;
    }
}
