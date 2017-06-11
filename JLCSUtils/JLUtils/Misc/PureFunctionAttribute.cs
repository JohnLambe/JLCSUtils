using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    /// <summary>
    /// Indicates whether the attributes item is a pure funtction.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class PureFunctionAttribute : Attribute
    {
        public PureFunctionAttribute(bool pure)
        {
            Pure = pure;
        }

        public virtual bool Pure { get; set; }
    }
}
