using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db
{
    public class EfDatabaseContext : OrmDatabaseContextBase
    {
        public EfDatabaseContext(DbContext efContext)
        {
            EfDbContext = efContext;
        }

        protected DbContext EfDbContext { get; set; }


        public override TEntity Add<TEntity>(Type EntityType, TEntity entity)
        {
            return (TEntity)EfDbContext.Set(EntityType).Add(entity);
        }

        public override TEntity Attach<TEntity>(Type EntityType, TEntity entity)
        {
            return (TEntity)EfDbContext.Set(EntityType).Attach(entity);
        }

        public override TEntity Find<TEntity>(Type EntityType, params object[] keyValues)
        {
            return (TEntity)EfDbContext.Set(EntityType).Find(keyValues);
        }

        public override TEntity Remove<TEntity>(TEntity entity)
        {
            return Attach(typeof(TEntity), entity); ////****BUG
        }

        public override TEntity Remove<TEntity>(Type EntityType, TEntity entity)
        {
            return (TEntity)EfDbContext.Set(EntityType).Remove(entity);
        }

        public override int SaveChanges()
        {
            return EfDbContext.SaveChanges();
        }

        public override void SetState<TEntity>(TEntity entity, EntityState state)
        {
            EfDbContext.Entry<TEntity>(entity).State = state;
        }

        public override EntityState GetState<TEntity>(TEntity entity)
        {
            return EfDbContext.Entry<TEntity>(entity).State;
        }
    }

}
