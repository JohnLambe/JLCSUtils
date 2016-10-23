using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DiExtension;
using JohnLambe.Util;
using JohnLambe.Util.FilterDelegates;
using System.Reflection;

using DiExtension.ConfigInject;
using DiExtension.ConfigInject.Providers;

namespace JohnLambe.Tests.JLUtilsTest.DependencyInjection.ConfigInject
{
    [TestClass]
    public class ConfigInjectTest
    {
        [TestMethod]
        public void ConfigProviderTest()
        {
            // Arrange:

            //            Container.ScanAssembly(Container.GetType().Assembly);  // the assembly of the container - JLUtils

            var provider = new DictionaryConfigProvider<object>();
            provider.AsDictionary["IntegerProperty1"] = 1005;
            provider.AsDictionary["StringProperty1"] = "string value 1";

            Container.ProviderChain.RegisterProvider(provider);

            var target = new ConfigInjectTestObject1();

            // Act:

            Container.BuildUp(target);

            // Assert:

            // Injected values:
            Assert.AreEqual("string value 1", target.StrProp1);
            Assert.AreEqual(1005,target.IntProp1);

            // Optional property not injected (its default should not be overwritten):
//            Assert.AreEqual("StrProp2DefaultValue", target.StrProp2, "Default value overwritten");

            Assert.AreEqual(3300, target.IntProp3, "Default value; no configured value in container");
            Assert.AreEqual(1005, target.IntProp4, "Default value overridden by value in container");

            //            Container.ScanAssembly(Assembly.GetExecutingAssembly(),
            //                new BooleanExpression<Type>(t => t.Namespace.StartsWith(this.GetType().Namespace)));

        }

        /// <summary>
        /// Demonstrates Unity's behaviour with [OptionalDependency] when the
        /// dependency is not resolved.
        /// </summary>
        [TestMethod]
        public void UnityOptionalDependencyTest()
        {
            // Arrange:

            var container = new Microsoft.Practices.Unity.UnityContainer();
            var target = new UnityTestObject1();

            // Act:

            container.BuildUp(typeof(UnityTestObject1),target,null);

            // Assert:

            Assert.AreEqual(null, target.StrProp2);   // the default value is overwritten with null
        }


        protected ExtendedDiContext Container = new ExtendedDiContext(new Microsoft.Practices.Unity.UnityContainer());
    }
}
