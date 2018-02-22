using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiExtension.AutoFactory;
using DiExtension.Attributes;
using DiExtension.SimpleInject;
using DiExtension.AutoFactory.SimpleInject;

namespace JohnLambe.Tests.JLUtilsTest.Di
{
    [TestClass]
    public class AutoFactoryTest2
    {
        const string InjectedPropertyValue = "InjectedValue";

        public void Setup()
        {
            _context.RegisterInstance("InjectedProperty", InjectedPropertyValue);

            new AutoFactorySimpleInjectorExtension(_context.Container);
        }

        [TestMethod]
        public void InjectFactoryAndProperty()
        {
            Setup();

            _context.Container.Register(typeof(ClassToBeCreatedByFactory));  // register so that AutoFactory can create it
            _context.Container.Register(typeof(ClassThatNeedsFactoryWithPropertyInjection));


            // This just tests the setup (if the target class wasn't registered, this would throw an exception):
            var resolvedDirect = _context.Container.GetInstance<ClassThatNeedsFactoryWithPropertyInjection>();
            Assert.AreEqual(InjectedPropertyValue, resolvedDirect.InjectedProperty);


            // Act:

            var resolved = _context.Container.GetInstance<ClassThatNeedsFactoryWithPropertyInjection>();


            // Assert:

            Assert.IsTrue(resolved.TheFactory != null);
            Assert.AreEqual(InjectedPropertyValue, resolved.InjectedProperty);
        }

        //TODO:
        /// <summary>
        /// Factory is injected, but property injection fails with an exception.
        /// </summary>
        [TestCategory("Failing")]
        [TestMethod]
        public void InjectFactoryAndPropertyFails()
        {
            Setup();

            _context.Container.Register(typeof(ClassToBeCreatedByFactory));  // register so that AutoFactory can create it
            _context.Container.Register(typeof(ClassThatNeedsFactoryWithPropertyInjection2));


            // This just test the setup (if the target class wasn't registered, this would throw an exception):
            TestUtil.AssertThrows(typeof(Exception), () => _context.Container.GetInstance<ClassThatNeedsFactoryWithPropertyInjection2>());


            // Act:

            TestUtil.AssertThrows(typeof(Exception),
                () => _context.Container.GetInstance<ClassThatNeedsFactoryWithPropertyInjection2>() );
        }

        protected SiExtendedDiContext _context = new SiExtendedDiContext(new SimpleInjector.Container());
    }


    public class ClassThatNeedsFactoryWithPropertyInjection : ClassThatNeedsFactory
    {
        public ClassThatNeedsFactoryWithPropertyInjection(IFactory<ClassToBeCreatedByFactory> factory)
            : base(factory)
        { }

        [Inject(InjectAttribute.CodeName)]
        public string InjectedProperty { get; set; }
    }


    public class ClassThatNeedsFactoryWithPropertyInjection2 : ClassThatNeedsFactory
    {
        public ClassThatNeedsFactoryWithPropertyInjection2(IFactory<ClassToBeCreatedByFactory> factory)
            : base(factory)
        { }

        /// <summary>
        /// Injection should fail.
        /// </summary>
        [Inject]
        public string InjectedProperty { get; set; }
    }
}
