using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db
{
    public class EfDbRepositoryBase<TEntity> : IDatabaseRepositoryBase<TEntity>
        where TEntity : class
    {
        public EfDbRepositoryBase(DbContext context) : this(context, (IDbSet<TEntity>)context.Set(typeof(TEntity)))
        {
        }

        public EfDbRepositoryBase(DbContext context, IDbSet<TEntity> set)
        {
            Context = context;
            Data = set;
        }

        public virtual IQueryable<TEntity> AsQueryable()
        {
            return Data.AsQueryable<TEntity>();
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

        protected readonly DbContext Context;
        protected readonly IDbSet<TEntity> Data;
    }


    public class EfReadOnlyDbRepository<TEntity> : EfDbRepositoryBase<TEntity>, IReadOnlyDatabaseRepository<TEntity>
        where TEntity : class
    {
        public EfReadOnlyDbRepository(DbContext context) : base(context)
        {
        }

        public EfReadOnlyDbRepository(DbContext context, IDbSet<TEntity> set) : base(context,set)
        {
        }
    }


    public class EfMutableDbRepository<TEntity> : EfDbRepositoryBase<TEntity>, IMutableDatabaseRepository<TEntity>
        where TEntity : class
    {
        public EfMutableDbRepository(DbContext context) : base(context)
        {
        }

        public EfMutableDbRepository(DbContext context, IDbSet<TEntity> set) : base(context,set)
        {
        }

        public virtual TEntity Add(TEntity entity)
        {
            return Data.Add(entity);
        }

        public virtual TEntity Remove(TEntity entity)
        {
            return Data.Remove(entity);
        }

        public virtual int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public virtual TEntity Create()
        {
            return Data.Create();
        }

        public virtual TDerivedEntity Create<TDerivedEntity>()
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