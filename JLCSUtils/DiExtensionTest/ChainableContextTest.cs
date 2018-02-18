using DiExtension.SimpleInject;
using JohnLambe.Tests.JLUtilsTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.DiExtensionTest
{
    [TestClass]
    public class ChainableContextTest
    {
        public ChainableContextTest()
        {
            _baseContext.RegisterInstance(typeof(TestClass), new TestClass() { X = "A" });

            _context = new ChainableContext(_baseContext);
        }

        /// <summary>
        /// Resolve an item in the parent context using the child context.
        /// </summary>
        [TestMethod]
        public void ResolveFromParent()
        {
            // Act:
            var resolved = _context.GetInstance<TestClass>();

            // Assert:
            Assert.AreEqual("A", resolved.X);
        }

        /// <summary>
        /// Register an additional, independent instance in the base context, after creating the child context.
        /// </summary>
        [TestMethod]
        public void AddToBaseAndResolve()
        {
            // Arrange:
            _baseContext.RegisterInstance(typeof(TestClass2), new TestClass2() { X = "C" });

            // Act:
            var resolved = _context.GetInstance<TestClass>();
            var resolved2 = _context.GetInstance<TestClass2>();

            // Assert:
            Assert.AreEqual("A", resolved.X);
            Assert.AreEqual("C", resolved2.X);
        }

        /// <summary>
        /// Register an additional, independent instance in the child context.
        /// </summary>
        [TestMethod]
        public void AddAndResolve()
        {
            // Arrange:
            _context.RegisterInstance(typeof(TestClass2), new TestClass2() { X = "C" });

            // Act:
            var resolved = _context.GetInstance<TestClass>();
            var resolved2 = _context.GetInstance<TestClass2>();

            // Assert:
            Assert.AreEqual("A", resolved.X);
            Assert.AreEqual("C", resolved2.X);
        }

        /// <summary>
        /// Register an overriding registration in the child context.
        /// Resolving returns this instead of the parent one.
        /// </summary>
        [TestMethod]
        public void OverrideAndResolve()
        {
            // Arrange:
            _context.RegisterInstance(typeof(TestClass), new TestClass() { X = "B" });

            // Act:
            var resolved = _context.GetInstance<TestClass>();

            // Assert:
            Assert.AreEqual("B", resolved.X);
        }

        #region Child of ChainableContext

        /// <summary>
        /// Create a child context of a <see cref="ChainableContext"/>, nested two levels below the root context, and test that items are successfully registered in its setup event.
        /// </summary>
        // Note: ChainableContext can also be a root context.
        [TestMethod]
        public void SetupChildContext()
        {
            // Act:
            _context.OnSetupChildContext += _context_OnSetupChildContext;
            var context2 = _context.CreateChildContext();    // create a child of the child context

            // Assert:
            Assert.AreEqual(_testInstance.ToString(), context2.GetInstance<StringBuilder>().ToString());  // registered in the OnSetupChildContext event
            TestUtil.AssertThrows<Exception>(() => _context.GetInstance<StringBuilder>());  // not registered in parent

            // Check that normal child behaviour are not broken:
            var resolved = _baseContext.GetInstance<TestClass>();   // resolve item in root context
            Assert.AreEqual("A", resolved.X);

            ResolveFromParent();  // resolve item inherited from root to one level below root

            resolved = context2.GetInstance<TestClass>();  // resolve same item inherited from root to two levels down
            Assert.AreEqual("A", resolved.X);
        }

        private void _context_OnSetupChildContext(object sender, ChainableContext.SetupContextEventArgs args)
        {
            args.Context.RegisterInstance(typeof(StringBuilder), _testInstance);
        }
        protected StringBuilder _testInstance = new StringBuilder("test");

        #endregion

        #region classes for test

        public class TestClass
        {
            public string X { get; set; }
        }

        public class TestClass2 : TestClass
        { }

        #endregion

        protected ChainableContext _context;
        protected SiExtendedDiContext _baseContext = new SiExtendedDiContext();
    }

}
