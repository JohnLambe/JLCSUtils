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
    public interface IDatabaseRepositoryBase<TEntity>
    {
        /// <summary>
        /// Get the whole contents of the repository.
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> AsQueryable(bool includeDeleted = false);

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

        /// <summary>
        /// Get the original value of 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        object GetOriginalValue(TEntity entity, string propertyName);


        //| We could provide this, for iterating through original values:
        // IPropertyCollection GetOriginalValues(TEntity entity);
    }

    /*
    public interface IPropertyCollection : IReadOnlyDictionary<string,object>
    {
    }
    */


    public interface IDatabaseExt<TEntity> : IDatabaseRepositoryBase<TEntity>
    {
        /// <summary>
        /// Return the unmodified version of the given entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity GetOriginal(TEntity entity);
    }


    /// <summary>
    /// A read-only repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of object held in the repository.</typeparam>
    public interface IReadOnlyDatabaseRepository<TEntity> : IDatabaseRepositoryBase<TEntity>
    {
    }


    /// <summary>
    /// A repository that can be modified.
    /// </summary>
    /// <typeparam name="TEntity">The type of object held in the repository.</typeparam>
    public interface IMutableDatabaseRepository<TEntity> : IDatabaseRepositoryBase<TEntity>
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
        /// Delete an entity from the repository
        /// (or mark it deleted (if <see cref="IMarkDeleteEntity"/>)).
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        /// <returns>the entity</returns>
        TEntity Remove(TEntity entity);

        /// <summary>
        /// Save all changes.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Create an instance of the type for the repository. It is not attached to the ORM context.
        /// </summary>
        /// <param name="context">Context information for creating an entity. (The interpretation depends on the repository implementation.)</param>
        /// <returns>the new entity.</returns>
        TEntity Create(object context = null);

        /// <summary>
        /// Create an instance of a specified type that can be stored in the repository (the repository's type or a subclass of it).
        /// </summary>
        /// <typeparam name="TDerivedEntity">The type of entity to be created.</typeparam>
        /// <param name="context">Context information for creating an entity.</param>
        /// <returns>the new entity.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the requested type cannot be created or stored in this repository.
        /// A repository may not be able to store all subclasses of its base type.
        /// </exception>
        TDerivedEntity Create<TDerivedEntity>(object context = null) where TDerivedEntity : class, TEntity;


        /*  These could be added, replacing the EntityState type with one not specific to Entity Framework.
         *  Currently, separate methods are used for each state transition instead.
           
                void SetState<TEntity>(TEntity entity, EntityState state) where TEntity : class;

                EntityState GetState<TEntity>(TEntity entity) where TEntity : class;
        */
    }


    [Obsolete("Interface not finalized")]
    public interface IDatabase
    {
        IDatabaseRepositoryBase<T> GetRepositoryFor<T>(object entity);

        IDatabaseRepositoryBase<T> GetRepositoryForType<T>(Type entityType);

        bool SaveChanges(object entity);
    }


    /// <summary>
    /// Interface for an entity that has a flag indicating whether it is deleted.
    /// </summary>
    public interface IHasActiveFlag
    {
        /// <summary>
        /// true iff this is not deleted.
        /// </summary>
        //| It could, alternatively, be called "IsDeleted", with the opposite meaning.
        bool IsActive { get; set; }
    }

}
