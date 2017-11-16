using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.Attributes
{
    /// <summary>
    /// Indicates whether an attribute item (class) can be used with dependency injection
    /// (for use with tools that scan objects for potential running of DI).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SupportsInjectionAttribute : DiAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="enabled"><see cref="Enabled"/></param>
        public SupportsInjectionAttribute(bool enabled)
        {
            this.Enabled = enabled;
        }

        /// <summary>
        /// true if the attributed item can be used with DI.
        /// </summary>
        public virtual bool Enabled { get; }
    }
}
