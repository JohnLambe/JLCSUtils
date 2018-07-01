using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Collections;

namespace MvpFramework.Security
{
    public abstract class SecurityManager : ISecurityManager
    {
        public virtual string DescribeRight(string right)
        {
            return right;
            //TODO
        }

        public virtual void ValidateRights(SecurityRequirement req, out SecurityResult result)
        {
            foreach(var right in req.Rights)
            {
                if (HasRight(right))
                    result = SecurityResult.Success;
            }
            result = SecurityResult.Denied;
        }

        public virtual bool HasRight(string right)
        {
            return RightsResolved.TryGetValue(right) ?? false;
        }

        /// <summary>
        /// Populates <see cref="RightsResolved"/> for <see cref="CurrentPrincipal"/>.
        /// </summary>
        protected virtual void ResolveRights()
        {
            RightsResolved.Clear();
            foreach(var role in CurrentPrincipal.Roles)
            {
                foreach(var entry in role.Rights)
                {
                    RightsResolved[entry.Key] = CombineRight(RightsResolved[entry.Key], entry.Value);
                }
            }
        }

        /// <summary>
        /// Indicates whether a right is granted or denied when the settings of two roles (or equivalent) are combined.
        /// </summary>
        /// <param name="existing"></param>
        /// <param name="additional"></param>
        /// <returns></returns>
        protected virtual bool? CombineRight(bool? existing, bool? additional)
        {
            if (existing == false || additional == false)     // if either denies the right
                return false;                                 // it is denied (as in SQL)
            else if (existing == true || additional == true)  // if either grants it (and neither denies it)
                return true;                                  // it is allowed
            else
                return null;                                  // otherwise, it is unspecified (both are null)
        }

        /// <summary>
        /// Each right that is allowed or denied for the current principal.
        /// </summary>
        protected IDictionary<string, bool?> RightsResolved { get; set; } = new Dictionary<string,bool?>();

        /// <summary>
        /// The logged in user.
        /// </summary>
        public virtual IPrincipal CurrentPrincipal
        {
            get
            {
                return _currentPrincipal;
            }
            set
            {
                _currentPrincipal = value;
                ResolveRights();
            }
        }
        private IPrincipal _currentPrincipal;
    }
}
