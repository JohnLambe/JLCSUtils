using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Db.Ef;

namespace JLUtilsEFTest.Db.Ef
{

    [TestClass]
    public class DbSetUtilTest
    {
        [TestMethod]
        [TestCategory("Failing")]
        [Timeout(5000)]
        public void AddIfNotExists()
        {
            // Arrange:


            // Act:
            _dbContext.Set<TestEntity>().AddIfNotExists(x => x.Name,
                new TestEntity() { Name = "Test1" },
                new TestEntity() { Name = "Test2" }
                );

        }

        [TestMethod]
        [TestCategory("Db")]
        [Timeout(5000)]
        public void AddIfNotExists1()
        {
            // Arrange:


            // Act:
            _dbContext.Set<TestEntity>().AddIfNotExists((set,item) => set.Where(x => x.Name == item.Name).Any(),
                new TestEntity() { Name = "Test1" },
                new TestEntity() { Name = "Test2" }
                );

        }

        TestDbContext _dbContext = new TestDbContext();
    }

}
