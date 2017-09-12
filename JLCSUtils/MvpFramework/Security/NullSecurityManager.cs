using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Security
{
    /// <summary>
    /// An <see cref="ISecurityManager"/> that accepts or rejects all requests.
    /// </summary>
    public class NullSecurityManager : ISecurityManager
    {
        /// <summary>
        /// </summary>
        /// <param name="allow"><see cref="Allow"/> (defaults to true).</param>
        public NullSecurityManager(bool allow = true)
        {
            this.Allow = allow;
        }

        /// <summary>
        /// True to accept all requests. False to reject all requests.
        /// </summary>
        public virtual bool Allow { get; private set; }

        /// <inheritdoc cref="ISecurityManager.ValidateRights(SecurityRequirement, out SecurityResult)" />
        public void ValidateRights(SecurityRequirement r, out SecurityResult result)
        {
            result = new SecurityResult() { Allowed = Allow };
        }

        /// <inheritdoc cref="ISecurityManager.DescribeRight(string)" />
        public string DescribeRight(string right) => null;  // no description available.
    }

}
