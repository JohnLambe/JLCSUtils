using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Security
{
    public interface ISecurityManager
    {
        /// <summary>
        /// Returns true if the user has the specified right.
        /// </summary>
        /// <param name="rights"></param>
        /// <returns></returns>
        bool ValidateRights(string rights);

        /// <summary>
        /// Returns true if the user has any of the specified rights.
        /// </summary>
        /// <param name="rights"></param>
        /// <returns></returns>
        bool ValidateRights(string[] rights);

        /// <summary>
        /// Returns a human-readable description of a right.
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        string DescribeRight(string right);
    }

    public static class SecurityManagerExtension
    {
        /*
            bool ValidateRights(this ISecurityManager m, string rights)
            {

            }
        */

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
    }

}
