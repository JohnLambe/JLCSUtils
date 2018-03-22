using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db
{
    public class EfDatabaseRepositoryBase<TEntity> : IDatabaseRepositoryBase<TEntity>
        where TEntity : class
    {
        public EfDatabaseRepositoryBase(DbContext context) : this(context, (IDbSet<TEntity>)context.Set(typeof(TEntity)))
        {
            _hasDeletedFlag = Reflection.ReflectionUtil.Implements(typeof(TEntity), typeof(IHasActiveFlag));
        }

        public EfDatabaseRepositoryBase(DbContext context, IDbSet<TEntity> set)
        {
            Context = context;
            Data = set;
        }

        public virtual IQueryable<TEntity> AsQueryable(bool includeDeleted = false)
        {
            if (includeDeleted || !_hasDeletedFlag)
                return Data.AsQueryable<TEntity>();
            else
                return Data.AsQueryable<TEntity>().Where(x => ((IHasActiveFlag)x).IsActive);
        }

        public virtual TEntity Find(params object[] keyValues)
        {
            return Data.Find(keyValues);
        }

        public virtual TEntity Detach(TEntity entity)
        {
            Context.Entry<TEntity>(entity).State = EntityState.Detached;
            return entity;
        }

        public virtual object GetOriginalValue(TEntity entity, string propertyName)
        {
            return Context.Entry(entity).OriginalValues[propertyName];
        }

        /// <summary>
        /// true iff the entity implements <see cref="IHasActiveFlag"/>.
        /// </summary>
        protected readonly bool _hasDeletedFlag;

        protected readonly DbContext Context;
        protected readonly IDbSet<TEntity> Data;
    }


    public class EfReadOnlyDatabaseRepository<TEntity> : EfDatabaseRepositoryBase<TEntity>, IReadOnlyDatabaseRepository<TEntity>
        where TEntity : class
    {
        public EfReadOnlyDatabaseRepository(DbContext context) : base(context)
        {
        }

        public EfReadOnlyDatabaseRepository(DbContext context, IDbSet<TEntity> set) : base(context,set)
        {
        }
    }


    public class EfMutableDatabaseRepository<TEntity> : EfDatabaseRepositoryBase<TEntity>, IMutableDatabaseRepository<TEntity>
        where TEntity : class
    {
        public EfMutableDatabaseRepository(DbContext context) : base(context)
        {
        }

        public EfMutableDatabaseRepository(DbContext context, IDbSet<TEntity> set) : base(context,set)
        {
        }

        public virtual TEntity Add(TEntity entity)
        {
            return Data.Add(entity);
        }

        public virtual TEntity Remove(TEntity entity)
        {
            if (entity is IMarkDeleteEntity)
            {
                ((IMarkDeleteEntity)entity).IsActive = false;
                return entity;
            }
            else
            {
                return Data.Remove(entity);
            }
        }

        public virtual int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public virtual TEntity Create(object context = null)
        {
            return Data.Create();
        }

        public virtual TDerivedEntity Create<TDerivedEntity>(object context = null)
            where TDerivedEntity : class, TEntity
        {
            return Data.Create<TDerivedEntity>();
        }

        public virtual TEntity Attach(TEntity entity, bool modified = false)
        {
            Data.Attach(entity);
            if(modified)
                Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }
    }

}