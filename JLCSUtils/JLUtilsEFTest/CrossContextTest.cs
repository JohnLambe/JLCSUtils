using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.Db.Ef;
using JohnLambe.Util.Collections;
using JLUtilsEFTest.Db.Ef;
using System.Linq;

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


        /// <summary>
        /// Add an entity that references an unattached entity that should not be added.
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        public void AdjustStates()
        {
            // Arrange:
            var dbSet = _dbContext.Set<Entity1>();

            var a = new Entity1()
            {
                Name = "Reference existing test"
            };
            var b = new Entity3()
            {
                Id = 1,
                Name = "don't save"
            };
            a.Reference = b;

            Console.WriteLine(_dbContext.Entry(b).State);

            // This would also work instead of the call below:
            //   _dbContext.Entry(b).State = System.Data.Entity.EntityState.Unchanged;

            dbSet.Add(a);

            Console.WriteLine(_dbContext.Entry(b).State);

            /*
            foreach (var e in _dbContext.ChangeTracker.Entries())
            {
                if (e.Entity is Entity3)
                {
                    if (e.State == System.Data.Entity.EntityState.Added && ((Entity3)(e.Entity)).Id != 0)
                        e.State = System.Data.Entity.EntityState.Unchanged;
                }
            }
            */

            EfUtil.AdjustStates(_dbContext, x => !(x is Entity3) || (x as Entity3)?.Id == 0);

            _dbContext.SaveChanges();
        }


        [TestMethod]
        [TestCategory("Db")]
        public void AdjustStates2()
        {
            // Arrange:
            var dbSet = _dbContext.Set<Entity1>();

            var b = new Entity3()
            {
                Id = 1,
                Name = "don't save"
            };

            var a1 = new Entity1()
            {
                Name = "Duplicate test 1",
                Reference = b
            };
            var a2 = new Entity1()
            {
                Name = "Duplicate test 2",
                Reference = b
            };

            Console.WriteLine(_dbContext.Entry(b).State);

            // This would also work instead of the call below:
            //   _dbContext.Entry(b).State = System.Data.Entity.EntityState.Unchanged;

            dbSet.Add(a1);
            dbSet.Add(a2);

            Console.WriteLine(_dbContext.Entry(b).State);

            /*
            foreach (var e in _dbContext.ChangeTracker.Entries())
            {
                if (e.Entity is Entity3)
                {
                    if (e.State == System.Data.Entity.EntityState.Added && ((Entity3)(e.Entity)).Id != 0)
                        e.State = System.Data.Entity.EntityState.Unchanged;
                }
            }
            */

            EfUtil.AdjustStates(_dbContext, x => !(x is Entity3) || (x as Entity3)?.Id == 0);

            _dbContext.SaveChanges();
        }


        [TestMethod]
        [TestCategory("Db")]
        public void NonUnique()
        {
            // Arrange:
            var dbSet = _dbContext.Set<Entity1>();

            var b1 = new Entity3()
            {
                Id = 1,
                Name = "don't save"
            };
            var b2 = new Entity3()
            {
                Id = 1,
                Name = "don't save"
            };

            var a1 = new Entity1()
            {
                Name = "Duplicate test 1",
                Reference = b1
            };
            var a2 = new Entity1()
            {
                Name = "Duplicate test 2",
                Reference = b2
            };

            Console.WriteLine(_dbContext.Entry(b1).State);

            // This would also work instead of the call below:
            //   _dbContext.Entry(b).State = System.Data.Entity.EntityState.Unchanged;


            dbSet.Add(a1);
            dbSet.Add(a2);


            var entry = _dbContext.ChangeTracker.Entries().Where(x => x.Entity == b2).Single();
            //            entry.State = System.Data.Entity.EntityState.Detached;

            var entry1 = _dbContext.ChangeTracker.Entries().Where(x => x.Entity == a2).Single();
            ((Entity1)(entry1.Entity)).Reference = b1;

            Console.WriteLine(_dbContext.Entry(b1).State);

            /*
            foreach (var e in _dbContext.ChangeTracker.Entries())
            {
                if (e.Entity is Entity3)
                {
                    if (e.State == System.Data.Entity.EntityState.Added && ((Entity3)(e.Entity)).Id != 0)
                        e.State = System.Data.Entity.EntityState.Unchanged;
                }
            }
            */

            EfUtil.AdjustStates(_dbContext, x => !(x is Entity3) || (x as Entity3)?.Id == 0, (e1,e2) => e1 is Entity3 && e2 is Entity3 && ((Entity3)e1).Id == ((Entity3)e2).Id);

            _dbContext.SaveChanges();
        }


        [TestMethod]
        [TestCategory("Db")]
        public void CopyToContext()
        {
            var a = _dbContext.Entity1s.Find(1);
            a.Name += "-";
            _dbContext.SaveChanges();


            var a2 = a;
//            var a2 = EfUtil.CopyToContext(_dbContext2, a);

            a2.Name = a2.Name + "+";

//            _dbContext2.ChangeTracker.DetectChanges();  // not necessary
            Console.Out.WriteLine(_dbContext2.Entry(a2).State);

            _dbContext.SaveChanges();
            _dbContext2.SaveChanges();
        }



        [TestMethod]
        [TestCategory("Db")]
        public void FindInContext()
        {
//            var q0 = _dbContext.Entity1s.ToList();// .Where(e => true) ;//.Where(e => e != null && e.Id == 1);//.SingleOrDefault();


            //            var query1 = EfUtil.FindInContext<Entity1>(_dbContext, 1, x => (x as Entity1)?.Id);
            //            Assert.AreEqual(null, query1);

            EfUtil.GetKeyDelegate getKeyDelegate = x => (x as Entity1)?.Id;
            object key = 1;
            //            var z = _dbContext.ChangeTracker.Entries().Where(e => getKeyDelegate(e) == key).FirstOrDefault()?.Entity;

            Entity1 found = null;
            _dbContext.ChangeTracker.Entries();
            /*
            foreach(var e1 in _dbContext.ChangeTracker.Entries())
            {
//                if (getKeyDelegate(e1) == key)
//                    found = e1.Entity as Entity1;
            }
            */

            /*
            var x1 = _dbContext.Entity1s;
            var x2 = x1.ToList();
            var x3 = x2.Where(e => e == null || e.Id == 1);
            */

            var q = _dbContext.Entity1s.Where(e => e != null && e.Id == 1);//.SingleOrDefault();
            var a = q.SingleOrDefault();
            _dbContext.Entry(a).State = System.Data.Entity.EntityState.Modified;

            foreach(var e1 in _dbContext.ChangeTracker.Entries())
            {
                Console.WriteLine(e1);
            }

            var query2 = EfUtil.FindInContext<Entity1>(_dbContext, 1, x => (x as Entity1)?.Id);
            Assert.AreEqual(1, query2.Id);
        }


        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        public void LoadFromContext()
        {
            var a = _dbContext.Set<Entity1>().Find(1);

            var a2 = EfUtil.LoadFromContext(_dbContext2, a, x => x is Entity1 ? ((Entity1)x).Id : (object)null );

            var a2_2 = EfUtil.LoadFromContext(_dbContext2, a, x => x is Entity1 ? ((Entity1)x).Id : (object)null);   // load same entity again from 2nd context

            // Assert:
            Assert.AreEqual(a2.Id, a.Id);  // same value
            Assert.AreNotEqual(a2, a);  // not the same reference

            Assert.AreEqual(a2_2.Id, a2.Id);  // same value
            Assert.AreEqual(a2, a2_2);  // same reference
        }


        TestDbContext _dbContext = new TestDbContext();
        TestDbContext _dbContext2 = new TestDbContext();
    }


}
