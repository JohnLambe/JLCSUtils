using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db.Ef
{
    public class ExtendedContext : DbContext
    {
        //
        // Summary:
        //     Constructs a new context instance using the given string as the name or connection
        //     string for the database to which a connection will be made. See the class remarks
        //     for how this is used to create a connection.
        //
        // Parameters:
        //   nameOrConnectionString:
        //     Either the database name or a connection string.
        public ExtendedContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
        //
        // Summary:
        //     Constructs a new context instance using the given string as the name or connection
        //     string for the database to which a connection will be made, and initializes it
        //     from the given model. See the class remarks for how this is used to create a
        //     connection.
        //
        // Parameters:
        //   nameOrConnectionString:
        //     Either the database name or a connection string.
        //
        //   model:
        //     The model that will back this context.
        public ExtendedContext(string nameOrConnectionString, DbCompiledModel model) : base(nameOrConnectionString, model) { }
        //
        // Summary:
        //     Constructs a new context instance using the existing connection to connect to
        //     a database. The connection will not be disposed when the context is disposed
        //     if contextOwnsConnection is false.
        //
        // Parameters:
        //   existingConnection:
        //     An existing connection to use for the new context.
        //
        //   contextOwnsConnection:
        //     If set to true the connection is disposed when the context is disposed, otherwise
        //     the caller must dispose the connection.
        public ExtendedContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { }
        //
        // Summary:
        //     Constructs a new context instance around an existing ObjectContext.
        //
        // Parameters:
        //   objectContext:
        //     An existing ObjectContext to wrap with the new context.
        //
        //   dbContextOwnsObjectContext:
        //     If set to true the ObjectContext is disposed when the DbContext is disposed,
        //     otherwise the caller must dispose the connection.
        public ExtendedContext(ObjectContext objectContext, bool dbContextOwnsObjectContext) : base(objectContext, dbContextOwnsObjectContext) { }
        //
        // Summary:
        //     Constructs a new context instance using the existing connection to connect to
        //     a database, and initializes it from the given model. The connection will not
        //     be disposed when the context is disposed if contextOwnsConnection is false.
        //
        // Parameters:
        //   existingConnection:
        //     An existing connection to use for the new context.
        //
        //   model:
        //     The model that will back this context.
        //
        //   contextOwnsConnection:
        //     If set to true the connection is disposed when the context is disposed, otherwise
        //     the caller must dispose the connection.
        public ExtendedContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection) { }
        //
        // Summary:
        //     Constructs a new context instance using conventions to create the name of the
        //     database to which a connection will be made. The by-convention name is the full
        //     name (namespace + class name) of the derived context class. See the class remarks
        //     for how this is used to create a connection.
        protected ExtendedContext() : base() { }
        //
        // Summary:
        //     Constructs a new context instance using conventions to create the name of the
        //     database to which a connection will be made, and initializes it from the given
        //     model. The by-convention name is the full name (namespace + class name) of the
        //     derived context class. See the class remarks for how this is used to create a
        //     connection.
        //
        // Parameters:
        //   model:
        //     The model that will back this context.
        protected ExtendedContext(DbCompiledModel model) : base(model) { }



        public override int SaveChanges()
        {
            var args = new SaveChangesEventArgs(this);
            BeforeSaveChanges?.Invoke(this, args);

            if (args.Intercept)
                return 0;

            int result = 0;
            try
            {
                result = base.SaveChanges();
            }
            catch(Exception ex)
            {
                /*
                if(ex is DbEntityValidationException)
                {
                    throw EfUtil.FormatErrorsException((DbEntityValidationException)ex);
                }
                */

                OnError?.Invoke(this, new ErrorEventArgs(this,ex));
            }

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
        public event EventHandler<ErrorEventArgs> OnError;

        public event EventHandler<ModelCreatingEventArgs> BeforeModelCreating;
        public event EventHandler<ModelCreatingEventArgs> AfterModelCreating;

        //TODO: Events for other virtual properties.

    }

}
