using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnLambe.Util.Exceptions
{
    /// <summary>
    /// Exception to be raised on errors made by a user (e.g. invalid input).
    /// This can be handled by user interfaces, to show a dialog in a style appropriate for user errors.
    /// </summary>
    public class UserErrorException : Exception, IUserError
    {
        // The optional detailMessage parameter is added to each of the base class constructors, except the default one
        // (since that would be ambiguous).

        //
        // Summary:
        //     Initializes a new instance of the System.Exception class.
        public UserErrorException()
        { }

        //
        // Summary:
        //     Initializes a new instance of the System.Exception class with a specified error
        //     message.
        //
        // Parameters:
        //   message:
        //     The message that describes the error.
        public UserErrorException(string message, string detailMessage = null) : base(message)
        {
            DetailMessage = detailMessage;
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
        public UserErrorException(string message, Exception innerException, string detailMessage = null, string title = null) : base(message, innerException)
        {
            DetailMessage = detailMessage;
            Title = title;
        }

        /// <summary>
        /// Non-technical message for end-users.
        /// </summary>
        public virtual string UserMessage
            => Message;

        /// <summary>
        /// Message with more detail, such as technical details, relating to the error.
        /// <para>User interfaces may hide this from the user unless they request it by a UI action (e.g. clicking a button, or icon to expand a panel).</para>
        /// </summary>
        public virtual string DetailMessage { get; protected set; }

        /// <summary>
        /// The title to be show in the title of an error dialog or equivalent.
        /// </summary>
        public virtual string Title { get; protected set; }

    }
}
