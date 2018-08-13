using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLUtilsEFTest.Db
{
    /// <summary>
    /// Base class for attributes that define how classes are mapped to a database.
    /// </summary>
    public abstract class OrmMappingAttribute : Attribute
    {
    }


    /// <summary>
    /// Defines an association with another entity, where the attribute property is one side of the association.
    /// Only one side should be attributed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class AssociationAttribute : OrmMappingAttribute
    {
        /// <summary>
        /// The referenced type.
        /// If null, the type of the attributed property is used.
        /// The effective value (this, or the type of the attributed property, if this is null) must be an entity type that is mapped to the database.
        /// </summary>
        public virtual Type ReferencedType { get; set; }

        /// <summary>
        /// The name of the referenced property on the referenced type.
        /// </summary>
        public virtual string Property { get; set; }

        /// <summary>
        /// The name of the association table, if required.
        /// null for the default (or if no mapping table is required).
        /// <para>
        /// The default name is the left entity name concatenated with the left property name.
        /// </para>
        /// </summary>
        public virtual string TableName { get; set; }

        /// <summary>
        /// The name of the 'left' column (in the database).
        /// null for the default: The name of the 'left' entity plus "Id".
        /// </summary>
        //| We could make the column name use the name of the column that the [Key] property is mapped to. (What about when it has multiple properties?)
        public virtual string LeftColumnName { get; set; }

        /// <summary>
        /// The name of the 'right' column (in the database).
        /// null for the default: The name of the 'right' entity plus "Id", except for self-references, when this is prepended with the 'left' property name.
        /// </summary>
        public virtual string RightColumnName { get; set; }

        /// <summary>
        /// true if this is the 'right' side of the association.
        /// </summary>
        public virtual bool IsRight { get; set; } = false;
    }


    /// <summary>
    /// Specifies that the attributed property is mapped to a database column.
    /// </summary>
    /// <remarks>
    /// This can be used to map a protected property with Entity Framework.
    /// <see cref="System.ComponentModel.DataAnnotations.Schema.ColumnAttribute"/> allows mapping a property with a protected setter, but requires a public getter.
    /// </remarks>
    public class ColumnMappingAttribute : OrmMappingAttribute
    {
        /// <summary>
        /// The column name.
        /// Null for default: The property name, possibly modified if it is invalid for the database.
        /// </summary>
        public virtual string Name { get; set; }

        // Order
        // TypeName
    }
}