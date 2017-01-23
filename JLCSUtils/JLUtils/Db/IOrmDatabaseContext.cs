using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db
{
    public interface IOrmDatabaseContext
    {
        TEntity Add<TEntity>(TEntity entity);
        TEntity Remove<TEntity>(TEntity entity);
        TEntity Attach<TEntity>(TEntity entity);

        TEntity Find<TEntity>(params object[] keyValues);

        // For when the runtime type of the entity varies:
        TEntity Add<TEntity>(Type EntityType, TEntity entity);
        TEntity Remove<TEntity>(Type EntityType, TEntity entity);
        TEntity Attach<TEntity>(Type EntityType, TEntity entity);

        TEntity Find<TEntity>(Type EntityType, params object[] keyValues);

        int SaveChanges();
        //TODO: Async?
    }

    /// <summary>
    /// Base class for implementations of <see cref="IOrmDatabaseContext"/>.
    /// </summary>
    public abstract class OrmDatabaseContextBase : IOrmDatabaseContext
    {
        public virtual TEntity Add<TEntity>(TEntity entity)
        {
            return Add<TEntity>(typeof(TEntity), entity);
            //return EfDbContext.Set<TEntity>().Add(entity);
        }

        public abstract TEntity Add<TEntity>(Type EntityType, TEntity entity);

        public virtual TEntity Attach<TEntity>(TEntity entity)
        {
            return Attach(typeof(TEntity), entity);
            //return EfDbContext.Set<TEntity>().Attach(entity);
        }

        public abstract TEntity Attach<TEntity>(Type EntityType, TEntity entity);

        public virtual TEntity Find<TEntity>(Type EntityType, params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public virtual TEntity Find<TEntity>(params object[] keyValues)
        {
            return Find<TEntity>(typeof(TEntity), keyValues);
        }

        public abstract TEntity Remove<TEntity>(TEntity entity);

        public abstract TEntity Remove<TEntity>(Type EntityType, TEntity entity);

        public abstract int SaveChanges();
    }


    /* Entity Framework implementation:
      
    public class EfDatabaseContext : OrmDatabaseContextBase
    {
        public EfDatabaseContext(DbContext efContext)
        {
            EfDbContext = efContext;
        }

        protected DbContext EfDbContext { get; set; }


        public override TEntity Add<TEntity>(Type EntityType, TEntity entity)
        {
            return EfDbContext.Set(EntityType).Add(entity);
        }

        public override TEntity Attach<TEntity>(Type EntityType, TEntity entity)
        {
            return EfDbContext.Set(EntityType).Attach(entity);
        }

        public override TEntity Find<TEntity>(Type EntityType, params object[] keyValues)
        {
            return EfDbContext.Set(EntityType).Find(keyValues);
        }

        public override TEntity Remove<TEntity>(TEntity entity)
        {
            return Attach(typeof(TEntity), entity);
            //return EfDbContext.Set<TEntity>().Remove(entity);
        }

        public override TEntity Remove<TEntity>(Type EntityType, TEntity entity)
        {
            return EfDbContext.Set(EntityType).Remove(entity);
        }

        public override int SaveChanges()
        {
            return EfDbContext.SaveChanges();
        }
    }


    public class MemoryDatabaseContext : OrmDatabaseContextBase
    {
    }

        */

}
