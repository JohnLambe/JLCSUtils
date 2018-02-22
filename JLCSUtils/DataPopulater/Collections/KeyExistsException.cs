using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Collections
{
    /// <summary>
    /// An item with a given key, which was expected to be unique, already exists.
    /// </summary>
    public class KeyExistsException : SystemException
        //| Same superclass as KeyNotFoundException.
    {
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
        public KeyExistsException(string message = null, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
