using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;

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
        /// <returns>the entity found.</returns>
        TEntity Find(params object[] keyValues);

        /// <summary>
        /// Detach an entity from the context.
        /// Does nothing if there is no actual ORM context.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>the given entity (<paramref name="entity"/>).</returns>
        TEntity Detach(TEntity entity);

        /// <summary>
        /// Get the original value of a property.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        object GetOriginalValue(TEntity entity, string propertyName);

        //| We could provide this, for iterating through original values:
        // IPropertyCollection GetOriginalValues(TEntity entity);

        /// <summary>
        /// Given an entity that may be in a different ORM context, return an instance of that entity that is usable in the ORM context of this repository.
        /// The returned instance may the same instance as the given one (if the instance is already in this context, or the ORM system allows the same instance to be used).
        /// </summary>
        /// <param name="source">The entity, possibly in a different context.</param>
        /// <returns>the entity in the context of this repository.</returns>
        TEntity ToContext(TEntity source);

        /// <summary>
        /// Reload the given entity from the database.
        /// Any pending changes to the object (including any pending deletion or insertion) are discarded.
        /// </summary>
        /// <param name="entity">
        /// An entity from this repository, or an unattached object.
        /// If null, null is returned.
        /// </param>
        /// <param name="ifAttached">Iff true, the entity is reloaded only if it is attached.</param>
        /// <returns>the given entity (<paramref name="entity"/>).</returns>
        TEntity Reload([Nullable] TEntity entity, OrmLoadFlags flags = OrmLoadFlags.Default);  // Rename to "Revert" ?

        /// <summary>
        /// Get the state of the entity in the current ORM context.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The state of <paramref name="entity"/>.</returns>
        System.Data.Entity.EntityState GetState(TEntity entity);

        /// <summary>
        /// Set the state of the entity in the ORM context.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="state"></param>
        void SetState(TEntity entity, System.Data.Entity.EntityState state);

        /// <summary>
        /// If an instance of the same entity as the given object (i.e. having the same type and key) exists in the context,
        /// the properties of this one are copied to it.
        /// If it does not exist, this is attached to the context.
        /// If this is already attached to the context, this does nothing.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="options"></param>
        /// <returns>The entity in the context.</returns>
        TEntity UpdateEntity(TEntity entity, OrmLoadFlags options = OrmLoadFlags.Default);

        //TODO: Populate navigation properties.


        //TODO: AsNoTracking ?
    }


    /*
    public interface IPropertyCollection : IReadOnlyDictionary<string,object>
    {
    }
    */


    public interface IDatabaseRepositoryExt<TEntity> : IDatabaseRepositoryBase<TEntity>
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

        /// <summary>
        /// The database connection of this repository.
        /// </summary>
        IDatabaseConnection Database { get; }
    }


    /// <summary>
    /// Abstraction of an object or ORM database.
    /// This provides access to the repository for each type.
    /// </summary>
    /// <remarks>
    /// In general, injecting repositories for the required types is recommended, so this is rarely needed.
    /// It should be avoided when not needed, since it creates a dependency on the whole database rather than just the required types.
    /// This may be needed for utilities or frameworks that can operate on any type in the database without having knowledge of it at compile time
    /// (possibly by reflection). (e.g. 
    /// </remarks>
    public interface IDatabase
    {
        /// <summary>
        /// Same as <see cref="GetRepositoryFor{T}(Type, bool)"/> except that the type is the generic parameter.
        /// </summary>
        /// <param name="writeable">Iff true, the returned instance allows modifiying the data (it will implement <see cref="IMutableDatabaseRepository{TEntity}"/>.)
        /// </param>
        /// <typeparam name="T">The type of entity whose repository is requested.</typeparam>
        /// <returns>the repository</returns>
        IDatabaseRepositoryBase<T> GetRepository<T>(bool writeable = false) where T : class;

        /// <summary>
        /// Get the repository for the given type in this database.
        /// </summary>
        /// <typeparam name="T">The return value is cast to a repository for this type.
        /// This does not affect what repository is returned. <paramref name=""/>
        /// </typeparam>
        /// <param name="entityType"></param>
        /// <param name="writeable">Iff true, the returned instance allows modifiying the data (it will implement <see cref="IMutableDatabaseRepository{TEntity}"/>.)
        /// </param>
        /// <returns>the repository</returns>
        IDatabaseRepositoryBase<T> GetRepositoryForType<T>(Type entityType, bool writeable = false) where T : class;

        /// <summary>
        /// Get the repository for the given instance in this database.
        /// </summary>
        /// <typeparam name="T">The return value is cast to a repository for this type.
        /// This does not affect what repository is returned. <paramref name=""/>
        /// </typeparam>
        /// <param name="entity"></param>
        /// <param name="writeable">Iff true, the returned instance allows modifiying the data (it will implement <see cref="IMutableDatabaseRepository{TEntity}"/>.)
        /// </param>
        /// <returns>the repository</returns>
        IDatabaseRepositoryBase<T> GetRepositoryFor<T>(object entity, bool writeable = false) where T: class;

        /// <summary>
        /// Write all pending changes in this session/context to persistent storage.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// The base types (classes) of all entities in the database.
        /// These are the types at the highest level in the type hierarchy at which a repository is available. (Each of these typically corresponds to a table in an ORM database.)
        /// </summary>
        IEnumerable<Type> EntityTypes { get; }

        //| Could add a method to enumerate repositories.
    }


    /// <summary>
    /// Database-independent abstraction of a database connection.
    /// </summary>
    public interface IDatabaseConnection
    {
        IDatabaseTransaction StartTransaction(IsolationLevel isolationLevel);
        //TODO: Provide default value for isolationLevel
    }

    /*
    /// <summary>
    /// Database independent abstraction of an isolation level.
    /// </summary>
    public enum TransactionIsolationLevel
    {
        //
        // Summary:
        //     A different isolation level than the one specified is being used, but the level
        //     cannot be determined.
        Unspecified = -1,
        //
        // Summary:
        //     The pending changes from more highly isolated transactions cannot be overwritten.
        Chaos = 16,
        //
        // Summary:
        //     A dirty read is possible, meaning that no shared locks are issued and no exclusive
        //     locks are honored.
        ReadUncommitted = 256,
        //
        // Summary:
        //     Shared locks are held while the data is being read to avoid dirty reads, but
        //     the data can be changed before the end of the transaction, resulting in non-repeatable
        //     reads or phantom data.
        ReadCommitted = 4096,
        //
        // Summary:
        //     Locks are placed on all data that is used in a query, preventing other users
        //     from updating the data. Prevents non-repeatable reads but phantom rows are still
        //     possible.
        RepeatableRead = 65536,
        //
        // Summary:
        //     A range lock is placed on the System.Data.DataSet, preventing other users from
        //     updating or inserting rows into the dataset until the transaction is complete.
        Serializable = 1048576,
        //
        // Summary:
        //     Reduces blocking by storing a version of data that one application can read while
        //     another is modifying the same data. Indicates that from one transaction you cannot
        //     see changes made in other transactions, even if you requery.
        Snapshot = 16777216
    }
    */


    /// <summary>
    /// Database-independent abstraction of a transaction.
    /// </summary>
    public interface IDatabaseTransaction
    {
        bool Commit();
        bool Rollback();
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
