using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Core;

namespace JohnLambe.Util.Logging.Log4n
{
    /// <summary>
    /// Adapts a Log4Net <see cref="ILog"/>to <see cref="IGeneralLogger"/>.
    /// </summary>
    public class Log4nAdapter : IGeneralLogger
    {
        public Log4nAdapter(ILog log)
        {
            this.Log4n = log;
        }

        /// <summary>
        /// The underlying logger.
        /// </summary>
        protected ILog Log4n { get; }

        public virtual int LogLevel
        {
            get
            {
                if (Log4n.IsFatalEnabled)
                    return Logging.LogLevel.Fatal;
                if (Log4n.IsErrorEnabled)
                    return Logging.LogLevel.Error;
                if (Log4n.IsWarnEnabled)
                    return Logging.LogLevel.Warn;
                if (Log4n.IsInfoEnabled)
                    return Logging.LogLevel.Info;
                if (Log4n.IsDebugEnabled)
                    return Logging.LogLevel.Debug;
                return Logging.LogLevel.None;
            }
        }

        public virtual int NestingLevel => _nestingLevel;
        protected int _nestingLevel = 0;

        public virtual Exception DownLevel(int level = -1, string loggerName = null, object message = null)
        {
            _nestingLevel++;
            return Log(level, message);
        }

        public virtual Exception Log(int level, Exception exception = null)
        {
            return Log(level, null, exception);
        }

        public virtual Exception Log(int level, object message, Exception exception = null)
        {
            if (Log4n != null)
            {
                switch (Log4NetLevel)
                {
                    case Logging.LogLevel.Fatal:
                        Log4n.Fatal(message, exception);
                        break;
                    case Logging.LogLevel.Error:
                        Log4n.Error(message, exception);
                        break;
                    case Logging.LogLevel.Warn:
                        Log4n.Warn(message, exception);
                        break;
                    case Logging.LogLevel.Info:
                        Log4n.Info(message, exception);
                        break;
                    default:  // Logging.LogLevel.Debug
                        Log4n.Debug(message, exception);
                        break;
                }
            }
            return null;
        }

        public virtual Exception LogFormat(int level, IFormatProvider provider, string format, params object[] args)
        {
            if (Log4n != null)
            {
                switch (Log4NetLevel)
                {
                    case Logging.LogLevel.Fatal:
                        Log4n.FatalFormat(provider, format, args);
                        break;
                    case Logging.LogLevel.Error:
                        Log4n.ErrorFormat(provider, format, args);
                        break;
                    case Logging.LogLevel.Warn:
                        Log4n.WarnFormat(provider, format, args);
                        break;
                    case Logging.LogLevel.Info:
                        Log4n.InfoFormat(provider, format, args);
                        break;
                    default:  // Logging.LogLevel.Debug
                        Log4n.DebugFormat(provider, format, args);
                        break;
                }
            }
            return null;
        }

        public virtual Exception LogFormat(int level, string format, params object[] args)
        {
            if (Log4n != null)
            {
                switch (Log4NetLevel)
                {
                    case Logging.LogLevel.Fatal:
                        Log4n.FatalFormat(format, args);
                        break;
                    case Logging.LogLevel.Error:
                        Log4n.ErrorFormat(format, args);
                        break;
                    case Logging.LogLevel.Warn:
                        Log4n.WarnFormat(format, args);
                        break;
                    case Logging.LogLevel.Info:
                        Log4n.InfoFormat(format, args);
                        break;
                    default:  // Logging.LogLevel.Debug
                        Log4n.DebugFormat(format, args);
                        break;
                }
            }
            return null;
        }

        public virtual Exception UpLevel()
        {
            _nestingLevel--;
            //TODO: write messsage
            if (_nestingLevel < 0)
                return new InvalidOperationException("Already at top level");
            return null;
        }

        /// <summary>
        /// The log level to use for logging with <see cref="ILog"/>.
        /// This is a <see cref="Logging.LogLevel"/> value, but is restricted to only return values that have a corresponding method in <see cref="ILog"/>.
        /// </summary>
        protected int Log4NetLevel
        {
            get
            {
                if (LogLevel >= Logging.LogLevel.Fatal)
                    return Logging.LogLevel.Fatal; // Level.Fatal;
                if (LogLevel >= Logging.LogLevel.Error)
                    return Logging.LogLevel.Error; // Level.Error;
                if (LogLevel >= Logging.LogLevel.Warn)
                    return Logging.LogLevel.Warn; // Level.Warn;
                if (LogLevel >= Logging.LogLevel.Info)
                    return Logging.LogLevel.Info; // Level.Info;
                //                if (LogLevel <= Logging.LogLevel.Debug)
                return Logging.LogLevel.Debug; // Level.Debug;
            }
        }
    }
}
