using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;

namespace JohnLambe.Util.Db.Ef
{
    /// <summary>
    /// Entity Framework -related utilities.
    /// </summary>
    public static class EfUtil
    {
        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns>true if the given object is either an Entity Framework proxy object or the Type of one.</returns>
        public static bool IsEfClass(object value)
        {
            Type clas = value is Type ? (Type)value : value.GetType();
            return clas?.Namespace?.StartsWith(EfProxyNamespace) ?? false;
        }

        /// <summary>
        /// The namespace of Entity Framework proxies.
        /// (This may change with future versions of Entity Framework.)
        /// </summary>
        private const string EfProxyNamespace = "System.Data.Entity.DynamicProxies";

        /// <summary>
        /// Change the state of entities in the EntityState.Modified to Unmodified if none of their fields are modified.
        /// </summary>
        /// <param name="ChangeTracker"></param>
        public static void UndoUnmodified(DbChangeTracker ChangeTracker)
        {
            foreach (var entry in ChangeTracker.Entries().Where(x => x.State == EntityState.Modified))
            {
                if (!IsModified(entry))
                    entry.State = EntityState.Unchanged;
            }

            // Collection properties ?
        }

        /// <summary>
        /// Tests whether the entity in the given entry is modified, i.e. its current values are different to the original values.
        /// (This does NOT compare to the values actually currently in persistent storage - it just checks whether it is modified in this context.)
        /// </summary>
        /// <param name="entry"></param>
        /// <returns>true iff the entity is modified.</returns>
        /// <seealso cref="IsEntityModified(DbContext, object)"/>
        public static bool IsModified([NotNull] DbEntityEntry entry)
        {
            return ComparePropertyValues(entry.OriginalValues, entry.CurrentValues);
        }

        /// <summary>
        /// Compare two values of a type compatible with the same property in a <see cref="DbPropertyValues"/>.
        /// If both values are <see cref="DbPropertyValues"/>, their contents are compared.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>true if the parameters are equal.</returns>
        private static bool Compare([Nullable] object a, [Nullable] object b)
        {
            if (a is DbPropertyValues && b is DbPropertyValues)
            {
                return ComparePropertyValues((DbPropertyValues)a, (DbPropertyValues)b);
            }
            else
            {
                return JohnLambe.Util.ObjectUtil.CompareEqual(a, b);
            }
        }

        /// <summary>
        /// Compare the contents of two <see cref="DbPropertyValues"/> instances.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">A DbPropertyValues with the same properties as <paramref name="a"/>.</param>
        /// <returns>true if the parameters are equal.</returns>
        private static bool ComparePropertyValues([NotNull] DbPropertyValues a, [NotNull] DbPropertyValues b)
        {
            foreach (var propertyName in a.PropertyNames)
            {
                var oldValue = a.GetValue<object>(propertyName);
                var newValue = b.GetValue<object>(propertyName);

                if (!Compare(oldValue, newValue))
                {   // difference found
                    return false;    // different
                }
            }
            return true;   // equal
        }

        /// <summary>
        /// Tests whether the given entity is modified, i.e. its current values are different to the original values.
        /// (This does NOT compare to the values actually currently in persistent storage - it just checks whether it is modified in this context.)
        /// </summary>
        /// <param name="context">The DbContext of the given entity.</param>
        /// <param name="entity">The entity to test.</param>
        /// <returns>true iff the entity is modified.</returns>
        public static bool IsEntityModified(DbContext context, object entity)
        {
            return IsModified(context.Entry(entity));
        }
    }
}