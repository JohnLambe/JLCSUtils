using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// Controls the display of the attributed property in tabular views / reports / grids.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class ColumnDisplayAttribute : Attribute
    {
        /// <summary>
        /// Specifies which views etc. these settings apply to.
        /// </summary>
        public virtual string[] Filter { get; set; }

        /// <summary>
        /// Sorting order.
        /// </summary>
        public virtual int Order { get; set; }

        /// <summary>
        /// Set to false to effectively remove the attribute for the specified Filter values.
        /// </summary>
        public virtual bool Enabled { get; set; }

        /// <summary>
        /// Sorting order.
        /// </summary>
        public virtual bool Descending { get; set; }
        //| Could use an enumeration ( Ascending, Descending ).

        public virtual string Title { get; set; }  // ? Could use DataAnnotations instead.

        /// <summary>
        /// 
        /// </summary>
        public virtual int DisplayWidth { get; set; }

        /// <summary>
        /// True iff this item should be shown in the grid.
        /// </summary>
        public virtual bool Visible { get; set; }
    }

    //| Attribute on class for Report/View -level settings ?
}
