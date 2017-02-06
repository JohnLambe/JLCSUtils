using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db
{
    // Interfaces for repositories.
    // The method signatures mostly match those on IDbSet.

    /// <summary>
    /// Base interface for a repository, which may or may not be modifiable.
    /// </summary>
    /// <typeparam name="TEntity">The type of object held in the repository.</typeparam>
    public interface IDbRepositoryBase<TEntity>
    {
        /// <summary>
        /// Get the whole contents of the repository.
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> AsQueryable();

        /// <summary>
        /// Find a single instance by its key.
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        TEntity Find(params object[] keyValues);

        /// <summary>
        /// Detach an entity from the context.
        /// Does nothing if there is no actual ORM context.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Detach(TEntity entity);
    }


    /// <summary>
    /// A read-only repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of object held in the repository.</typeparam>
    public interface IReadOnlyDbRepository<TEntity> : IDbRepositoryBase<TEntity>
    {
    }


    /// <summary>
    /// A repository that can be modified.
    /// </summary>
    /// <typeparam name="TEntity">The type of object held in the repository.</typeparam>
    public interface IMutableDbRepository<TEntity> : IDbRepositoryBase<TEntity>
    {
        /// <summary>
        /// Insert an entity into the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Add(TEntity entity);

        /// <summary>
        /// Add an entity to the ORM context.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="modified">true iff the entity is already modified (so its state is set to modified on attaching).</param>
        /// <returns></returns>
        TEntity Attach(TEntity entity, bool modified = false);

        /// <summary>
        /// Delete an entity from the repository.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Remove(TEntity entity);

        /// <summary>
        /// Save all changes.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Create an instance of the type for the repository. It is not attached to the ORM context.
        /// </summary>
        /// <returns></returns>
        TEntity Create();
        TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity;

        /*  These could be added, preferably replacing the EntityState type with one not specific to Entity Framework.
         *  Currently, separate methods are used for each state transition instead.
           
                void SetState<TEntity>(TEntity entity, EntityState state) where TEntity : class;

                EntityState GetState<TEntity>(TEntity entity) where TEntity : class;
        */
    }

}
