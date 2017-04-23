using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;

namespace JohnLambe.Util.Documentation
{
    /// <summary>
    /// Base class for attributes affecting object model diagrams.
    /// </summary>
    public abstract class DocumentationAttributeBase : Attribute
    {
        /// <summary>
        /// Identifies the diagram affected by this attribute.
        /// </summary>
        public virtual string Diagram { get; set; }

        /// <summary>
        /// True to hide the attributed item on the diagram.
        /// </summary>
        public virtual NullableBool Hidden { get; set; }
    }

    /// <summary>
    /// Settings affecting the display of a type on a diagram.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class TypeDocumentationAttribute : DocumentationAttributeBase
    {
        /// <summary>
        /// True to show the attributed item collapsed on the diagram.
        /// </summary>
        public virtual NullableBool Collapsed { get; set; }

        /// <summary>
        /// Specifies what compartments are show on the diagram.
        /// </summary>
        public virtual DiagramCompartments CompartmentsVisible { get; set; }
    }

    /// <summary>
    /// Settings affecting the display of a type on a diagram.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class MemberDocumentationAttribute : DocumentationAttributeBase
    {
        /// <summary>
        /// For navigation properties and collections: Specifies how the assocation is shown on the diagram.
        /// </summary>
        public virtual DiagramAssociationType AssociationType { get; set; }
    }

    /// <summary>
    /// How an association is show on a diagram.
    /// </summary>
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
