using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Db;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;

namespace JohnLambe.Util.Db.Ef
{
    public class EfDatabase : IDatabase
    {
        public EfDatabase([NotNull] DbContext dbContext)
        {
            _dbContext = dbContext.ArgNotNull(nameof(dbContext));
        }

        public virtual IDatabaseRepositoryBase<T> GetRepositoryFor<T>(object entity)
            where T: class
        {
            return new EfMutableDatabaseRepository<T>(_dbContext);
        }

        public virtual IDatabaseRepositoryBase<T> GetRepositoryForType<T>(Type entityType)
            where T : class
        {
            return ReflectionUtil.Create<IDatabaseRepositoryBase<T>>( Type.GetType("EfMutableDatabaseRepository<>").MakeGenericType(entityType), _dbContext);
        }

        public virtual int SaveChanges(object entity)
        {
            return _dbContext.SaveChanges();
        }

        protected DbContext _dbContext;
    }
}
