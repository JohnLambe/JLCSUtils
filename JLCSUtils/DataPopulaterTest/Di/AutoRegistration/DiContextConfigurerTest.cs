using DiExtension.SimpleInject;
using JohnLambe.Util.FilterDelegates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.Di.AutoRegistration
{
    [TestClass]
    public class DiContextConfigurerTest
    {
        [TestMethod]
        public void AutoRegistrationTest()
        {
            // Arrange:

            Container.ScanAssembly(this.GetType().Assembly,
                new BooleanExpression<Type>(t => t.Namespace.StartsWith(this.GetType().Namespace))  // only classes in the same namespace as this
                );  // this assembly, and the namespace of this test class
            //Container.ProviderChain


            // Act:

            object resolvedNyName;
            var resolvedByType = Container.GetInstance<ToBeRegisteredByType>();
            Container.GetValue("RegisteredByName", typeof(ToHaveRegisteredInstance), out resolvedNyName);
            Console.WriteLine("Resolved by name: " + (resolvedNyName ?? "(null)") );


            // Assert:

            Assert.IsTrue(resolvedByType != null, "Failed to resolve class registered by type");
            Assert.IsTrue(resolvedNyName != null && resolvedByType is ToBeRegisteredByType, "Failed to resolve instance registered by name");
        }

        protected SiExtendedDiContext Container = new SiExtendedDiContext(new SimpleInjector.Container());
    }
}
