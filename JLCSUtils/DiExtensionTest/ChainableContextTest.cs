using DiExtension.SimpleInject;
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
        /// Register an additional, independent instance in the base context, after creating the child.
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

        public class TestClass
        {
            public string X { get; set; }
        }

        public class TestClass2 : TestClass
        { }

        protected ChainableContext _context;
        protected SiExtendedDiContext _baseContext = new SiExtendedDiContext();
    }
}
