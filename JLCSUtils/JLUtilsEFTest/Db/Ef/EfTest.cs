using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLUtilsEFTest.Db.Ef
{
    public class Entity1
    {
        [Key]
        public virtual int Id { get; set; }

        [MaxLength(100)]
        public virtual string Name { get; set; }

        [Range(0,10)]
        public virtual int IntField { get; set; }

        [ForeignKey(nameof(ReferenceId))]
        public virtual Entity3 Reference { get; set; }
        public virtual int? ReferenceId { get; set; }
    }

    public class Entity3
    {
        [Key]
        public virtual int Id { get; set; }

        public string Name { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }


    [TestClass]
    public class EfTest
    {
        TestDbContext _dbContext = new TestDbContext();

        /// <summary>
        /// The key value is assigned on saving an added entity (not on the Add() call), ignoring any value already assigned.
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        [Timeout(5000)]
        public void AssignKey()
        {
            // Arrange:
            const int InitialId = int.MaxValue - 1;

            var e1 = new Entity1()
            {
                Id = InitialId    // assign reference but not ID
            };
            _dbContext.Set<Entity1>().Add(e1);

            Console.WriteLine("e1.Id= " + e1.Id);
            Assert.AreEqual(InitialId, e1.Id);  // The Id is unchanged

            // Act:
            _dbContext.SaveChanges();
            Console.WriteLine("e1.Id= " + e1.Id);  // It's assigned now

            // Assert:
            Assert.AreNotEqual(InitialId, e1.Id);   // the new Id has been assigned, ignoring the one provided before saving
        }

        /// <summary>
        /// When assigning only the object of a foreign key,
        /// EF assigns the Id, on SaveChanges().
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        public void ReferenceIdPopulationTest()
        {
            // Arrange:
            var e2 = new Entity3();
            var e1 = new Entity1()
            {
                Reference = e2    // assign reference but not ID
            };

            _dbContext.Set<Entity1>().Add(e1);

            Console.WriteLine("e1.ReferenceId= " + e1.ReferenceId);
            Assert.AreEqual(null, e1.ReferenceId);  // The reference Id is not assigned at this point. (Would be 0 if not nullable).

            // Act:
            _dbContext.SaveChanges();
            Console.WriteLine("e1.ReferenceId= " + e1.ReferenceId);  // It's assigned now

            // Assert:
            Assert.AreNotEqual(null, e1.ReferenceId);    // the reference Id property is assigned
            Assert.AreEqual(e2.Id, e1.ReferenceId);   // it matches the (generated) Id of the referenced object
        }

        /// <summary>
        /// When assigning both the object (non-null) and the Id of a foreign key property,
        /// EF overwrites the Id, on SaveChanges().
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        public void ReferenceIdPopulationTest2()
        {
            // Arrange:
            const int WrongId = 1000000000;

            var e2 = new Entity3();
            var e1 = new Entity1()
            {
                Reference = e2,
                ReferenceId = WrongId
            };

            _dbContext.Set<Entity1>().Add(e1);

            Console.WriteLine("e1.ReferenceId= " + e1.ReferenceId);
            Assert.AreEqual(WrongId, e1.ReferenceId);  // The reference Id has not been modified at this point

            // Act:
            _dbContext.SaveChanges();
            Console.WriteLine("e1.ReferenceId= " + e1.ReferenceId);  // It's assigned now

            // Assert:
            Assert.AreNotEqual(0, e1.ReferenceId);
            Assert.AreEqual(e2.Id, e1.ReferenceId);   // the referenced Id has been modified to point to the referenced object
        }

        /// <summary>
        /// When assigning only the Id of a foreign key property,
        /// EF does NOT assign the object.
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        public void ReferencePopulationTest()
        {
            // Arrange:
            const int ReferencedId = 1;

            var e2 = new Entity3()
            {
                Id = ReferencedId
            };
            _dbContext.Set<Entity3>().Add(e2);
            _dbContext.SaveChanges();

            var e1 = new Entity1()
            {
                ReferenceId = ReferencedId
            };

            _dbContext.Set<Entity1>().Add(e1);

            Console.WriteLine("e1.ReferenceId= " + e1.ReferenceId);
            Assert.AreEqual(ReferencedId, e1.ReferenceId);  // The reference Id is unchanged
            Assert.AreEqual(null, e1.Reference);  // The reference is not assigned

            // Act:
            _dbContext.SaveChanges();
            Console.WriteLine("e1.ReferenceId= " + e1.ReferenceId);

            Assert.AreEqual(ReferencedId, e1.ReferenceId);
            Assert.AreEqual(null, e1.Reference);  // It's still not assigned
        }

        /// <summary>
        /// When assigning both the object and the Id of a foreign key object, with inconsistent values,
        /// EF assigns the Id (to match the object),
        /// on the Add() call.
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        public void ReferencePopulationTest2()
        {
            const int ReferencedId = 1;
            const int WrongId = 2000000000;

            var e2 = new Entity3()
            {
                Id = ReferencedId
            };
            _dbContext.Set<Entity3>().Add(e2);
            _dbContext.SaveChanges();

            var e2b = new Entity3()
            {
                Id = WrongId
            };
            _dbContext.Set<Entity3>().Add(e2b);
            _dbContext.SaveChanges();

//            Assert.AreEqual(WrongId,e2b.Id);

            var e1 = new Entity1()
            {
                Reference = e2b,            // doesn't match ReferenceId
                ReferenceId = ReferencedId
            };

            // Act:
            _dbContext.Set<Entity1>().Add(e1);

            Console.WriteLine("e1.ReferenceId= " + e1.ReferenceId);
            Assert.AreEqual(e2b.Id, e1.ReferenceId);  // The reference Id is changed
            Assert.AreEqual(e2b, e1.Reference);  // The reference is not assigned

            _dbContext.SaveChanges();
            Console.WriteLine("e1.ReferenceId= " + e1.ReferenceId);

            // Assert:
            Assert.AreEqual(e2b, e1.Reference);  // The object reference is unchanged
            Assert.AreEqual(e2b.Id, e1.ReferenceId);  // The reference Id is changed
        }
    }
}
