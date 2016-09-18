using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util.DependencyInjection;
using JohnLambe.Util;
using JohnLambe.Util.FilterDelegates;
using System.Reflection;

namespace JohnLambe.Tests.JLUtilsTest.DependencyInjection
{
    [TestClass]
    public class DiTest
    {
        [TestMethod]
        public void RegisterAndResolveTypeAndInstance()
        {
            Container.ScanAssembly(Assembly.GetExecutingAssembly(),
                new BooleanExpression<Type>( t => t.Namespace.StartsWith(this.GetType().Namespace) ));

            Console.Out.WriteLine(Container.ToString());

            // Instances created:
            Assert.AreNotEqual(null, Test1.Instance);
            Assert.AreNotEqual(null, Test3.Instance);

            // Injected by type:
            Assert.AreNotEqual(null, Test1.Instance.TestBaseRef);
            Assert.IsTrue(Test1.Instance.TestBaseRef is Test2Registered);

            // Injected registered instance:
            Assert.AreNotEqual(null, Test1.Instance.Test3Ref);
            Assert.AreEqual(Test3.Instance, Test1.Instance.Test3Ref);
        }


        //TODO: Broken by refactoring of IExtendedDiContext??
        [TestMethod]
        public void ConfigProviderTest()
        {
            Container.ScanAssembly(Container.GetType().Assembly);  // the assembly of the container - JLUtils
            //Container.ProviderChain

        }


        protected ExtendedDiContext Container = new ExtendedDiContext(new Microsoft.Practices.Unity.UnityContainer());
    }
}
