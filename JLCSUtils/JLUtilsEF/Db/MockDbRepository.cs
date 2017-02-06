using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Collections;
using JohnLambe.Util.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JohnLambe.Util.Db
{
    public class MockDbRepository<TEntity> : IMutableDbRepository<TEntity>, IReadOnlyDbRepository<TEntity>
        where TEntity : class
    {
        //TODO: tracking entity state (e.g. whether attached entities are modified).
        //  The behaviour of this does not fully match Entity Framework.

        public MockDbRepository(ICollection<TEntity> initialData)
        {
            if (initialData != null)
            {
                foreach(var item in initialData)
                {
                    AddInternal(item);
                }
            }
        }

        #region IDbRepositoryBase

        public virtual IQueryable<TEntity> AsQueryable()
        {
            return Data.Values.AsQueryable<TEntity>();
        }

        public virtual TEntity Find(params object[] keyValues)
        {
            TEntity result;
            if (!Data.TryGetValue(keyValues, out result))
            {
                throw new KeyNotFoundException("Key not found in mock repository for "
                      + typeof(TEntity).Name + "; Key: " + CollectionUtils.CollectionToString(keyValues));
            }
            return result;
        }

        public virtual TEntity Detach(TEntity entity)
        {
            return entity;
        }

        #endregion

        #region IMutableDbRepository

        public virtual TEntity Add(TEntity entity)
        {
            AddToLog(Action.Remove,entity);
            ChangesCount++;
            return AddInternal(entity);
        }

        protected virtual TEntity AddInternal(TEntity entity)
        {
            Data.Add(GetKeyValues(entity), entity);
            return entity;
        }

        public virtual TEntity Create()
        {
            return ReflectionUtils.Create<TEntity>(typeof(TEntity));
        }

        public virtual TDerivedEntity Create<TDerivedEntity>()
            where TDerivedEntity : class, TEntity
        {
            return ReflectionUtils.Create<TDerivedEntity>(typeof(TDerivedEntity));
        }

        // What does IDbSet.Remove do when the entity does not exist?
        public virtual TEntity Remove(TEntity entity)
        {
            AddToLog(Action.Remove, entity);
            ChangesCount++;
            return RemoveInternal(entity);
        }

        protected virtual TEntity RemoveInternal(TEntity entity)
        {
            Data.Remove(entity);
            return entity;
        }

        // THE RETURN VALUE MAY NOT BE CORRECT IN THIS VERSION.
        public virtual int SaveChanges()
        {
            AddToLog(Action.Save);
            int result = ChangesCount;
            ChangesCount = 0;
            return result;
        }

        public virtual TEntity Attach(TEntity entity, bool modified = false)
        {
            AddInternal(entity);
            return entity;
        }

        #endregion

        #region Mock features

        /// <summary>
        /// Returns the values of the Key properties of the given entity.
        /// Supports only attribute definition.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual object[] GetKeyValues(TEntity entity)
        {
            object[] keyValues = entity.GetType().GetProperties().Where(p => p.IsDefined<KeyAttribute>())
                .OrderBy(p => p.GetCustomAttribute<ColumnAttribute>()?.Order ?? 0)
                .ToArray();
            if (keyValues.Length == 0)
                throw new ArgumentException("The entity type (" + typeof(TEntity).Name + ") has no Key attribute. This is required by " + GetType().Name + ".");
            return keyValues;
        }

        /// <summary>
        /// The number of changes made and not committed.
        /// </summary>
        // THIS CURRENTLY MAY NOT MATCH ACTUAL ENTITY FRAMEWORK IMPLEMENTATION.
        public virtual int ChangesCount { get; protected set; } = 0;
        protected IDictionary<object, TEntity> Data = new Dictionary<object, TEntity>();

        #endregion

        #region Log

        public enum Action
        {
            Add,
            Remove,
            Save
        }

        public class LogEntry
        {
            public Action Action { get; set; }
            public string Entity { get; set; }
        }

        public virtual LinkedList<LogEntry> Log { get; protected set; } = new LinkedList<LogEntry>();

        protected virtual void AddToLog(Action action, TEntity entity = null)
        {
            Log.AddLast(new LogEntry() { Action = Action.Remove, Entity = entity.ToString() });
            OnChange?.Invoke(action, entity);
        }

        public delegate void RepositoryEvent(Action action, TEntity entity);
        //| We could add a parameter to indicate whether an item was found.

        /// <summary>
        /// Fired when an action that can change the data is done.
        /// </summary>
        public event RepositoryEvent OnChange;

        #endregion

    }
}
