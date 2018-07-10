using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.Db.Ef;

namespace JLUtilsEFTest
{
    [TestClass]
    public class EntityEventsTest
    {
        /// <summary>
        /// Test firing of <see cref="IEntityBeforeSaveChanges.BeforeSaveChanges"/>.
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        [Timeout(5000)]
        public void BeforeSave()
        {
            // Arrange:
            EntityEvents.RegisterWith(_dbContext);

            var b = new Entity2() { Name = "Test" };
            _dbContext.Set<Entity2>().Add(b);

            // Act:
            _dbContext.SaveChanges();

            foreach(var x in _dbContext.TestEntities)
            {
                Console.Out.WriteLine(x);
            }

            Console.WriteLine("New object: " + b);

            // Assert:
            Assert.IsTrue(b.Name.Contains("BeforeSave"));
        }

        TestDbContext _dbContext = new TestDbContext();
    }
}
