using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util;
using System.Reflection;

namespace JohnLambe.Tests.JLUtilsTest
{
    /// <summary>
    /// Utilities for unit testing.
    /// </summary>
    public static class TestUtil
    {
        /// <summary>
        /// Assert that the given delegate throws an exception of the given type or a subtype.
        /// </summary>
        /// <param name="exceptionType">The expected exception type.</param>
        /// <param name="del">Delegate to be tested.</param>
        /// <param name="failMessage">Message to be output if the test fails (if the expected exception is not thrown).</param>
        public static void AssertThrows(Type exceptionType, VoidDelegate del, string failMessage = null)
        {
            AssertThrowsContains(exceptionType, "", del, failMessage);
        }

        /// <summary>
        /// Assert that the given delegate throws an exception of the given type or a subtype,
        /// with a message containing the given message.
        /// </summary>
        /// <param name="exceptionType">The expected exception type.</param>
        /// <param name="message">Expected message fragment.</param>
        /// <param name="failMessage">Message to be output if the test fails (if the expected exception is not thrown).</param>
        /// <param name="del">Delegate to be tested.</param>
        public static void AssertThrowsContains(Type exceptionType, string message, VoidDelegate del, string failMessage = null)
        {
            del.ArgNotNull(nameof(del));
            try
            {
                del();
            }
            catch (Exception ex)
            {
                if(ex is TargetInvocationException && ex.InnerException != null)
                    ex = ex.InnerException;    // the outer exception will be TargetInvocationException
                Console.WriteLine("Exception: " + ex);

                if (message != null && !ex.Message.Contains(message))
                {
                    Console.WriteLine("Wrong exception message");
                    throw new AssertFailedException("Assertion Failed: Wrong exception message.\n" 
                        + "Expected to contain: " + message + "; "
                        + "Actual: " + ex.Message + "\n"
                        + failMessage,
                        ex);
                }
                if (!exceptionType.IsAssignableFrom(ex.GetType()))  // if this exception could be assigned to a variable of type exceptionType
                {
                    Console.WriteLine("Wrong exception type");
                    throw new AssertFailedException("Assertion Failed: Wrong exception type.\n"
                        + "Expected: " + exceptionType + "; "
                        + "Actual: " + ex.GetType() + "\n"
                        + failMessage,
                        ex);
                }
                return;        // success
            }

            // failed - an exception should have been thrown:
            throw new AssertFailedException("No exception thrown; Expected: " + exceptionType 
                + (!string.IsNullOrEmpty(message) ? " containing message \"" + message + "\"; " : "")
                + " " + failMessage);
        }
    }
}