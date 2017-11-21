using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// Exception on resolving MVP classes or interfaces (base class).
    /// </summary>
    public class MvpException : Exception
    {
        //
        // Summary:
        //     Initializes a new instance of the MvpResolverException class.
        public MvpException() { }
        //
        // Summary:
        //     Initializes a new instance of the MvpResolverException class with a specified error
        //     message.
        //
        // Parameters:
        //   message:
        //     The message that describes the error.
        public MvpException(string message) : base(message) { }
        //
        // Summary:
        //     Initializes a new instance of the MvpResolverException class with a specified error
        //     message and a reference to the inner exception that is the cause of this exception.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        //
        //   innerException:
        //     The exception that is the cause of the current exception, or a null reference
        //     (Nothing in Visual Basic) if no inner exception is specified.
        public MvpException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// An exception that occurs on resolving in the MVP framework.
    /// </summary>
    public class MvpResolutionException : MvpException
    {
        //
        // Summary:
        //     Initializes a new instance of the MvpResolverException class.
        public MvpResolutionException() { }
        //
        // Summary:
        //     Initializes a new instance of the MvpResolverException class with a specified error
        //     message.
        //
        // Parameters:
        //   message:
        //     The message that describes the error.
        public MvpResolutionException(string message) : base(message) { }
        //
        // Summary:
        //     Initializes a new instance of the MvpResolverException class with a specified error
        //     message and a reference to the inner exception that is the cause of this exception.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        //
        //   innerException:
        //     The exception that is the cause of the current exception, or a null reference
        //     (Nothing in Visual Basic) if no inner exception is specified.
        public MvpResolutionException(string message, Exception innerException) : base(message, innerException) { }

        public override string Message
            => Consts.ResolutionFailedMessage + base.Message;
    }

    /// <summary>
    /// An exception that occurs on binding in the MVP framework.
    /// </summary>
    public class MvpBindingException : MvpException
    {
        //
        // Summary:
        //     Initializes a new instance of the MvpResolverException class.
        public MvpBindingException() { }
        //
        // Summary:
        //     Initializes a new instance of the MvpResolverException class with a specified error
        //     message.
        //
        // Parameters:
        //   message:
        //     The message that describes the error.
        public MvpBindingException(string message) : base(message) { }
        //
        // Summary:
        //     Initializes a new instance of the MvpResolverException class with a specified error
        //     message and a reference to the inner exception that is the cause of this exception.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        //
        //   innerException:
        //     The exception that is the cause of the current exception, or a null reference
        //     (Nothing in Visual Basic) if no inner exception is specified.
        public MvpBindingException(string message, Exception innerException) : base(message, innerException) { }
    }
}
