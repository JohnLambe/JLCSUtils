using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Security
{
    /// <summary>
    /// Tests whether the current user has specified rights.
    /// </summary>
    public interface ISecurityManager
    {
        /// <summary>
        /// Tests whether the current user has specified rights.
        /// </summary>
        /// <param name="req">The requested right(s).</param>
        /// <param name="result">The validation result.</param>
        void ValidateRights(SecurityRequirement req, out SecurityResult result);

        /// <summary>
        /// Returns a human-readable description of a right.
        /// </summary>
        /// <param name="right">An identifier of a right, as specified in <see cref="Menu.MenuAttributeBase.Rights"/>.</param>
        /// <returns>The description.</returns>
        string DescribeRight(string right);
    }


    /// <summary>
    /// Extension methods of <seealso cref="ISecurityManager"/>.
    /// </summary>
    public static class SecurityManagerExtension
    {
        /// <summary>
        /// Throws an exception if the user does not have one of the specified rights.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="rights"></param>
        public static void AssertRights(this ISecurityManager m, string[] rights)
        {
            if (!m.ValidateRights(rights))
                throw new UserAuthenticationFailedException();
        }

        /// <summary>
        /// Returns true if the user has any of the specified rights.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        public static bool ValidateRights(this ISecurityManager m, string[] rights)
        {
            SecurityRequirement req = new SecurityRequirement()
            {
                Rights = rights
            };
            SecurityResult result;

            m.ValidateRights(req, out result);

            return result.Allowed;
        }

        /// <summary>
        /// Returns true if the user has the specified right.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool ValidateRights(this ISecurityManager manager, string right)
        {
            return manager.ValidateRights(new string[] { right });
        }
    }


    /// <summary>
    /// Specifies the rights required to access something.
    /// </summary>
    public class SecurityRequirement
    {
        /// <summary>
        /// Rights or roles required to access something.
        /// To access the item, the user must have one of the rights specified by an element of the array.
        /// <para>
        /// The format of the string depends on the consuming system. It may specify a combination of rights/roles.
        /// (So elements of the array are ORed, but rights may be ANDed within each element.)
        /// </para>
        /// </summary>
        /// <seealso cref="Menu.MenuAttributeBase.Rights"/>
        /// <seealso cref="Binding.MvpUiAttributeBase.Rights"/>
        public virtual string[] Rights { get; set; }
    }


    /// <summary>
    /// The result of testing a <see cref="SecurityRequirement"/>.
    /// </summary>
    public class SecurityResult
    {
        public SecurityResult(bool allowed, string message = null)
        {
            this.Allowed = allowed;
            this.Message = message ?? (allowed ? "Access allowed" : "Access denied");
        }

        /// <summary>
        /// True iff access is granted.
        /// </summary>
        public virtual bool Allowed { get; }

        /// <summary>
        /// A message for display to a user.
        /// This can be used to provide information on why access was denied.
        /// </summary>
        public virtual string Message { get; }

        /// <summary>
        /// Immutable instance for a successful validation.
        /// </summary>
        public static SecurityResult Success = new SecurityResult(true);

        public static SecurityResult Denied = new SecurityResult(false); 
    }

}
