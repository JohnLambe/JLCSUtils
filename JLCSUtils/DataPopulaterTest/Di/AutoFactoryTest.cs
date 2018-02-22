using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiExtension;
using DiExtension.SimpleInject;
using DiExtension.AutoFactory;
using DiExtension.AutoFactory.SimpleInject;
using SimpleInjector;

namespace JohnLambe.Tests.JLUtilsTest.Di
{
    [TestClass]
    public class AutoFactoryTest
    {
        public void Setup()
        {
            new AutoFactorySimpleInjectorExtension(_container);

        }

        [TestMethod]
        public void AutoFactoryTargetClassNotRegistered()
        {
            // Arrange:

            Setup();

            // Act:

            TestUtil.AssertThrows(typeof(Exception), () => _container.GetInstance<ClassThatNeedsFactory>());
        }

        [TestMethod]
        public void AutoFactorySimpleCase()
        {
            // Arrange:

            Setup();

            _container.Register(typeof(ClassThatNeedsFactory));
            _container.Register(typeof(ClassToBeCreatedByFactory));

            // This just test the setup (if the target class wasn't registered, this would throw an exception):
            var resolvedDirect = _container.GetInstance<ClassToBeCreatedByFactory>();


            // Act:

            var resolved = _container.GetInstance<ClassThatNeedsFactory>();


            // Assert:

            Assert.IsTrue(resolved.TheFactory != null);
        }


        protected Container _container = new Container();
//        protected SiExtendedDiContext _context = new SiExtendedDiContext(new SimpleInjector.Container());
    }

    public class ClassToBeCreatedByFactory
    {

    }

    public class ClassThatNeedsFactory
    {
        public ClassThatNeedsFactory(IFactory<ClassToBeCreatedByFactory> factory)
        {
            Console.WriteLine("ClassThatNeedsFactory created with parameter: " + factory);
            TheFactory = factory;
        }

        public ClassToBeCreatedByFactory UseFactory()
        {
            return TheFactory.Create();
        }

        public readonly IFactory<ClassToBeCreatedByFactory> TheFactory;
    }

}
