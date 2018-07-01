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
        /// true to accept all requests. false to reject all requests.
        /// </summary>
        public virtual bool Allow { get; private set; }

        /// <inheritdoc cref="ISecurityManager.ValidateRights(SecurityRequirement, out SecurityResult)" />
        public void ValidateRights(SecurityRequirement r, out SecurityResult result)
        {
            result = Allow ? SecurityResult.Success : SecurityResult.Denied;
        }

        /// <inheritdoc cref="ISecurityManager.DescribeRight(string)" />
        public string DescribeRight(string right) => null;  // no description available.

        /// <summary>
        /// Immutable security manager that accepts all requests.
        /// </summary>
        public static NullSecurityManager AcceptAll { get; } = new NullSecurityManager(true);
    }

}
