using JohnLambe.Util.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Documentation
{
    /// <summary>
    /// Attribute that provides documentation that may be output to an external document.
    /// </summary>
    public class DocumentationAttribute : Attribute, IDocumentationAttribute
    {
        public DocumentationAttribute(string text = null)
        {
            this.Text = text;
        }

        /// <summary>
        /// The natural language of this documentation.
        /// </summary>
        public virtual string Language { get; set; }

        public virtual string Path { get; set; }

        /// <summary>
        /// The main (body) text.
        /// </summary>
        public virtual string Text { get; set; }
        
        /// <summary>
        /// Title of this page or topic etc.
        /// </summary>
        public virtual string Title { get; set; }

        public virtual DocumentationSource Source { get; set; }

        public virtual string Format { get; set; }
    }

    public class DescriptionDocumentationAttribute : DescriptionAttribute, IDocumentationAttribute
    {
        public virtual string Language { get; set; }
        public virtual string Path { get; set; }
        /// <summary>
        /// The main (body) text.
        /// </summary>
        string IDocumentationAttribute.Text { get { return Description; } }
        /// <summary>
        /// Title of this page or topic etc.
        /// </summary>
        public virtual string Title { get; set; }
        public virtual string Format { get; set; }
        public virtual DocumentationSource Source { get; set; }
    }

    public interface IDocumentationAttribute
    {
        /// <summary>
        /// Where the documentation is to be published to.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The natural language of this documentation.
        /// </summary>
        string Language { get; }

        /// <summary>
        /// The main (body) text.
        /// </summary>
        string Text { get; }
        
        /// <summary>
        /// Title of this page or topic etc.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The format of the content of the <see cref="Text"/> property.
        /// </summary>
        string Format { get; }

        /// <summary>
        /// Specifies which parts of the XML documentation should be published
        /// to the location specified in this attribute.
        /// The <see cref="Text"/> property is published regardless of this setting.
        /// </summary>
        DocumentationSource Source { get; }
    }

    [Flags]
    public enum DocumentationSource
    {
        None = 0,

        /// <summary>
        /// The 'summary' section in the XML documentation.
        /// </summary>
        Summary = 1,

        /// <summary>
        /// The 'remarks' section in the XML documentation.
        /// </summary>
        Remarks = 2,

        /// <summary>
        /// The description from the <see cref="DescriptionAttribute"/>.
        /// </summary>
        DescriptionAttribute = 4,

        /// <summary>
        /// Documentation of any attributes on the item other than <see cref="DescriptionAttribute"/>.
        /// This includes any attributes implementing <see cref="IProvidesDescription"/>.
        /// </summary>
        Attributes = 8,

        /// <summary>
        /// The constant value of the item.
        /// Valid only on constants and static fields.
        /// When used on the latter, it reads the value that they have at the time of running the tool that uses it.
        /// This is probably only useful when the field has an initial value.
        /// </summary>
        Value = 16,

        All = Summary | Remarks | DescriptionAttribute | Attributes | Value,

        /// <summary>
        /// All values that can be used with <see cref="ReferenceAttribute"/>.
        /// </summary>
        AllReferenceable = Summary | Remarks | DescriptionAttribute | Attributes
    }

}
