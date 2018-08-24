using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db.Ef
{
    public class EfEventArgs
    {
        public EfEventArgs(DbContext context)
        {
            this.Context = context;
        }

        public DbContext Context { get; }
    }

    public class InterceptableEfEventArgs
    {
        public InterceptableEfEventArgs(DbContext context)
        {
            this.Context = context;
        }

        public DbContext Context { get; }

        public virtual bool Intercept { get; set; }
    }

    public class SaveChangesEventArgs : InterceptableEfEventArgs
    {
        public SaveChangesEventArgs(DbContext context) : base(context)
        {
        }
    }

    public class ModelCreatingEventArgs : InterceptableEfEventArgs
    {
        public ModelCreatingEventArgs(DbContext context, DbModelBuilder modelBuilder) : base(context)
        {
        }

        public DbModelBuilder ModelBuilder { get; set; }
    }

    public class ErrorEventArgs : InterceptableEfEventArgs
    {
        public ErrorEventArgs(DbContext context, Exception ex) : base(context)
        {
        }

        public virtual Exception Exception { get; }
    }

}
