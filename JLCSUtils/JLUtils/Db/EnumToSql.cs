using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db
{
    /// <summary>
    /// Generates SQL for tables corresponding to enumerated types
    /// - creating and populating the tables.
    /// </summary>
    public class EnumToSql
    {
        public virtual string ToSql(Type enumType)
        {
            string tableName = enumType.Name;

            //TODO: Override names with attribute.

            StringBuilder sql = new StringBuilder(2048);
            //TODO: Existence check.
            sql.AppendFormat("create table {0} ( id int not null primary key, name varchar(50) );\n",
                tableName);
            //TODO: Index on name (for reporting)?

            foreach (var item in Enum.GetValues(enumType))
            {
                sql.AppendFormat("insert into {0} (id, name) values ( {1}, {2} );\n",
                    tableName,
                    (int)Enum.ToObject(enumType, item),
                    item.ToString().DoubleQuote('\"') //Enum.GetName(enumType,item).DoubleQuote('\"')
                    );
            }

            return sql.ToString();
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SqlNameAttribute : Attribute
    {
        public SqlNameAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Name of the item in the database.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Human-readable display name of this item.
        /// </summary>
        public virtual string DisplayName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class SqlEnumNameAttribute : SqlNameAttribute
    {
        public SqlEnumNameAttribute(string name = null) : base(name)
        {
        }

        /// <summary>
        /// Name of the column that holds the ID of each enum member.
        /// </summary>
        public virtual string IdColumnName { get; set; }

        /// <summary>
        /// Name of the column that holds the display name of each enum member.
        /// </summary>
        public virtual string NameColumnName { get; set; }
    }
}