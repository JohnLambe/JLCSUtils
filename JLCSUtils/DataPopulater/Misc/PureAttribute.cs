using JohnLambe.Util.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    /// <summary>
    /// Indicates whether the attributed item is functionally pure.
    /// <para>
    /// Anything that calls anything that is not functionally pure, is not functionally pure itself.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class PureAttribute : Attribute, IEnabledAttribute
    {
        /// <summary>
        /// true (the default) means that the attributed item is functioanlly pure.
        /// false means that it is not.
        /// </summary>
        public virtual bool Enabled { get; set; } = true;

        public override bool IsDefaultAttribute()
        {
            return Enabled;         // it is in the default state when Enabled is true.
        }
    }
}
