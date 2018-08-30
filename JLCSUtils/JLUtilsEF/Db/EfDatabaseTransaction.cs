using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db
{
    public class EfDatabaseTransaction : IDatabaseTransaction
    {
        public EfDatabaseTransaction(DbContextTransaction transaction)
        {
            _transaction = transaction;
            _inProgress = true;
        }

        public virtual bool Commit()
        {
            _transaction.Commit();
            _inProgress = false;
            return true;
        }

        public virtual bool Rollback()
        {
            _transaction.Rollback();
            _inProgress = false;
            return true;
        }

        protected readonly DbContextTransaction _transaction;
        protected bool _inProgress;

        //| Provide access to the underlying DbContextTransaction ?
    }

}
