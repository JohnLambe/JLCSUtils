using JohnLambe.Util.Exceptions;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.TimeUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Diagnostic
{
    public static class Diagnostics
    {
        public static int AssertionLevel { get; set; } = 5;

        /// <summary>
        /// Iff true, call <see cref="Debug.Assert(bool, string)"/> when an assertion fails.
        /// </summary>
        public static bool UseDebug { get; set; } = false;

        public const int Level_Low = 2;

        #region Assertions

        private enum AssertionCategory
        {
            General = 1,
            /// <summary>
            /// A condition that should be true on entering a module, method, etc. (to validate the input).
            /// </summary>
            PreCondition,
            /// <summary>
            /// A condition that should be true of leaving a module, method, etc. (to verify that the item that has finished behaved correctly).
            /// </summary>
            PostCondition
        }

        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        public static void SetLevel(int level)
        {
            AssertionLevel = level;
        }

        #region Internal

        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AssertInternal(bool condition, string message = null, AssertionCategory category = AssertionCategory.General, Type exceptionClass = null)
        {
            if (!condition)
            {
                AssertionFailed(message, category, exceptionClass);
            }
        }

        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AssertInternal(int level, Func<bool> condition, Func<string> message = null, AssertionCategory category = AssertionCategory.General, Type exceptionClass = null)
        {
            if (level < AssertionLevel)
            {
                AssertInternal(condition.Invoke(), message, category);
            }
        }

        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AssertInternal(int level, bool condition, Func<string> message = null, AssertionCategory category = AssertionCategory.General, Type exceptionClass = null)
        {
            if (AssertionLevel > level)
            {
                AssertInternal(condition, message, category);
            }
        }

        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AssertInternal(int level, bool condition, string message = null, AssertionCategory category = AssertionCategory.General, Type exceptionClass = null)
        {
            if (AssertionLevel > level)
            {
                if (!condition)
                {
                    AssertionFailed(message, category, exceptionClass);
                }
            }
        }

        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AssertInternal(int level, Func<bool> condition, string message = null, AssertionCategory category = AssertionCategory.General, Type exceptionClass = null)
        {
            if (AssertionLevel > level)
            {
                AssertInternal(condition.Invoke(), message, category, exceptionClass);
            }
        }

        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        private static void AssertionFailed(string message, AssertionCategory category, Type exceptionClass = null)
        {
            Log("ASSERTION FAILED: " + message);
            if (UseDebug)
                Debug.Assert(false, message);
            else if (exceptionClass == null)
                throw new AssertionFailedException(message);
            //TODO: Get stack trace
            else
                throw ReflectionUtil.Create<Exception>(exceptionClass, message);
        }

        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AssertInternal(bool condition, Func<string> message, AssertionCategory category = AssertionCategory.General)
        {
            if (!condition)
            {
                AssertionFailed(message.Invoke(), category);
            }
        }

        #endregion

        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Assert(bool condition, string message = null)
        {
            AssertInternal(condition, message);
        }

        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        public static void Assert(bool condition, Func<string> message)
        {
            AssertInternal(condition, message);
        }

        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        public static void Assert(int level, Func<bool> condition, Func<string> message)
        {
            AssertInternal(level, condition, message);
        }

        /// <summary>
        /// Test a condition on entry, e.g. validating inputs.
        /// </summary>
        /// <typeparam name="TException">The type of exception to be thrown on failure (if the condition is false).</typeparam>
        /// <param name="condition">The condition that is expected to be true.</param>
        /// <param name="message">Message to describe the failure (exception message on failure).</param>
        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        public static void PreCondition<TException>(bool condition, string message = null)
        {
            AssertInternal(condition, message, AssertionCategory.PreCondition, typeof(TException));
        }

        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        public static void PreCondition(bool condition, string message = null)
        {
            PreCondition<AssertionFailedException>(condition, message);
        }

        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS")]
        [Conditional("ASSERTIONS_LOW")]
        public static void PostCondition<TException>(bool condition, string message = null)
        {
            AssertInternal(condition, message, AssertionCategory.PostCondition, typeof(TException));
        }

        /// <summary>
        /// Assertion that is less important (than other assertions) or may be slow to evaluate.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS_LOW")]
        public static void AssertLow(Func<bool> condition, Func<string> message)
        {
            AssertInternal(Level_Low, condition, message);
        }

        /// <summary>
        /// Assertion that is less important (than other assertions) or may be slow to evaluate.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        [Conditional("ASSERTIONS_LOW")]
        public static void AssertLow(bool condition, Func<string> message)
        {
            AssertInternal(Level_Low, condition, message);
        }

        /// <summary>
        /// Assertion that is less important (than other assertions) or may be slow to evaluate.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        [Conditional("ASSERTIONS_LOW")]
        public static void AssertLow(bool condition, string message = null)
        {
            AssertInternal(condition, message);
        }

        #endregion

        #region Logging

        [Conditional("DEBUG")]
        [Conditional("LOGGING")]
        public static void Log(string message)
        {
            Console.WriteLine(TimeUtil.NowIso8601() + "\t" + message);
        }

        #endregion

    }
}
