using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace JohnLambe.Util.Logging.Log4n
{
    /// <summary>
    /// Extension methods for Log4Net.
    /// </summary>
    public static class Log4nExtension
    {
        /// <summary>
        /// Log a debug message with the message provided by a delegate.
        /// </summary>
        /// <param name="log">The log to log to.</param>
        /// <param name="message">Delegate to provide the message. Nothing is logged if this (the delegate) is null.</param>
        /// <param name="ex">An exception to be logged (if not null).</param>
        public static void Debug(this ILog log, Func<object> message, Exception ex = null)
        {
            if (log != null && message != null)
                if (log.IsErrorEnabled)
                    log.Debug(message.Invoke(), ex);
        }

        public static void Info(this ILog log, Func<object> message, Exception ex = null)
        {
            if (log != null && message != null)
                if (log.IsErrorEnabled)
                    log.Info(message.Invoke(), ex);
        }

        public static void Warn(this ILog log, Func<object> message, Exception ex = null)
        {
            if (log != null && message != null)
                if (log.IsErrorEnabled)
                    log.Warn(message.Invoke(), ex);
        }

        public static void Error(this ILog log, Func<object> message, Exception ex = null)
        {
            if (log != null && message != null)
                if (log.IsErrorEnabled)
                    log.Error(message.Invoke(), ex);
        }

        public static void Fatal(this ILog log, Func<object> message, Exception ex = null)
        {
            if (log != null && message != null)
                if (log.IsErrorEnabled)
                    log.Fatal(message.Invoke(), ex);
        }
    }
}
