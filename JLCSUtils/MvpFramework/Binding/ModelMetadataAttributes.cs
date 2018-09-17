using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    // Attributes that provide metadata about a model, which may be useful in frameworks, or custom form generation logic:

    /// <summary>
    /// Indicates that the attributed property is a reference to the parent of the object on which the property is defined.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ParentPropertyAttribute : Attribute
    {
    }

    /// <summary>
    /// Indicates that the attributed property should be populated (or required to be entered by the user) before other properties (not attributed with this) of the instance are used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PrePopulateAttribute : Attribute
    {
        /// <summary>
        /// When multiple properties of the same class have this attribute, they should be entered/populated in ascending order of this value.
        /// (Where properties have the same value, they can be entered/populates in any order.)
        /// </summary>
        public virtual int Order { get; set; }
    }
}
