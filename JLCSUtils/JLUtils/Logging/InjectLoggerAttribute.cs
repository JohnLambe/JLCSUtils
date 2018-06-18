using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Logging
{
    /// <summary>
    /// Indicates that logger should be injected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class InjectLoggerAttribute : Attribute
    {
        public virtual string Name { get; set; }
    }
}
