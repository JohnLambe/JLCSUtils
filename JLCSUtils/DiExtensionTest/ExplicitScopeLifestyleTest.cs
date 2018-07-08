using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;
using JohnLambe.Tests.JLUtilsTest;
//using SimpleInjector.Diagnostics;
using DiExtension.SimpleInject;

namespace Test.DiExtensionTest
{
    [TestClass]
    public class CustomLifestyleTest
    {
        [TestMethod]
        public void CustomLifeCycleTest1()
        {
            SetupDi();

            Console.Out.WriteLine("Container set up");


            // Resolve from the 'root' (no local scope):

            var rootInstance = _container.GetInstance<ITestClass1>();
            var rootInstance1 = _container.GetInstance<ITestClass1>();   // same instance

            var singletonFromRoot = _container.GetInstance<TestClass2>();

            Assert.AreEqual(rootInstance, rootInstance1);
            //            Assert.AreEqual(singletonFromRoot.Service, rootInstance1.Service);
            Assert.AreEqual(rootInstance.Service.Class2, singletonFromRoot);    // Service (custom Lifestyle) is injected with the same singleton instance as resolved directly


            // Resolve from a local scope (Scope 1):

            ScopeManager.StartScope();
            var scope1Instance = _container.GetInstance<ITestClass1>();  // different instance to 'root'
            var scope1Instance1 = _container.GetInstance<ITestClass1>();   // multiple calls in this scope give the same instance as each other

            // Nested scope:
            ScopeManager.StartScope();
            var scope1bInstance = _container.GetInstance<ITestClass1>();
            var singletonFromNested = _container.GetInstance<TestClass2>();    // from 'root'

            Assert.AreNotEqual(scope1bInstance, rootInstance);
            Assert.AreNotEqual(scope1bInstance, scope1Instance);

            Assert.AreEqual(singletonFromNested, singletonFromRoot);     // same singleton instance as root
            Assert.AreEqual(scope1bInstance.Service.Class2, singletonFromRoot);     // same singleton instance as root

            ScopeManager.EndScope();

            Assert.AreNotEqual(scope1Instance, rootInstance);
            Assert.AreEqual(scope1Instance, scope1Instance1);

            ScopeManager.EndScope();   // close Scope 1


            // Another local scope:

            ScopeManager.StartScope();
            var scope2Instance = _container.GetInstance<ITestClass1>();   // new instance

            Assert.AreNotEqual(scope2Instance, scope1Instance);
            Assert.AreNotEqual(scope2Instance, rootInstance);

            ScopeManager.EndScope();


            // From 'root' again:

            var rootInstance2 = _container.GetInstance<ITestClass1>();
            Assert.AreEqual(rootInstance2, rootInstance);
        }

        public void SetupDi()
        {
            _container = new Container();

            ScopeManager = new SiExplicitScopeLifestyleManager(_container);
            CustomLifeStyle = ScopeManager.Lifestyle;
            /*
            CustomLifeStyle = Lifestyle.CreateCustom(
                name: "CustomScopedLifestyle",
                lifestyleApplierFactory: instanceCreator =>
                {
                    return ScopeManager.Invoke(instanceCreator);
                });
            */

            _container.Register<IService, MyService>(CustomLifeStyle);
            _container.Register<ITestClass1, TestClass1>(CustomLifeStyle);
            //_container.Register<ITestClass1, TestClass1>();


            //            _container.Register(typeof(TestClass1));
            _container.Register<TestClass2>(Lifestyle.Singleton);


            _container.Verify();
        }


        public Container _container = new Container();
        public Lifestyle CustomLifeStyle { get; set; }
        public SiExplicitScopeLifestyleManager ScopeManager;



        public interface IService : IDisposable
        {
            string Id { get; }
            TestClass2 Class2 { get; }
        }

        public class MyService : IService
        {
            public MyService(TestClass2 c2)
            {
                Class2 = c2;
            }

            public TestClass2 Class2 { get; }   // singleton injected

            public string Id { get; } = Guid.NewGuid().ToString();

            public void Dispose()
            {
                Console.Out.WriteLine("Disposing " + ToString());
            }

            public override string ToString()
            {
                return "{" + base.ToString() + "#" + Id + "}";
            }
        }


        public interface ITestClass1
        {
            IService Service { get; }
        }

        public class TestClass1 : ITestClass1
        {
            public TestClass1(IService service)
            {
                Service = service;
            }

            public IService Service { get; }

            public override string ToString()
            {
                return "{" + base.ToString() + " Service=" + Service + "}";
            }

            public void Dispose()
            {
                Console.Out.WriteLine("Disposing " + ToString());
            }
        }


        public class TestClass2 : IDisposable   // registered as singleton
        {
            // public TestClass2(IService service)   would fail on verification - Lifestyle mismatch (can't inject custom Lifestyle in singleton)

            public TestClass2()//(IService service)
            {
                //                Service = service;
            }

            public IService Service { get; }

            public string Id { get; } = Guid.NewGuid().ToString();

            public override string ToString()
            {
                return "{" + base.ToString() + " Service=" + Service + "}";
            }

            public void Dispose()
            {
                Console.Out.WriteLine("Disposing " + ToString());
            }
        }

    }
}
