using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Validation;
using System.Reflection;
using JohnLambe.Util.Exceptions;

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
        public virtual bool Enabled { get; set; } = true;

        /// <summary>
        /// Specifies which views/grids etc. these settings apply to.
        /// </summary>
        public virtual string[] Filter { get; set; }

        /// <summary>
        /// Sorting order.
        /// e.g. Columns of a grid are sorted (in the text reading direction (left to right for English)) in ascending order of this value.
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
        public virtual int DisplayWidth { get; set; } = -1;

        public virtual int DisplayMinimumWidth { get; set; } = -1;
        public virtual int DisplayMaximumWidth { get; set; } = -1;


        [PercentageValidation]
        public virtual int DisplayWidthPercentage { get; set; } = -1;

        /// <summary>
        /// True iff this item should be shown in the grid.
        /// </summary>
        public virtual bool Visible { get; set; } = true;

        #region UI capabilities

        /// <summary>
        /// The UI should allow sorting on this field.
        /// </summary>
        public virtual bool AllowSorting { get; set; }

        /// <summary>
        /// The UI should allow filtering on this field.
        /// </summary>
        public virtual bool AllowFiltering { get; set; }

        #endregion
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class ColumnFilterBaseAttribute : GridAttribute
    {
        public abstract bool TestFilter<T>(T value);
    }

    public class ColumnFilterAttribute : ColumnFilterBaseAttribute
    {
        public virtual object MinimumValue { get; set; }

        public virtual object MaximumValue { get; set; }

        /// <summary>
        /// If &gt; "", filters to values beginning with this string (based on the result of <see cref="object.ToString"/> of the value).
        /// </summary>
        public virtual string StartsWith { get; set; }

        /// <summary>
        /// Iff true, the filter condition is negated.
        /// </summary>
        public virtual bool Not { get; set; } = false;

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

        public override bool TestFilter<T>(T value)
        {
            if (value == null)
                return true;
            IComparable valueComparable = value as IComparable;
            if (valueComparable == null)
                throw new InternalErrorException("Invalid value for " + GetType().Name + ": " + value.GetType());
            return (MinimumValue != null && valueComparable.CompareTo(MinimumValue) >= 0)
                && (MaximumValue != null && valueComparable.CompareTo(MaximumValue) <= 0);
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class SortOrderAttribute : Attribute
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
