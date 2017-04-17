using JohnLambe.Util.Reflection;
using JohnLambe.Util.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// Attributes for building queryies for the attributes class,
    /// and/or specifying a tabular layout.
    /// </summary>
    public abstract class GridAttribute : Attribute, IFilteredAttribute
    {
        /// <summary>
        /// Set to false to effectively remove the attribute for the specified Filter values.
        /// </summary>
        public virtual bool Enabled { get; set; }

        /// <summary>
        /// Specifies which views/grids etc. these settings apply to.
        /// </summary>
        public virtual string[] Filter { get; set; }

        /// <summary>
        /// Sorting order.
        /// e.g. Columns of a grid are sorting (left to right) in ascending order of this value.
        /// </summary>
        public virtual int Order { get; set; }
    }


    /// <summary>
    /// Controls the display of the attributed property in tabular views / reports / grids.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class ColumnDisplayAttribute : GridAttribute
    {
        /// <summary>
        /// Human-readable caption (e.g. column title) for the attributed item.
        /// </summary>
        public virtual string DisplayName { get; set; }  // ? Could use DataAnnotations instead.

        /// <summary>
        /// The width of the column etc. for the attributed item.
        /// </summary>
        public virtual int DisplayWidth { get; set; }

        [PercentageValidation]
        public virtual int DisplayWidthPercentage { get; set; }

        /// <summary>
        /// True iff this item should be shown in the grid.
        /// </summary>
        public virtual bool Visible { get; set; }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class ColumnFilterAttribute : GridAttribute
    {
        public virtual object MinimumValue { get; set; }

        public virtual object MaximumValue { get; set; }

        public virtual object Value
        {
            get
            {
                return MinimumValue;
            }
            set
            {
                MinimumValue = value; MaximumValue = value;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class SortOrder : Attribute
    {
        /// <summary>
        /// Whether sorting on the attributed item is ascending or descending.
        /// <para>This can be set to <see cref="SortDirection.None"/> to override an attribute on a superclass
        /// and cause the item to not be included in the sorting order.</para>
        /// </summary>
        public virtual SortDirection Direction { get; set; } = SortDirection.Ascending;
        //TODO: Using SortDirection.None here is redundant. Enabled=false would do the same.
    }


    public enum SortDirection
    {
        /// <summary>
        /// Don't sort on this item.
        /// </summary>
        None,
        
        /// <summary>
        /// Sort in ascending order.
        /// </summary>
        Ascending,

        /// <summary>
        /// Sort in descending order.
        /// </summary>
        Descending
    }


    //TODO: Filtering.
    //TODO: Use methods to dynamically generate parts of the query.


    //| Attribute on class for Report/View -level settings ?
}
