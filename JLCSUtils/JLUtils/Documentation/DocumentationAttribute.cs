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
        public virtual DocumentationSource Source { get; set; }
    }

    public interface IDocumentationAttribute
    {
        string Language { get; }
        string Path { get; }
        /// <summary>
        /// The main (body) text.
        /// </summary>
        string Text { get; }
        /// <summary>
        /// Title of this page or topic etc.
        /// </summary>
        string Title { get; }
        DocumentationSource Source { get; }
    }

    [Flags]
    public enum DocumentationSource
    {
        Summary = 1,
        Remarks = 2
    }

}
