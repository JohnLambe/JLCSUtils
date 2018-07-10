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
            _dbContext.Set<Entity2>().AddIfNotExists(x => x.Name,
                new Entity2() { Name = "Test1" },
                new Entity2() { Name = "Test2" }
                );

        }

        [TestMethod]
        [TestCategory("Db")]
        [Timeout(5000)]
        public void AddIfNotExists1()
        {
            // Arrange:


            // Act:
            _dbContext.Set<Entity2>().AddIfNotExists((set,item) => set.Where(x => x.Name == item.Name).Any(),
                new Entity2() { Name = "Test1" },
                new Entity2() { Name = "Test2" }
                );

        }

        TestDbContext _dbContext = new TestDbContext();
    }

}
