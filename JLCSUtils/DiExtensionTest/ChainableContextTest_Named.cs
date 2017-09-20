using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.DiExtensionTest
{
    [TestClass]
    public class ChainableContextTest_Named : ChainableContextTest
    {
        public ChainableContextTest_Named()
        {
            _baseContext.RegisterInstance("Named1", new TestClass() { X = "Named1" });

            var resolved = _baseContext.GetValue<TestClass>("Named1", typeof(TestClass));
            Assert.AreEqual("Named1", resolved.X);
        }

        /// <summary>
        /// Resolve an item in the parent context using the child context.
        /// </summary>
        [TestMethod]
        public void ResolveFromParentNamed()
        {
            // Act:
            var resolved = _context.GetValue<TestClass>("Named1", typeof(TestClass));

            // Assert:
            Assert.AreEqual("Named1", resolved.X);
        }

        /// <summary>
        /// Register an additional, independent instance in the base context, after creating the child.
        /// </summary>
        [TestMethod]
        public void AddToBaseAndResolveNamed()
        {
            // Arrange:
            _baseContext.RegisterInstance("Named2", new TestClass2() { X = "C" });

            // Act:
            var resolved = _context.GetValue<TestClass>("Named1", typeof(TestClass));
            var resolved2 = _context.GetValue<TestClass>("Named2", typeof(TestClass));

            // Assert:
            Assert.AreEqual("Named1", resolved.X);
            Assert.AreEqual("C", resolved2.X);
        }

        /// <summary>
        /// Register an additional, independent instance in the child context.
        /// </summary>
        [TestMethod]
        public void AddAndResolveNamed()
        {
            // Arrange:
            _context.RegisterInstance("Named2", new TestClass2() { X = "C" });

            // Act:
            var resolved = _context.GetValue<TestClass>("Named1", typeof(TestClass));
            var resolved2 = _context.GetValue<TestClass>("Named2", typeof(TestClass));

            // Assert:
            Assert.AreEqual("Named1", resolved.X);
            Assert.AreEqual("C", resolved2.X);
        }

        /// <summary>
        /// Register an overriding registration in the child context.
        /// Resolving returns this instead of the parent one.
        /// </summary>
        [TestMethod]
        public void OverrideAndResolveNamed()
        {
            // Arrange:
            _context.RegisterInstance("Named1", new TestClass2() { X = "B" });

            // Act:
            var resolved = _context.GetValue<TestClass>("Named1", typeof(TestClass));

            // Assert:
            Assert.AreEqual("B", resolved.X);
        }
    }
}
