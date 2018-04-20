using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JohnLambe.Util;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using JohnLambe.Util.Types;

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
        public static void AssertEqualWithPrecision(decimal expected, decimal actual, int decimalPlaces, string message = null)
        {
            decimal expectedRounded = Math.Round(expected, decimalPlaces);
            decimal actualRounded = Math.Round(actual, decimalPlaces);
            if (expectedRounded != actualRounded)
            {
                OutputAndThrowException(
                    new AssertFailedException("Values do not match: Expected " + expectedRounded + " (rounded from " + expected + ")\n"
                    + "Actual: " + actualRounded + " (rounded from " + actual + ")"
                    + message != null ? "\n" + message : "")
                    );
            }
        }

        /// <summary>
        /// Compare floating point numbers with an allowed percentage difference.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="allowedDifferenceProportion">The allowed difference as a proportion. e.g. 0.1 means +/- 10%.</param>
        /// <param name="message">Message to output on failure.</param>
        public static void AssertEqual(double expected, double actual, double allowedDifferenceProportion, string message = null)
        {
            double difference = expected - actual;
            double differenceProportion = Math.Abs(difference) / Math.Min(expected, actual);
            if (differenceProportion > allowedDifferenceProportion)
            {
                OutputAndThrowException(
                    new AssertFailedException("Values do not match: Expected " + expected + " +/- " + (allowedDifferenceProportion * 100) + "%\n"
                        + "Actual: " + actual + " (difference of " + (differenceProportion * 100) + "%)"
                        + message != null ? "\n" + message : "")
                );
            }
        }

        /// <summary>
        /// Tests that the values of two items are the same (regardless of type).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        public static void AssertValueEqual(object expected, object actual, string message = null)
        {
            if (!ValueEquals(expected,actual))
            {
                OutputAndThrowException(
                    new AssertFailedException("Values do not match: Expected: " + expected + "; "
                        + "Actual: " + actual + "; " + (message ?? ""))
                );
            }
        }

        public static bool ValueEquals<T>(T a, T b)
        {
            if (a == null && b == null)
                return true;
            return a.Equals(b);
        }


        /// <summary>
        /// Write the details of an exception to the console, and raise the exception.
        /// </summary>
        /// <param name="ex"></param>
        public static void OutputAndThrowException(Exception ex)
        {
            Console.Out.WriteLine(ex.ToString());
            throw ex;
        }

        /// <summary>
        /// Test that a value is less than a given value.
        /// </summary>
        /// <typeparam name="T">The type of the value being tested.</typeparam>
        /// <param name="expected">The value which the actual value should be less then.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">Message to be output if the test fails.</param>
        /// <exception cref="NullReferenceException">If the given value is null.</exception>
        public static void AssertLessThan<T>(T expected, T actual, string message = null)
            where T : IComparable
        {
            AssertCompare(actual.CompareTo(expected) < 0, "<", expected, actual, message);
        }

        /// <summary>
        /// Test that a value is greater than a given value.
        /// </summary>
        /// <typeparam name="T">The type of the value being tested.</typeparam>
        /// <param name="expected">The value which the actual value should be greater then.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">Message to be output if the test fails.</param>
        /// <exception cref="NullReferenceException">If the given value is null.</exception>
        public static void AssertGreaterThan<T>(T expected, T actual, string message = null)
            where T : IComparable
        {
            AssertCompare(actual.CompareTo(expected) > 0, ">", expected, actual, message);
        }

        /// <summary>
        /// Test that a value is less or equal to a given value.
        /// </summary>
        /// <typeparam name="T">The type of the value being tested.</typeparam>
        /// <param name="expected">The value which the actual value should be less then.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">Message to be output if the test fails.</param>
        /// <exception cref="NullReferenceException">If the given value is null.</exception>
        public static void AssertLessThanOrEqual<T>(T expected, T actual, string message = null)
            where T : IComparable
        {
            AssertCompare(actual.CompareTo(expected) <= 0, "<=", expected, actual, message);
        }

        /// <summary>
        /// Test that a value is greater than or equal to a given value.
        /// </summary>
        /// <typeparam name="T">The type of the value being tested.</typeparam>
        /// <param name="expected">The value which the actual value should be less then.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="message">Message to be output if the test fails.</param>
        /// <exception cref="NullReferenceException">If the given value is null.</exception>
        public static void AssertGreaterThanOrEqual<T>(T expected, T actual, string message = null)
            where T : IComparable
        {
            AssertCompare(actual.CompareTo(expected) >= 0, ">=", expected, actual, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">False iff failed.</param>
        /// <param name="operatorName"></param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        private static void AssertCompare<T>(bool result, string operatorName, T expected, T actual, string message = null)
        {
            if (!result)
            {
                OutputAndThrowException(
                    new AssertFailedException("Assertion Failed: Expected: " + operatorName + expected
                    + "; Actual: " + actual + "; " + (message ?? ""))
                    );
            }
        }

        #endregion

        /// <summary>
        /// Test that a value contains a given string.
        /// </summary>
        /// <param name="expected">The string that is expected to be contained in <paramref name="value"/>.</param>
        /// <param name="value">The actual value to be inspected.
        /// (The <see cref="object.ToString()"/> method is called to get the string value to compare.)</param>
        /// <param name="message">Message to be displayed on failure.</param>
        /// <exception cref="NullReferenceException">If <paramref name="value"/> is null.</exception>
        public static void AssertContains(string expected, object value, [Nullable] string message = null)
        {
            var valueString = value.ToString();    // evaluated only once, in case ToString() returns a different result each time
            if(!valueString.Contains(expected))
            {
                OutputAndThrowException(
                    new AssertFailedException("Values does not contain expected string: Expected substring: " + expected + "; "
                        + "Actual: " + valueString + ( message != null ? "; Message: " + message : "") )
                    );
            }
        }

        /// <summary>
        /// Test that a given string is NOT contained in a value.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <seealso cref="AssertContains(string, object, string)"/>
        public static void AssertDoesNotContain(string expected, object value, [Nullable] string message = null)
        {
            var valueString = value.ToString();    // evaluated only once, in case ToString() returns a different result each time
            if (valueString.Contains(expected))
            {
                OutputAndThrowException(
                    new AssertFailedException("Values contains unexpected string: Required to no contain: " + expected + "; "
                        + "Actual: " + valueString + (message != null ? "; Message: " + message : ""))
                    );
            }
        }

        /// <summary>
        /// Run multiple tests, report the results to Console output,
        /// and report the worst outcome as the outome of this test.
        /// <para>The InnerException of the exception thrown is the exception thrown on the first failed test.</para>
        /// <para>The tests are run in series, in the order given.</para>
        /// </summary>
        /// <param name="delegates">The tests to be run.</param>
        /// <exception/>
        public static void Multiple(params VoidDelegate[] delegates)
        {
            TestResults results = new TestResults();
            StringBuilder errors = new StringBuilder();
            Exception firstFailure = null;

            var tests = new LinkedList<TestResult>();

            foreach(var dlg in delegates)
            {
                Exception ex = null;
                UnitTestOutcome outcome = UnitTestOutcome.Unknown;

                results.Tests++;
                Console.Out.WriteLine("------ Test " + results.Tests + " ------------");

                try
                {
                    dlg?.Invoke();

                    outcome = UnitTestOutcome.Passed;
                }
                catch (Exception originalException)
                {
                    Console.Out.WriteLine(originalException);
                    if (originalException is TargetInvocationException)  //TODO: Get original stack trace. It's not TargetInvocationException
                    {
                        ex = originalException.InnerException ?? originalException;  // get the Inner Exception (if there is one)
                    }
                    else
                    {
                        ex = originalException;
                    }

                    string error = "";
                    if (ex is AssertInconclusiveException)
                    {
                        outcome = UnitTestOutcome.Inconclusive;
                        results.InconclusiveCount++;
                        error = "TEST INCONCLUSIVE: " + ex.Message;
                    }
                    else if (ex is AssertFailedException)
                    {
                        outcome = UnitTestOutcome.Failed;
                        results.FailedCount++;
                        error = "TEST FAILED: " + ex.Message;
                    }
                    else
                    {
                        outcome = UnitTestOutcome.Error;
                        results.FailedCount++;
                        error = "EXCEPTION: " + ex.ToString();
                    }
                    firstFailure = firstFailure ?? ex;

                    Console.Out.WriteLine("\n" + error);

                    errors.AppendLine("Test " + results.Tests + ": " + outcome + "\n"
                        + error + "\n\n");
                }
                Console.Out.WriteLine("------ Outcome: " + outcome + " ------");
                Console.Out.WriteLine();

                tests.AddLast(new TestResult()
                {
                    Id = results.Tests.ToString(),
                    Outcome = outcome,
                    Exception = ex
                });
            }

            Console.WriteLine("----- Results ----------");
            foreach(var test in tests)
            {
                Console.WriteLine(test);
            }

            string summary = "Tests: " + results.Tests
                + "    Passed: " + results.Passed
                + "    Failed: " + results.FailedCount
                + "    Inconclusive: " + results.InconclusiveCount;

            Console.Out.WriteLine(summary);

            summary = summary 
                + ("\n\n" + errors.ToString().TrimEnd());

            if (results.FailedCount > 0)   // at least one failed
            {
                throw new AssertFailedException("Test Failed.  " + summary, firstFailure);
            }
            else if(results.InconclusiveCount > 0)    // at least one Inconclusive, but none failed
            {
                throw new AssertInconclusiveException("Test Inconclusive.  " + summary, firstFailure);
            }

            lock(Results)
            {
                Results.Add(results);
            }
        }

        /// <summary>
        /// The totals for all tests run through <see cref="Multiple(VoidDelegate[])"/>.
        /// </summary>
        public static readonly TestResults Results = new TestResults();

        /// <summary>
        /// Set of counts of test results.
        /// </summary>
        public class TestResults
        {
            /// <summary>
            /// Number of tests run.
            /// </summary>
            public virtual int Tests { get; set; }
            /// <summary>
            /// Number of tests that failed.
            /// <para>
            /// <see cref="UnitTestOutcome.Error"/> and <see cref="UnitTestOutcome.Timeout"/>
            /// are counted as failures.
            /// </para>
            /// </summary>
            public virtual int FailedCount { get; set; }
            /// <summary>
            /// Number of inconclusive tests (i.e. with outcome of <see cref="UnitTestOutcome.Inconclusive"/>).
            /// </summary>
            public virtual int InconclusiveCount { get; set; }
            /// <summary>
            /// Number of passed tests.
            /// </summary>
            public virtual int Passed => Tests - InconclusiveCount - FailedCount;
            /// <summary>
            /// Number of groups of tests that are combined in these results.
            /// </summary>
            public virtual int Groups { get; set; }

            /// <summary>
            /// Aggregate a set of results with this one.
            /// </summary>
            /// <param name="results"></param>
            public virtual void Add(TestResults results)
            {
                Tests += results.Tests;
                FailedCount += results.FailedCount;
                InconclusiveCount += results.InconclusiveCount;
                Groups++;
            }

            /// <summary>
            /// Reset all counts to 0.
            /// </summary>
            public virtual void Clear()
            {
                Tests = 0;
                FailedCount = 0;
                InconclusiveCount = 0;
                Groups = 0;
            }
        }

        public class TestResult
        {
            public virtual string Id { get; set; }
            public virtual UnitTestOutcome Outcome { get; set; }
            public virtual Exception Exception { get; set; }

            public override string ToString()
            {
                return Id.PadRight(10) + " " + Outcome.ToString().PadRight(15) + " " +
                    StrUtil.Truncate(Exception?.Message.ExtractDelimited(null, new char[] { '\r', '\n' }, true), 80);
            }
        }

    }
}