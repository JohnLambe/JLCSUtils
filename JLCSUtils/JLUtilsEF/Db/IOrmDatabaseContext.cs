using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db
{
    // This is based on Entity Framework, with most parameters and return values matching its methods.

    /// <summary>
    /// Abtraction of a minimal interface to an ORM framework context.
    /// </summary>
    public interface IOrmDatabaseContext
    {
        /// <summary>
        /// Add an object to the database.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">object to be added.</param>
        /// <returns>The entity added (same as passed in).</returns>
        TEntity Add<TEntity>(TEntity entity);

        /// <summary>
        /// Remove an entity from the database.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Remove<TEntity>(TEntity entity);

        /// <summary>
        /// Attach a given object to the database.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">The object to attach.</param>
        /// <returns>The object attached.</returns>
        TEntity Attach<TEntity>(TEntity entity);

        /// <summary>
        /// Get an enity from the database by its key.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        TEntity Find<TEntity>(params object[] keyValues);


        // For when the runtime type of the entity varies:
        TEntity Add<TEntity>(Type EntityType, TEntity entity);
        TEntity Remove<TEntity>(Type EntityType, TEntity entity);
        TEntity Attach<TEntity>(Type EntityType, TEntity entity);

        TEntity Find<TEntity>(Type EntityType, params object[] keyValues);


        void SetState<TEntity>(TEntity entity, EntityState state) where TEntity : class;

        EntityState GetState<TEntity>(TEntity entity) where TEntity : class;


        /// <summary>
        /// Save pending changes to the database.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        //TODO: Async?
    }

    //TODO: DbSet / Repository abstraction.


    /// <summary>
    /// Base class for implementations of <see cref="IOrmDatabaseContext"/>.
    /// </summary>
    public abstract class OrmDatabaseContextBase : IOrmDatabaseContext
    {
        public virtual TEntity Add<TEntity>(TEntity entity)
        {
            return Add<TEntity>(typeof(TEntity), entity);
        }

        public abstract TEntity Add<TEntity>(Type EntityType, TEntity entity);

        public virtual TEntity Attach<TEntity>(TEntity entity)
        {
            return Attach(typeof(TEntity), entity);
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

        public abstract void SetState<TEntity>(TEntity entity, EntityState state) where TEntity : class;

        public abstract EntityState GetState<TEntity>(TEntity entity) where TEntity : class;

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
