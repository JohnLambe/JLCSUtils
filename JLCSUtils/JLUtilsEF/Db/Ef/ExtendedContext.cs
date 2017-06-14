using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db.Ef
{
    public class ExtendedContext : DbContext
    {
        public override int SaveChanges()
        {
            var args = new SaveChangesEventArgs(this);
            BeforeSaveChanges?.Invoke(this, args);

            if (args.Intercept)
                return 0;

            var result = base.SaveChanges();

            AfterSaveChanges?.Invoke(this, args);

            return result;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var args = new ModelCreatingEventArgs(this, modelBuilder);
            BeforeModelCreating?.Invoke(this, args);

            if (args.Intercept)
                return;

            base.OnModelCreating(modelBuilder);

            AfterModelCreating?.Invoke(this, args);
        }

        public event EventHandler<SaveChangesEventArgs> BeforeSaveChanges;
        public event EventHandler<SaveChangesEventArgs> AfterSaveChanges;

        public event EventHandler<ModelCreatingEventArgs> BeforeModelCreating;
        public event EventHandler<ModelCreatingEventArgs> AfterModelCreating;

        //TODO: Events for other virtual properties.

    }

}
