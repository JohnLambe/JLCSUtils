using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;

namespace JohnLambe.Util.Documentation
{
    public abstract class DocumentationAttributeBase : Attribute
    {
        public virtual string Diagram { get; set; }

        public virtual NullableBool Hidden { get; set; }

    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class TypeDocumentationAttribute : DocumentationAttributeBase
    {
        public virtual NullableBool Collapsed { get; set; }

        public virtual DiagramCompartments CompartmentsVisible { get; set; }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MemberDocumentationAttribute : DocumentationAttributeBase
    {
        public virtual DiagramAssociationType AssociationType { get; set; }
    }

    public enum DiagramAssociationType
    {
        Null = 0,
        None,
        Association,
        Collection
    }

    /// <summary>
    /// Compartments in a class/type box on a diagram.
    /// </summary>
    [Flags]
    public enum DiagramCompartments
    {
        Properties = 1,
        Methods = 2,
        Nested = 4
    }

}
