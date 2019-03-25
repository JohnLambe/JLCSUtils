using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db.Ef
{
    /// <summary>
    /// Fires optional event(s) on entities.
    /// </summary>
    public class EntityEvents
    {
        public static void RegisterWith(ExtendedContext context)
        {            
            context.BeforeSaveChanges += new EntityEvents().Context_BeforeSaveChanges;
        }

        public virtual void Context_BeforeSaveChanges(object sender, SaveChangesEventArgs evt)
        {
            evt.Context.ChangeTracker.DetectChanges();
            foreach(var entity in 
                evt.Context.ChangeTracker.Entries()
                 .Where(x => (x.State == EntityState.Modified)
                        || (x.State == EntityState.Added))
                 .Select(x => x.Entity))
            {
                (entity as IEntityBeforeSaveChanges)?.BeforeSaveChanges();
            }
        }
    }

    public interface IEntityBeforeSaveChanges
    {
        /// <summary>
        /// Called immediately before changes are saved to the database.
        /// </summary>
        void BeforeSaveChanges();
    }


    /// <summary>
    /// An entity with handler(s) for certain events.
    /// </summary>
    public interface IEntityWithEvents
    {
        /// <summary>
        /// Called after certain actions that change an entity's state.
        /// </summary>
        void AfterEntityStateChange(EntityStateChangeArgs args);
    }

    public class EntityStateChangeArgs : EventArgs
    {
        public EntityStateChangeArgs(OrmEntityState newState)
        {
            this.NewState = newState;
        }

        public OrmEntityState NewState { get; }
    }

}
