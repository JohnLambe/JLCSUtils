using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Exceptions
{
    /// <summary>
    /// An error due to something internal to the system, (apparently) not caused by the user, nor the environment,
    /// e.g. an invalid state that must be the result of a bug.
    /// (An error that should never happen.)
    /// </summary>
    public class InternalErrorException : Exception, IUserError
    {
        #region System-wide configuration

        /// <summary>
        /// Message displayed to the user on an internal error.
        /// </summary>
        public static string InternalErrorMessage { get; set; } = "INTERNAL ERROR: An internal error has occurred";

        public static string InternalErrorMessageFormat { get; set; } = FormatToken_IE + "\n" + FormatToken_MSG;

        public const string FormatToken_IE = "<IE>";
        public const string FormatToken_MSG = "<MSG>";

        #endregion

        //
        // Summary:
        //     Initializes a new instance of the System.Exception class.
        public InternalErrorException() : base() { }
        //
        // Summary:
        //     Initializes a new instance of the System.Exception class with a specified error
        //     message.
        //
        // Parameters:
        //   message:
        //     The message that describes the error.
        public InternalErrorException(string message) : base(message) { }
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
        public InternalErrorException(string message, Exception innerException)
            : base(InternalErrorMessageFormat.Replace(FormatToken_IE, InternalErrorMessage)
            .Replace(FormatToken_MSG, message), innerException) { }

        public virtual string DetailMessage
            => Message;

        public virtual string UserMessage
            => "INTERNAL ERROR: An internal error has occurred";
    }


    /// <summary>
    /// Exception to be thrown when an assertion fails.
    /// </summary>
    public class AssertionFailedException : InternalErrorException, IUserError
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Exception class.
        public AssertionFailedException() : base("INTERNAL ERROR:\nASSERTION FAILED")
        { }

        //
        // Summary:
        //     Initializes a new instance of the System.Exception class with a specified error
        //     message.
        //
        // Parameters:
        //   message:
        //     The message that describes the error.
        public AssertionFailedException(string message, Exception innerException = null) : base(message, innerException)
        {
            AssertionMessage = message;
        }

        public virtual string AssertionMessage { get; protected set; }

        public override string DetailMessage
            => "ASSERTION FAILURE: " + AssertionMessage;

        public override string UserMessage
            => InternalErrorMessage;
    }

}
