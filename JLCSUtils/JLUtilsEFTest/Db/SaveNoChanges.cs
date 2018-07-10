using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.Db.Ef;
using JohnLambe.Util.Collections;

namespace JLUtilsEFTest.Db
{
    [TestClass]
    public class SaveNoChanges
    {
        /// <summary>
        /// Test firing of <see cref="IEntityBeforeSaveChanges.BeforeSaveChanges"/>.
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        //[Timeout(5000)]
        public void SaveWithNoChanges()
        {
            // Arrange:
            EntityEvents.RegisterWith(_dbContext);

            var dbSet = _dbContext.Set<Entity2>();

            var obj = dbSet.Find(123456);
            if (obj == null)
            {
                var b = new Entity2() { Id = 123456, Name = "No Changes Test" };
                //byte[] rowVersion1 = b.RowVersion;
                dbSet.Add(b);
                _dbContext.SaveChanges();

                //Assert.AreNotEqual(rowVersion1, obj.RowVersion);

                obj = dbSet.Find(b.Id);
            }

            byte[] rowVersion = (byte[])obj.RowVersion.Clone();

            // Act:
            _dbContext.Entry(obj).State = System.Data.Entity.EntityState.Modified;
            _dbContext.SaveChanges();

            // Assert:
            Assert.AreEqual(CollectionUtil.CollectionToString(rowVersion), CollectionUtil.CollectionToString(obj.RowVersion));  // check that RowVersion has not changed
        }

        TestDbContext _dbContext = new TestDbContext();
    }
}
