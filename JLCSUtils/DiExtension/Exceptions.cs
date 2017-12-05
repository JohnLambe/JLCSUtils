using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension
{
    /// <summary>
    /// Exception that occurs on dependency injection or setup of the container.
    /// </summary>
    public class DependencyInjectionException : Exception
    {
        public DependencyInjectionException() : base()
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Exception class with a specified error
        //     message and a reference to the inner exception that is the cause of this exception.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        //
        //   innerException:
        //     The exception that is the cause of the current exception, or a null reference
        //     (Nothing in Visual Basic) if no inner exception is specified.
        public DependencyInjectionException(string message, Type targetType = null, string memberName = null, Exception innerException = null)
            : base(message, innerException)
        {
            TargetType = targetType;
            MemberName = memberName;
        }

        public DependencyInjectionException(string message, Exception innerException)
            : this(message, null, null, innerException)
        { }

        /// <summary>
        /// The class being injected.
        /// </summary>
        public virtual Type TargetType { get; }

        /// <summary>
        /// The name of the member (parameter or property) that could not be populated.
        /// </summary>
        public virtual string MemberName { get; }
    }


    public class InjectionFailedException : DependencyInjectionException
    {
        public InjectionFailedException() : base()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="targetType">The class being injected.</param>
        /// <param name="memberName">The name of the member (parameter or property) that could not be populated.</param>
        /// <param name="innerException"></param>
        public InjectionFailedException(string message, Type targetType = null, string memberName = null, Exception innerException = null)
            : base(message, targetType, memberName, innerException)
        { }

        public InjectionFailedException(string message, Exception innerException)
            : this(message, null, null, innerException)
        { }
    }


    public class PropertyInjectionNotSupportedException : DependencyInjectionException
    {
        public PropertyInjectionNotSupportedException() : base()
        {
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Exception class with a specified error
        //     message.
        //
        // Parameters:
        //   message:
        //     The message that describes the error.
        public PropertyInjectionNotSupportedException(string message) : base(message)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="targetType">The class being injected.</param>
        /// <param name="memberName">The name of the member (parameter or property) that could not be populated.</param>
        /// <param name="innerException"></param>
        public PropertyInjectionNotSupportedException(string message, Type targetType, string memberName = null, Exception innerException = null)
            : base(message, targetType, memberName, innerException)
        { }
    }
}
