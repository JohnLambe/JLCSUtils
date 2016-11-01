using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DiExtension;
using JohnLambe.Util;
using JohnLambe.Util.FilterDelegates;
using System.Reflection;

using DiExtension.ConfigInject;
using DiExtension.ConfigInject.Providers;
using DiExtension.SimpleInject;
using DiExtension.Attributes;

namespace JohnLambe.Tests.JLUtilsTest.Di.ConfigInject
{
    [TestClass]
    public class ConfigInjectTest
    {
        [TestMethod]
        public void ConfigProviderTest()
        {
            // Arrange:

            //            Container.ScanAssembly(Container.GetType().Assembly);  // the assembly of the container - JLUtils

            /*
            var provider = new DictionaryConfigProvider<object>();
            provider.AsDictionary["IntegerProperty1"] = 1005;
            provider.AsDictionary["StringProperty1"] = "string value 1";

            Container.ProviderChain.RegisterProvider(provider);
            */

            Container.RegisterInstance("IntegerProperty1", 1005);
            Container.RegisterInstance("StringProperty1", "string value 1");


            var target = new ConfigInjectTestObject1();

            // Act:

            Container.BuildUp(target);

            // Assert:

            // Injected values:
            Assert.AreEqual("string value 1", target.StrProp1);
            Assert.AreEqual(1005, target.IntProp1);

            // Optional property not injected (its default should not be overwritten):
            //            Assert.AreEqual("StrProp2DefaultValue", target.StrProp2, "Default value overwritten");

            Assert.AreEqual(300, target.IntProp3, "Default value; no configured value in container");
            Assert.AreEqual(1005, target.IntProp4, "Default value overridden by value in container");

            //            Container.ScanAssembly(Assembly.GetExecutingAssembly(),
            //                new BooleanExpression<Type>(t => t.Namespace.StartsWith(this.GetType().Namespace)));

        }

        [TestMethod]
        public void ConfigProviderTest2()
        {
            // Arrange:

            //            Container.ScanAssembly(Container.GetType().Assembly);  // the assembly of the container - JLUtils

            /*
            var provider = new DictionaryConfigProvider<object>();
            provider.AsDictionary["IntegerProperty1"] = 1005;
            provider.AsDictionary["StringProperty1"] = "string value 1";

            Container.ProviderChain.RegisterProvider(provider);
            */

            Container.RegisterInstance("IntegerProperty1", 1005);
            Container.RegisterInstance("StringProperty1", "string value 1");


            var target = new ConfigInjectTestObject1();

            // Act:

            Container.BuildUp(target);

            // Assert:

            // Injected values:
            Assert.AreEqual("string value 1", target.StrProp1);
            Assert.AreEqual(1005, target.IntProp1);

            // Optional property not injected (its default should not be overwritten):
            //            Assert.AreEqual("StrProp2DefaultValue", target.StrProp2, "Default value overwritten");

            Assert.AreEqual(300, target.IntProp3, "Default value; no configured value in container");
            Assert.AreEqual(1005, target.IntProp4, "Default value overridden by value in container");

            //            Container.ScanAssembly(Assembly.GetExecutingAssembly(),
            //                new BooleanExpression<Type>(t => t.Namespace.StartsWith(this.GetType().Namespace)));

        }

        [TestMethod]
        public void InjectionFailed()
        {
            // Arrange:

            Container.RegisterInstance("IntegerProperty1", 1005);
            Container.RegisterInstance("StringProperty1", "string value 1");

            var target = new ConfigInjectTestObject1();


            // Act:

            TestUtil.AssertThrows(typeof(Exception),
                () => Container.BuildUp(target)
                );

        }

        [TestMethod]
        public void TryChangeRegisteredValue_Fail()
        {
            // Arrange:

            var target = new ConfigInjectSimpleTestObject();

            Container.RegisterInstance("IntegerProperty1", 1005);
            Container.RegisterInstance("StringProperty1", "string value 1");

            Container.RegisterInstance("IntegerProperty1", 1006);   // replaces existing value, because it is not resolved

            // Act:

            Container.BuildUp(target);
            Assert.AreEqual(1006, target.IntProp1);

            Container.RegisterInstance("IntegerProperty1", 1007);  // doesn't work because it is already resolved

            Container.BuildUp(target);


            // Assert:

            Assert.AreEqual(1006, target.IntProp1);

        }

        [TestMethod]
        public void TryChangeRegisteredValue_NoCache()
        {
            // Arrange:

            Container.CacheValues = false;

            var target = new ConfigInjectSimpleTestObject();

            Container.RegisterInstance("IntegerProperty1", 1005);
            Container.RegisterInstance("StringProperty1", "string value 1");

            Container.RegisterInstance("IntegerProperty1", 1006);   // replaces existing value, because it is not resolved

            // Act:

            Container.BuildUp(target);
            Assert.AreEqual(1006, target.IntProp1);

            Container.RegisterInstance("IntegerProperty1", 1007);  // doesn't work because it is already resolved

            Container.BuildUp(target);


            // Assert:

            Assert.AreEqual(1007, target.IntProp1);
        }


        protected SiExtendedDiContext Container = new SiExtendedDiContext(new SimpleInjector.Container());
    }


    public class ConfigInjectTestObject1
    {
        [Inject("IntegerProperty1")]
        public virtual int IntProp1 { get; set; }

        //[Dependency("Config:NonExistantProperty2")]
        public virtual int IntProp2 { get; set; } = 200;

//        [Inject("NonExistantProperty3")]//3300
        public virtual int IntProp3 { get; set; } = 300;

        [Inject("IntegerProperty1")]
        public virtual int IntProp4 { get; set; } = 20;

        [Inject("StringProperty1")]
        public virtual string StrProp1 { get; set; }

    }

    public class ConfigInjectTestObject2 : ConfigInjectTestObject1
    {
        [Inject("NonExistantStringProperty", Required = false)]
        public virtual string StrProp2 { get; set; } = "StrProp2DefaultValue";
    }

    public class ConfigInjectTestObject3
    {
        [Inject("NonExistantProperty5")]
        public virtual int IntProp5 { get; set; } = 500;
    }

    public class ConfigInjectSimpleTestObject
    {
        [Inject("IntegerProperty1")]
        public virtual int IntProp1 { get; set; } = 500;
    }

}
