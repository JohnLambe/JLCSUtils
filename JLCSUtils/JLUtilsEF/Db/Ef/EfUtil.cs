using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Exceptions;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Text;
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
            return !ComparePropertyValues(entry.OriginalValues, entry.CurrentValues);
        }

        public static TEntity Reload<TEntity>(DbContext context, TEntity entity, OrmLoadFlags flags) where TEntity : class
        {
            if (entity != null)
            {
                if (!flags.HasFlag(OrmLoadFlags.IfAttached) || context.Entry(entity).State != EntityState.Deleted)
                {
                    var entry = context.Entry(entity);
                    entry.Reload();

                    if (flags.HasFlag(OrmLoadFlags.References))
                    {
                        foreach (var property in entity.GetType().GetProperties().Where(p => p.GetCustomAttribute<ForeignKeyAttribute>() != null))
                        {
                            entry.Reference(property.Name).Load();
                        }
                    }
                }
            }
            return entity;
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

        /// <summary>
        /// Corrects the state of 'Added' entities in the context that are not actually new entities.
        /// Used because adding an entity which references a detached entity causes the detached entity's state to be set to 'added', but it could be a detached copy of an existing entity in the database.
        /// A delegate is provided to test whether an item should already be in the database. If keys are assigned on saving (e.g. autoincrement keys), this could test for the key being assigned.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="isNewDelegate">Delegate to return true if the given object is new (not yet added).</param>
        /// <param name="isDuplicateDelegate">Delegate to return true if the given objects are for the same entity.</param>
        public static void AdjustStates(DbContext context, Func<object,bool> isNewDelegate, Func<object,object,bool> isDuplicateDelegate = null)
        {
            var entries = context.ChangeTracker.Entries().ToList();
            foreach (var e in entries)
            {
                if (e.State == System.Data.Entity.EntityState.Added && !isNewDelegate(e.Entity))
                {
                    if (isDuplicateDelegate != null)
                    {
                        foreach (var entry in context.ChangeTracker.Entries().Where(x => x.Entity != e.Entity && isDuplicateDelegate(x.Entity, e.Entity)))
                        {
//                            entry.State = EntityState.Detached;
                            //TODO: Find all references to x.Entity in the context and change to e.Entity.
                        }
                    }
                    e.State = System.Data.Entity.EntityState.Unchanged;
                }
            }
        }

        /// <summary>
        /// Make a clone of the given entity that is attached to the given context.
        /// The entity must not already be in the given context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="destinationContext"></param>
        /// <param name="entity"></param>
        /// <param name="newState">The state that the returned object should be in.</param>
        /// <returns></returns>
        [return: Nullable]
        public static T CopyToContext<T>(DbContext destinationContext, T entity, EntityState newState = EntityState.Unchanged)
            where T: class
        {
            if (entity == null)
            {
                return null;
            }
            else
            {
                var dest = EntityCloner.Clone(entity, true);
                destinationContext.Entry(dest).State = newState;
                return dest;
            }
        }

        /// <summary>
        /// Clone the given entity and attach it to the given context, unless the same entity already exists in that context.
        /// This may return the same instance passed, if it is already usable in the required context.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="destinationContext"></param>
        /// <param name="entity"></param>
        /// <param name="getKeyDelegate"></param>
        /// <param name="newState">The state that the returned object should be in if it is copied to the context. This does not apply if the returned object was already in the context.</param>
        /// <returns></returns>
        [return: Nullable]
        public static T CopyOrFindInContext<T>(DbContext destinationContext, T entity, GetKeyDelegate getKeyDelegate = null, EntityState newState = EntityState.Unchanged)
            where T : class
        {
            getKeyDelegate = getKeyDelegate ?? GetEntityKey;
            if (entity == null)
            {
                return null;
            }
            else
            {
                return FindInContext<T>(destinationContext, getKeyDelegate(entity), getKeyDelegate)
                    ?? CopyToContext(destinationContext, entity, newState);
            }
        }

        /// <summary>
        /// Find an entity in the context by its type and key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="getKeyDelegate"></param>
        /// <returns>The matching entity, or null if it is not found in the context.</returns>
        [return: Nullable]
        public static T FindInContext<T>(DbContext context, object key, GetKeyDelegate getKeyDelegate = null)
            where T: class
        {
            getKeyDelegate = getKeyDelegate ?? GetEntityKey;
            if (key == null)
                return null;
            else
                return context.ChangeTracker.Entries<T>().Where( e => getKeyDelegate(e) == key).FirstOrDefault()?.Entity;
        }

        /// <summary>
        /// Load the given entity from the given context (or return it from the cache of the given context).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <param name="getKeyDelegate"></param>
        /// <returns>An instance of the given entity in the required context.</returns>
        public static T LoadFromContext<T>(DbContext context, T entity, GetKeyDelegate getKeyDelegate = null)
        {
            getKeyDelegate = getKeyDelegate ?? GetEntityKey;
            var keyValues = getKeyDelegate(entity);
            object result;
            if(keyValues is object[])
                result = context.Set(typeof(T)).Find((object[])keyValues);
            else
                result = context.Set(typeof(T)).Find(keyValues);   // one key
            return (T)result;
        }

        /// <summary>
        /// Returns a key of a given entity.
        /// </summary>
        /// <param name="entity">The entity whose key is to be returned.
        /// </param>
        /// <returns>If this is an object[], it is the list of key values, as could be passed to <see cref="DbSet.Find(object[])"/>.
        /// Otherwise, it is a single value that is a unique key of the object, which could be passed to <see cref="DbSet.Find(object[])"/> as a single element in an array.
        /// </returns>
        public delegate object GetKeyDelegate(object entity);

        /// <summary>
        /// Default implementation of <see cref="GetKeyDelegate"/>.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static object GetEntityKey(object entity)
        {
            if (entity == null)
                return null;

            var properties = entity.GetType().GetProperties();
            object[] keyValues = new object[properties.Length];
            int index = 0;
            foreach (var property in properties)
            {
                keyValues[index] = property.GetValue(entity);
                index++;
            }

            if (keyValues.Length == 1)
                return keyValues[0];
            else
                return keyValues;
        }

        /// <summary>
        /// Return a human-readable string representation of the list of errors in the given exception,
        /// for use by IT people.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string FormatErrors(this DbEntityValidationException ex)
        {
            var builder = new StringBuilder();
            foreach(var result in ex.EntityValidationErrors)
            {
                builder.AppendLine("Entity Type: " + result.Entry.Entity.GetType().FullName);
                
                foreach(var error in result.ValidationErrors)
                {
                    builder.AppendLine("  " + error.PropertyName + ": " + error.ErrorMessage);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Like <see cref="FormatErrors(DbEntityValidationException)"/>, but formats errors for reading 
        /// by end users.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string FormatErrorsForUser(this DbEntityValidationException ex)
        {
            var builder = new StringBuilder();
            foreach (var result in ex.EntityValidationErrors)
            {
                builder.AppendLine(CaptionUtil.GetDisplayName(result.Entry.Entity.GetType()));
                //TODO: include description of instance

                foreach (var error in result.ValidationErrors)
                {
                    builder.AppendLine("  " + CaptionUtil.PascalCaseToCaption(error.PropertyName) + ": " + error.ErrorMessage);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns an exception with a human-readable description of the validation errors,
        /// with the given exception as <see cref="Exception.InnerException"/>.
        /// </summary>
        public static Exception FormatErrorsException(DbEntityValidationException ex)
        {
            return new UserErrorException("There is something invalid in the information being saved. The following errors are reported:\r\n" + FormatErrorsForUser(ex), ex);
        }

        public static Exception ToUserException(Exception ex)
        {
            return ex;
            //TODO: Extract SqlException from Exception, and convert to user-friendly error based on error number.
            // Look up field names in error message against entity class and convert to display names.
            // Look up index names by the IndexAttribute and report the display names of the fields.
        }


        //| Could make these extension methods.
        //| Some methods could take ChangeTracker instead of DbContext.
    }
}