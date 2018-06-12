using JLUtilsEFTest.Db.Ef;
using JLUtilsEFTest.Db.Ef.CollectionInitializerTst;
using JLUtilsEFTest.Db.Ef.Collections;
using JLUtilsEFTest.Db.Ef.CollectionsInitialized;
using JohnLambe.Util.Db.Ef;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Db;

namespace JLUtilsEFTest
{
    public class TestDbContext : ExtendedContext
    {
        public TestDbContext()
            : base(@"Data Source=localhost\SQLEXPRESS; Integrated Security=True; MultipleActiveResultSets=True;Initial Catalog=JLUtilsEFTest")
        //            : base(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString())
        {
            //            Database.SetInitializer<TestDbContext>(new TestInitializer());
        }

        public virtual DbSet<TestEntity> TestEntities { get; set; }
        public virtual DbSet<Entity1> Entity1s { get; set; }
        public virtual DbSet<CollectionEntity> CollectionEntities { get; set; }
        public virtual DbSet<CollectionEntityInit> CollectionEntityInits { get; set; }
        public virtual DbSet<EbCollectionEntity> EbCollectionEntitys { get; set; }
        public virtual DbSet<ProtectedPropertyEntity> ProtectedPropertyEntitys { get; set; }

        public virtual DbSet<FlagDeleted1Entity> FlagDeleted1Entities { get; set; }
        public virtual DbSet<FlagDeletedEntityBase> FlagDeletedEntities { get; set; }


        public override int SaveChanges()
        {
            EfUtil.UndoUnmodified(ChangeTracker);
            /*
            foreach(var entry in ChangeTracker.Entries().Where( x => x.State == EntityState.Modified ))
            {
                bool modified = false;
                foreach (var propertyName in entry.OriginalValues.PropertyNames)
                {
                    var oldValue = entry.OriginalValues.GetValue<object>(propertyName);
                    var newValue = entry.OriginalValues.GetValue<object>(propertyName);
                    modified = modified || !JohnLambe.Util.ObjectUtil.CompareEqual(oldValue, newValue);
                }
                if (!modified)
                    entry.State = EntityState.Unchanged;
            }
            */

            return base.SaveChanges();
        }
    }

    public class TestEntity : IEntityBeforeSaveChanges
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public void BeforeSaveChanges()
        {
            Name = Name + "(BeforeSave)";
        }

        public override string ToString()
        {
            return base.ToString() + " Id=" + Id + "; Name=" + Name;
        }
    }

    public class FlagDeleted1Entity : JohnLambe.Util.Db.FlagDeletedEntityBase
    {

        public string Value { get; set; }

    }
}
