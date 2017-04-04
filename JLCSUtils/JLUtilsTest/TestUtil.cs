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
        #region Testing Exceptions

        /// <summary>
        /// Assert that the given delegate throws an exception of the given type or a subtype.
        /// 
        /// <seealso cref="AssertThrows{TException}(VoidDelegate, string)"/>
        /// </summary>
        /// <param name="exceptionType">The expected exception type.</param>
        /// <param name="del">Delegate to be tested.</param>
        /// <param name="failMessage">Message to be output if the test fails (if the expected exception is not thrown).</param>
        public static void AssertThrows(Type exceptionType, VoidDelegate del, string failMessage = null)
        {
            AssertThrowsContains(exceptionType, "", del, failMessage);
        }

        /// <summary>
        /// Assert that the given delegate throws an exception of the given type or a subtype.
        /// </summary>
        /// <typeparam name="TException">The expected exception type.</typeparam>
        /// <param name="del">Delegate to be tested.</param>
        /// <param name="failMessage">Message to be output if the test fails (if the expected exception is not thrown).</param>
        public static void AssertThrows<TException>(VoidDelegate del, string failMessage = null)
            where TException : Exception
        {
            AssertThrows(typeof(TException), del, failMessage);
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

        #endregion

        #region Compare

        /// <summary>
        /// Compare two values after rounding to a specified number of decimal places.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="decimalPlaces">Number of decimal places to round the values to before comparing. (Must not be negative).</param>
        /// <param name="message">Message to output on failure.</param>
        public static void AreEqualWithPrecision(decimal expected, decimal actual, int decimalPlaces, string message = null)
        {
            decimal expectedRounded = Math.Round(expected, decimalPlaces);
            decimal actualRounded = Math.Round(actual, decimalPlaces);
            if (expected != actual)
            {
                throw new AssertFailedException("Values do not match: Expected " + expectedRounded + " (rounded from " + expected + ")\n"
                    + "Actual: " + actualRounded + " (rounded from " + actual + ")"
                    + message != null ? "\n" + message : "");
            }
        }

        /// <summary>
        /// Compare floating point numbers with an allowed percentage difference.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="allowedDifferenceProportion">The allowed difference as a proportion. e.g. 0.1 means +/- 10%.</param>
        /// <param name="message">Message to output on failure.</param>
        public static void AreEqual(double expected, double actual, double allowedDifferenceProportion, string message = null)
        {
            double difference = expected - actual;
            double differenceProportion = Math.Abs(difference) / Math.Min(expected, actual);
            if (differenceProportion > allowedDifferenceProportion)
            {
                throw new AssertFailedException("Values do not match: Expected " + expected + " +/- " + (allowedDifferenceProportion *100) + "%\n"
                    + "Actual: " + actual + " (difference of " + (differenceProportion*100) + "%)"
                    + message != null ? "\n" + message : "");
            }
        }

        #endregion

    }
}