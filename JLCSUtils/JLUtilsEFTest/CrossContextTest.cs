using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.Db.Ef;
using JohnLambe.Util.Collections;
using JLUtilsEFTest.Db.Ef;

namespace JLUtilsEFTest
{
    [TestClass]
    public class CrossContextTest
    {
        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        //[Timeout(5000)]
        public void SaveWithNoChanges()
        {
            // Arrange:
            var dbSet = _dbContext.Set<Entity3>();

            var obj = dbSet.Find(123456);
            if (obj == null)
            {
                var b = new Entity3() { Id = 2512, Name = "Cross-context test" };
                //byte[] rowVersion1 = b.RowVersion;
                dbSet.Add(b);
                _dbContext.SaveChanges();

                //Assert.AreNotEqual(rowVersion1, obj.RowVersion);

                obj = dbSet.Find(b.Id);
            }

//            byte[] rowVersion = (byte[])obj.RowVersion.Clone();

            // Act:
            var dbSetEntity3 = _dbContext2.Set<Entity3>();
            obj.Name = obj.Name + "+";
            dbSetEntity3.Add(obj);
            _dbContext2.SaveChanges();

            var dbSet2 = _dbContext2.Set<Entity1>();

            var a = new Entity1();
            a.Name = "Cross-context test " + DateTime.Now.ToString();
            a.Reference = obj;

            dbSet2.Add(a);

//            dbSet2.Add(obj);

//            _dbContext.Entry(obj).State = System.Data.Entity.EntityState.Modified;

            _dbContext2.SaveChanges();

            // Assert:
//            Assert.AreEqual(CollectionUtil.CollectionToString(rowVersion), CollectionUtil.CollectionToString(obj.RowVersion));  // check that RowVersion has not changed
        }

        TestDbContext _dbContext = new TestDbContext();
        TestDbContext _dbContext2 = new TestDbContext();
    }
}
