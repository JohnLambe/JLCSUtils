using JohnLambe.Util.Reflection;
using JohnLambe.Util.Text;
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
            var attribute = enumType.GetCustomAttribute<SqlEnumNameAttribute>();
            string tableName = attribute?.Name ?? enumType.Name;     // use name in code if there is no attribute

            StringBuilder sql = new StringBuilder(2048);
            //TODO: Existence check.

            string idColumn = attribute?.IdColumnName ?? "Id";
            string nameColumn = attribute?.NameColumnName ?? "Name";

            sql.AppendFormat("create table {0} ( " + idColumn + " int not null primary key, " + nameColumn + " varchar(50) );\n",
                tableName);
            //TODO: Index on name (for reporting)?

            foreach (var item in Enum.GetValues(enumType))
            {
                var itemAttribute = EnumUtil.GetField((Enum)item).GetCustomAttribute<SqlNameAttribute>();
                sql.AppendFormat("insert into {0} (" + idColumn + ", " + nameColumn + ") values ( {1}, {2} );\n",
                    tableName,
                    (int)Enum.ToObject(enumType, item),    // the integer value
                    (itemAttribute?.Name ?? item.ToString()).DoubleQuote('\"') //Enum.GetName(enumType,item).DoubleQuote('\"')
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
        public virtual string IdColumnName
        {
            get { return _idColumnName; }
            set
            {
                Validate(value);
                _idColumnName = value;
            }
        }
        protected string _idColumnName;

        /// <summary>
        /// Name of the column that holds the display name of each enum member.
        /// </summary>
        public virtual string NameColumnName
        {
            get { return _nameColumnName; }
            set
            {
                Validate(value);
                _nameColumnName = value;
            }
        }
        protected string _nameColumnName;

        protected void Validate(string value)
        {
            if (!value.ContainsOnlyCharacters(CharacterUtil.IdentifierCharacters))
                throw new ArgumentException("Invalid character in name: " + value);
        }
    }
}