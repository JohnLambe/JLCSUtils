using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;

namespace JohnLambe.Util.Db.Ef
{
    //TODO:
    public class ReferenceOnlyTracker
    {
        public ReferenceOnlyTracker()
        {
        }

        public bool Add(object entity)
        {
            return _referenceOnlyEntities.Add(entity);
        }
        public bool Remove(object entity)
        {
            return _referenceOnlyEntities.Remove(entity);
        }

        public virtual bool IsReferenceOnly(object entity)
        {
            return _referenceOnlyEntities.Contains(entity);
        }

        protected HashSet<object> _referenceOnlyEntities;


        public virtual void BeforeSave(DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries())
            {
                if((entry.State != EntityState.Unchanged) && IsReferenceOnly(entry.Entity))
                {
                    entry.State = EntityState.Unchanged;
                }
            }
        }

        public virtual void AfterSave(DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries())
            {
                if ((entry.State != EntityState.Detached) && IsReferenceOnly(entry.Entity))
                {
                    entry.State = EntityState.Detached;
                }
            }
        }


        /*
        protected virtual void RemoveReferencedOnlyEntities(DbContext context)
        {
            foreach(var entry in context.ChangeTracker.Entries())
            {
                // for each property
                    // if reference property and value is in _referenceOnlyEntities
                        // new RemovedProperty()
                        // get key
                        // assign property to null (setter!)
                        // assign FK (even if protected)

                    // if collection
                        // for each item
                            // if in _referenceOnlyEntities
                                // make Unchanged
            }
        }

        protected virtual void RestoreAll()
        {

            foreach(var removedEntityEntry in _removedProperties)
            {
                foreach(var prop in removedEntityEntry.Value)  // for each RemovedProperty
                {
                    prop.Restore();
                }
                // detach
            }

        }

        protected IDictionary<object, IEnumerable<RemovedProperty>> _removedProperties;

        public class RemovedProperty
        {
            public RemovedProperty([NotNull] object entity, [NotNull] string propertyName, [Nullable] object value)
            {
                Entity = entity;
                Property = entity.GetType().GetProperty(propertyName);
                Value = value;
            }

            public virtual object Entity { get; }

//            public virtual string PropertyName { get; set; }

            public virtual PropertyInfo Property { get; }

            public virtual object Value { get; }

            public virtual void Restore()
            {
                Property.SetValue(Entity, Value);
                // setter!
            }
        }
        */

    }
}
