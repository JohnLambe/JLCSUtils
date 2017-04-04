using JohnLambe.Util.Misc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Tests.JLUtilsTest.Misc
{
    [TestClass]
    public class GeneralUtilsTest
    {
        #region IgnoreException

        /// <summary>
        /// Exception should be suppressed. No default value is explicity given (default for the type is used).
        /// </summary>
        [TestMethod]
        public void IgnoreException_Suppressed_Default()
        {
            // Setup:
            int y = 0;

            // Act:
            int x = GeneralUtil.IgnoreException(() => 10 / y, typeof(DivideByZeroException));

            // Assert:
            Assert.AreEqual(0, x);
        }

        /// <summary>
        /// An exception which is a subclass of the declared expected one is suppressed.
        /// A default value is given.
        /// </summary>
        [TestMethod]
        public void IgnoreException_Suppressed()
        {
            // Setup:
            const int ExpectedResult = -5000;
            int y = 0;

            // Act:
            int x = GeneralUtil.IgnoreException(() => 10 / y, typeof(ArithmeticException), ExpectedResult);

            // Assert:
            Assert.AreEqual(ExpectedResult, x);
        }

        /// <summary>
        /// An exception of an unexpected type is thrown and not suppressed.
        /// </summary>
        [TestMethod]
        public void IgnoreException_NotSuppressed()
        {
            // Setup:
            int y = 0;

            // Act:
            TestUtil.AssertThrows(typeof(DivideByZeroException),
                () => GeneralUtil.IgnoreException(() => 10 / y, typeof(OverflowException), 100)
                );
        }

        /// <summary>
        /// No exception is thrown.
        /// </summary>
        [TestMethod]
        public void IgnoreException_NoException()
        {
            // Act:
            int x = GeneralUtil.IgnoreException(() => 10 / 5, typeof(ArithmeticException), 1234);

            // Assert:
            Assert.AreEqual(10 / 5, x);
        }

        #endregion

    }
}
