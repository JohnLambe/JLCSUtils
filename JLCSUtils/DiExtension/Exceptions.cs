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
        //     message.
        //
        // Parameters:
        //   message:
        //     The message that describes the error.
        public DependencyInjectionException(string message) : base(message)
        { }

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
        public DependencyInjectionException(string message, Exception innerException) : base(message, innerException)
        { }

    }


    public class InjectionFailedException : DependencyInjectionException
    {
        public InjectionFailedException() : base()
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
        public InjectionFailedException(string message) : base(message)
        { }

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
        public InjectionFailedException(string message, Exception innerException) : base(message, innerException)
        { }

    }
}
