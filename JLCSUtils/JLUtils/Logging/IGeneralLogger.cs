using System;

namespace JohnLambe.Util.Logging
{
    /// <summary>
    /// Interface for appending messages to a log.
    /// Logging methods return null on success, and an <see cref="Exception"/> on failure.
    /// </summary>
    public interface IGeneralLogger
    {
        int LogLevel { get; }

        Exception Log(int level, Object message, Exception exception = null);

        Exception Log(int level, Exception exception = null);

        #region Formatted messages

        // Inspired by Log4Net:

        Exception LogFormat(int level, string format, params object[] param);
        //| Named 'LogFormat' rather than 'LogFormatted' for consistency with Log4Net.

        Exception LogFormat(int level, IFormatProvider provider, string format, params object[] args);

        #endregion

        #region Nested logging

        /// <summary>
        /// Increase the nesting/indentation level (or equivalent),
        /// optionally logging a message to describe what the lower level relates to.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="loggerName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Exception DownLevel(int level = -1, string loggerName = null, object message = null);

        /// <summary>
        /// Decrease the nesting/indentation level (or equivalent).
        /// </summary>
        /// <returns></returns>
        Exception UpLevel();

        int NestingLevel { get; }

        /* Alternative:
        // 

        /// <summary>
        /// Create a nested logger.
        /// A nested logger logs messages at a lower level, such as for a subcomponent.
        /// Must be closed by calling <see cref="Close"/> on the returned logger.
        /// This logger must not log anything until the nested one is closed.
        /// </summary>
        /// <param name="level">The maximum level that the nsted logger can log.
        /// If this is lower than the currently configured level of this logger, a null logger is returned.
        /// </param>
        /// <returns></returns>
        ILogger GetSubLogger(int level = -1, string loggerName = null, object message = null);

        bool Close();
        */

        #endregion
    }

    /// <summary>
    /// Values of the 'level' parameter when logging with <see cref="IGeneralLogger"/>.
    /// Higher values indicate a higher level of abstraction or lower detail level.
    /// </summary>
    public static class LogLevel
    {
        /// <summary>
        /// Level for logging everthing.
        /// </summary>
        public const int All = 0;  // could be int.MinValue

        /// <summary>
        /// For verbose diagnostic details.
        /// </summary>
        public const int Finest = 10000;  // Verbose
        /// <summary>
        /// For diagnostic details.
        /// </summary>
        public const int Debug = 30000;
        /// <summary>
        /// For informational (not error or warning) messages.
        /// </summary>
        public const int Info = 40000;
        //| Named 'Warn' rather than 'Warning' for consistency with Log4Net.
        public const int Warn = 60000;
        public const int Error = 70000;
        //| Named 'Fatal' rather than 'FatalError' for consistency with Log4Net.
        public const int Fatal = 110000;

        /// <summary>
        /// Level for logging nothing.
        /// </summary>
        public const int None = int.MaxValue;
    }

    public static class LoggerExtension
    {
        /// <summary>
        /// Log a message provided by a delegate.
        /// The delegate is evaluated only if the logger is configured to log at the given level.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="level"></param>
        /// <param name="message">Delegate to return the message. Can be null to make the message null.</param>
        /// <param name="ex"></param>
        public static void Log<T>(this IGeneralLogger logger, int level, Func<T> message, Exception ex = null)
        {
            if (logger != null && message != null || ex != null)
            {
                if (logger.LogLevel > level)
                    logger.Log(level, message == null ? null : (object)message.Invoke(), ex);
            }
        }
    }

    /// <summary>
    /// Interface for a class that has a logger.
    /// </summary>
    public interface IHasLogger
    {
        IGeneralLogger Logger { get; set; }
    }
}
