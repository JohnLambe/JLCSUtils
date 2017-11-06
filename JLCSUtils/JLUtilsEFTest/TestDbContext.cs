using JLUtilsEFTest.Db.Ef;
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
        public virtual DbSet<CollectionEntityInit> CollectionEntityInit { get; set; }
        
    }


    public class TestEntity : IEntityBeforeSaveChanges
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public void BeforeSaveChanges()
        {
            Name = Name + "(BeforeSave)";
        }

        public override string ToString()
        {
            return base.ToString() + " Id=" + Id + "; Name=" + Name;
        }
    }
}
