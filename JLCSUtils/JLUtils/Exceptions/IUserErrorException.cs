using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Exceptions
{
    /// <summary>
    /// Interface to be implemented by exception classes, with details for separating the message given to the user,
    /// and technical details.
    /// </summary>
    public interface IUserErrorException
    {
        /// <summary>
        /// Error message for end-users.
        /// </summary>
        string UserMessage { get; }

        /// <summary>
        /// Message with more detail, such as technical details, relating to the error.
        /// <para>User interfaces may hide this from the user unless they request it by a UI action (e.g. clicking a button, or icon to expand a panel).</para>
        /// </summary>
        string DetailMessage { get; }

        //        Type ErrorCategory { get; }  ?
    }
}
