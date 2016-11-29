using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    /// <summary>
    /// Specifies details of how calls to a method should be logged, for use by a framework that
    /// adds logging by intercepting method calls.
    /// If placed on a class, it specifies a default value for all methods of it (which can be overridden
    /// by placing this attribute on individual methods).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class LoggingAttribute : Attribute
    {
        /// <summary>
        /// Default value for LogLevel.
        /// </summary>
        public const int LogLevel_Default = 500;

        public LoggingAttribute(int logLevel = LogLevel_Default)
        {
            this.LogLevel = logLevel;
        }

        public virtual int LogLevel { get; set; }

        /// <summary>
        /// Semi-colon-separated list of keys, each specifying a category, which can
        /// be used for filtering (in the logging configuration).
        /// </summary>
        public virtual string Category { get; set; }
    }
}
