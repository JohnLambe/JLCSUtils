﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Db.Ef;

namespace JohnLambe.Util.Db
{
    //TODO: Take a type implementing IHasActiveFlag as a parameter (probably a generic type parameter) and build a Linq Expression, in AsQueryableNonDeleted,
    // using this type.

    [Obsolete("likely to change")]
    public class EfDatabaseRepositoryBase<TEntity> : IDatabaseRepositoryBase<TEntity>
        where TEntity : class
//        where TFlagDeletedEntity : class, IHasActiveFlag //, TEntity
    {
        public EfDatabaseRepositoryBase(DbContext context) : this(context, context.Set<TEntity>())
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
                return Data.AsQueryable();
            else
                return AsQueryableNonDeleted<FlagDeletedEntityBase>();
//                return Data.AsQueryable().Cast<FlagDeletedEntity>().Where(x => x.IsActive).Cast<TEntity>();
        }

        protected virtual IQueryable<TEntity> AsQueryableNonDeleted<T>()
            where T: class, IHasActiveFlag
        {
            return Data.AsQueryable().Cast<T>().Where(x => x.IsActive).Cast<TEntity>();
        }



        public virtual TEntity Find(params object[] keyValues)
        {
            return Data.Find(keyValues);
        }

        public virtual TEntity Detach(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Detached;
            return entity;
        }

        public virtual object GetOriginalValue(TEntity entity, string propertyName)
        {
            return Context.Entry(entity).OriginalValues[propertyName];
        }

        public virtual TEntity ToContext(TEntity source)
        {
            return EfUtil.CopyOrFindInContext<TEntity>(Context, source);
        }

        public virtual TEntity Reload(TEntity entity, OrmLoadFlags flags = OrmLoadFlags.Default)
        {
            return EfUtil.Reload(Context,entity,flags);
        }

        public virtual IDatabaseConnection Database
        {
            get { return new EfDatabaseConnection(Context); }
            //| TODO: Return the same instance for multiple calls, and make repositories of the same database return the same instance ?
        }


        /// <summary>
        /// true iff the entity implements <see cref="IHasActiveFlag"/>.
        /// </summary>
        protected readonly bool _hasDeletedFlag;

        protected readonly DbContext Context;
        protected readonly IDbSet<TEntity> Data;
    }


    public class EfDatabaseConnection : IDatabaseConnection
    {
        public EfDatabaseConnection(DbContext context)
        {
            _context = context;
        }

        public virtual IDatabaseTransaction StartTransaction(TransactionIsolationLevel isolationLevel)
        {
            return new EfDatabaseTransaction(_context.Database.BeginTransaction(isolationLevel.ToIsolationLevel()));
        }

        protected readonly DbContext _context;
    }


    public class EfReadOnlyDatabaseRepository<TEntity> : EfDatabaseRepositoryBase<TEntity>, IReadOnlyDatabaseRepository<TEntity>
        where TEntity : class
//        where TFlagDeletedEntity : class, IHasActiveFlag //, TEntity
    {
        public EfReadOnlyDatabaseRepository(DbContext context) : base(context)
        {
        }

        public EfReadOnlyDatabaseRepository(DbContext context, IDbSet<TEntity> set) : base(context, set)
        {
        }
    }


    public class EfMutableDatabaseRepository<TEntity> : EfDatabaseRepositoryBase<TEntity>, IMutableDatabaseRepository<TEntity>
        where TEntity : class
//        where TFlagDeletedEntity : class, IHasActiveFlag //TEntity
    {
        public EfMutableDatabaseRepository(DbContext context) : base(context)
        {
        }

        public EfMutableDatabaseRepository(DbContext context, IDbSet<TEntity> set) : base(context, set)
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
                if (Context.Entry(entity).State != EntityState.Added)   // If 'adding', detach.
                {
                    Context.Entry(entity).State = EntityState.Detached;
                    return entity;
                }
                else
                {
                    return Data.Remove(entity);
                }
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
            if (Context.Entry(entity).State != EntityState.Added)             // If 'adding', do nothing.
            {
                Data.Attach(entity);
                if (modified)
                    Context.Entry(entity).State = EntityState.Modified;
            }
            return entity;
        }
    }


    /*
    public class EfReadOnlyDatabaseRepository<TEntity> : EfDatabaseRepositoryBase<TEntity, FlagDeletedEntity>
        where TEntity : class
    {
        public EfReadOnlyDatabaseRepository(DbContext context) : base(context)
        {
        }

        public EfReadOnlyDatabaseRepository(DbContext context, IDbSet<TEntity> set) : base(context, set)
        {
        }
    }


    public class EfMutableDatabaseRepository<TEntity> : EfMutableDatabaseRepository<TEntity, FlagDeletedEntity>
        where TEntity : class
    {
        public EfMutableDatabaseRepository(DbContext context) : base(context)
        {
        }

        public EfMutableDatabaseRepository(DbContext context, IDbSet<TEntity> set) : base(context, set)
        {
        }
    }
    */
}