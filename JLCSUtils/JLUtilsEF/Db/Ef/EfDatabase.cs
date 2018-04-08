using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Db;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;

namespace JohnLambe.Util.Db.Ef
{
    /// <summary>
    /// <see cref="IDatabase"/> implementation for Entity Framework.
    /// </summary>
    public class EfDatabase : IDatabase
    {
        public EfDatabase([NotNull] DbContext dbContext)
        {
            _dbContext = dbContext.ArgNotNull(nameof(dbContext));
        }

        public IDatabaseRepositoryBase<T> GetRepository<T>(bool writeable = false) where T : class
        {
            if(writeable)
                return new EfMutableDatabaseRepository<T>(_dbContext);
            else
                return new EfReadOnlyDatabaseRepository<T>(_dbContext);
        }

        public virtual IDatabaseRepositoryBase<T> GetRepositoryForType<T>(Type entityType, bool writeable = false)
            where T : class
        {
            if (writeable)
                return ReflectionUtil.Create<IDatabaseRepositoryBase<T>>(GenericTypeUtil.ChangeGenericParameters(typeof(EfMutableDatabaseRepository<>),entityType), _dbContext);
            else
                return ReflectionUtil.Create<IDatabaseRepositoryBase<T>>(GenericTypeUtil.ChangeGenericParameters(typeof(EfReadOnlyDatabaseRepository<>),entityType), _dbContext);
        }

        public virtual IDatabaseRepositoryBase<T> GetRepositoryFor<T>(object entity, bool writeable = false)
            where T: class
        {
            return GetRepositoryForType<T>(entity.GetType(), writeable);
        }

        public virtual int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        public virtual IEnumerable<Type> EntityTypes
        {
            get
            {
                return _dbContext.GetType().GetProperties().Where(p => p.PropertyType.GenericTypeArguments.Length == 1
                        && GenericTypeUtil.Compare(p.PropertyType, typeof(IDbSet<>)))
                        .Select(p => p.PropertyType.GenericTypeArguments[0]);
                // all types that have an IDbSet<T> property on the DbContext class
            }
        }

        protected DbContext _dbContext;
    }
}
