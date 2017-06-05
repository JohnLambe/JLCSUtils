using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Security
{
    public class UserAuthenticationFailedException : System.Security.Authentication.AuthenticationException
    {
        /// <summary>
        /// Message to use when none is supplied.
        /// </summary>
        public const string DefaultMessage = "You don't have the required permission for that action.";

        //
        // Summary:
        //     Initializes a new instance of the System.Security.Authentication.AuthenticationException
        //     class with the specified message and inner exception.
        //
        // Parameters:
        //   message:
        //     A System.String that describes the authentication failure.
        //
        //   innerException:
        //     The System.Exception that is the cause of the current exception.
        public UserAuthenticationFailedException(string message = null, Exception innerException = null,
            string[] requiredRights = null)
            : base(message ?? DefaultMessage, innerException)
        {
            this.RequiredRights = requiredRights;
        }

        //
        // Summary:
        //     Initializes a new instance of the System.Security.Authentication.AuthenticationException
        //     class from the specified instances of the System.Runtime.Serialization.SerializationInfo
        //     and System.Runtime.Serialization.StreamingContext classes.
        //
        // Parameters:
        //   serializationInfo:
        //     A System.Runtime.Serialization.SerializationInfo instance that contains the information
        //     required to deserialize the new System.Security.Authentication.AuthenticationException
        //     instance.
        //
        //   streamingContext:
        //     A System.Runtime.Serialization.StreamingContext instance.
        protected UserAuthenticationFailedException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        { }

        /// <summary>
        /// The rights that were required.
        /// null if not known.
        /// </summary>
        [Nullable]
        public virtual string[] RequiredRights { get; set; }
    }
}
