using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Security
{
    /// <summary>
    /// A password or password hash does not meet the security policy requirements (e.g. a password that is too short, or a password hash that did not use enough KDF iterations).
    /// </summary>
    public class PasswordRejectedException : ApplicationException
    {
    }
}
