using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.Db.Ef;
using JohnLambe.Util.Db;

namespace JLUtilsEFTest.Db.Ef
{
    [TestClass]
    public class FlagDeletedTest
    {
        [TestMethod]
        [TestCategory("Db")]
        public virtual void QueryNonDeleted()
        {
            var repository = new EfMutableDatabaseRepository<FlagDeleted1Entity>(new TestDbContext());

            Console.Out.WriteLine(repository.AsQueryable().FirstOrDefault());

        }

        [TestMethod]
        [TestCategory("Db")]
        public virtual void QueryNoActiveFlag()
        {
            var repository = new EfMutableDatabaseRepository<TestEntity>(new TestDbContext());

            Console.Out.WriteLine(repository.AsQueryable().FirstOrDefault());

        }

    }

    /*
    public class FlagDeletedEntity : FlagDeletedEntityBase
    {
    }
    */
}
