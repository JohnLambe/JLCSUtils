using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Collections;

namespace MvpFramework.Security
{
    /// <summary>
    /// An entity that can access the system (a user).
    /// </summary>
    public interface IPrincipal
    {
        /// <summary>
        /// The roles that this principal has.
        /// </summary>
        ICollection<IRole> Roles { get; }
    }

    /// <summary>
    /// A group of rights to the system that can be granted to a principal.
    /// (This can be thought of as a user group.)
    /// </summary>
    public interface IRole
    {
        /// <summary>
        /// The rights of this role.
        /// The key is an ID of a right. The returned value indicates whether this right is granted (true), or denied (false) to this role.
        /// </summary>
        IReadOnlyDictionary<string,bool> Rights { get; }

        /* Alternative:
        /// <summary>
        /// IDs of the rights granted to this role.
        /// </summary>
        ICollection<string> RightsGranted { get; }

        /// <summary>
        /// IDs of rights that this role specifically does not have.
        /// </summary>
        ICollection<string> RightsDenied { get; }
        */
    }
}
