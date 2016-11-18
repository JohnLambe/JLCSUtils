using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Documentation
{
    public abstract class DocumentationAttributeBase : Attribute
    {
        public virtual string Diagram { get; set; }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class HideDocumentationAttribute : DocumentationAttributeBase
    {
        public virtual bool Hidden { get; set; } = true;
    }
    // Could do the same for Collapse.

}
