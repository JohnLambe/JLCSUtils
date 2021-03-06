﻿using JohnLambe.Util.Collections;
using JohnLambe.Util.Db.Ef;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db
{
    /// <summary>
    /// Base class for ORM entities.
    /// <para>
    /// Automatically initializes certain inverse collection properties to empty collections. See <see cref="CollectionInitializer"/>.
    /// </para>
    /// </summary>
    public abstract class EntityBase
    {
        public EntityBase()
        {
            if (!EfUtil.IsEfClass(this))
                CollectionInitializer.InitializeInstance(this);
        }
    }

    /// <summary>
    /// Base class for entities with a key of type <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class EntityBase<TKey>
    {
        /// <summary>
        /// Create an instance with no inital key value.
        /// Use this if the key is generated by the database.
        /// </summary>
        protected EntityBase()
        {
        }

        /// <summary>
        /// Create an instance with a given key value.
        /// </summary>
        /// <param name="id">Value of <see cref="Id"/>.</param>
        protected EntityBase(TKey id)
        {
            Id = id;
        }

        [Key]
        public virtual TKey Id { get; protected set; }
    }

    public abstract class MarkDeleteEntityBase<TKey> : EntityBase<TKey>, IMarkDeleteEntity
    {
        /// <inheritdoc cref="IMarkDeleteEntity.IsActive"/>
        public virtual bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Entity which has a flag indicating whether it is considered deleted.
    /// </summary>
    public interface IMarkDeleteEntity
    {
        /// <summary>
        /// false iff this entity is considered deleted.
        /// </summary>
        bool IsActive { get; set; }  // Could name IsDeleted, Active ?
    }
}
