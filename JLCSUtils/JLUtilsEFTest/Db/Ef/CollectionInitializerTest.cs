using JohnLambe.Util.Db.Ef;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLUtilsEFTest.Db.Ef.CollectionInitializerTst
{
    public class EbCollectionEntity : EntityBase
    {
        [Key]
        public virtual int Id { get; set; }

        [InverseProperty(nameof(EbCollectionItemEntity.Parent))]
        public virtual ICollection<EbCollectionItemEntity> Children { get; set; }
    }

    public class EbCollectionItemEntity : EntityBase
    {
        [Key]
        public virtual int Id { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual EbCollectionEntity Parent { get; set; }
        public virtual int? ParentId { get; set; }
    }


    [TestClass]
    public class CollectionInitializerTest
    {
        TestDbContext _dbContext = new TestDbContext();

        /// <summary>
        /// The collection is not initialized on adding and saving (it remains null, NOT an empty collection).
        /// </summary>
        [TestMethod]
        public void InitializeWithCollectionProperty()
        {
            // Arrange:
            var e1 = new EbCollectionEntity()
            {
            };
            Assert.AreNotEqual(null, e1.Children);  // initialized

            _dbContext.Set<EbCollectionEntity>().Add(e1);

            Assert.AreNotEqual(null, e1.Children);  // initialized

            // Act:
            _dbContext.SaveChanges();

            // Assert:
            Assert.AreNotEqual(null, e1.Children);  // initialized
        }

        /// <summary>
        /// Lazy loading occurs on reading the collection property after loading the referencing entity, 
        /// but not when the cached referencing entity is re-fetched (it couldn't have overriden the property).
        /// </summary>
        [TestMethod]
        public void LazyLoad()
        {
            // Arrange:
            var collection = new EbCollectionEntity()
            {
            };
            _dbContext.Set<EbCollectionEntity>().Add(collection);

            var item = new EbCollectionItemEntity()   // new item in the collection
            {
                Parent = collection
            };

            Assert.AreNotEqual(null, collection.Children);

            _dbContext.Set<EbCollectionItemEntity>().Add(item);
            Assert.AreNotEqual(null, collection.Children);     // collection.Children has been populated

            _dbContext.SaveChanges();

            Assert.AreNotEqual(null, collection.Children);

            // Act:
            var reloadCollection = _dbContext.Set<EbCollectionEntity>().Find(collection.Id);

            Assert.AreEqual(reloadCollection, collection);
            Assert.AreNotEqual(null, reloadCollection.Children);

            var context2 = new TestDbContext();
            var reloadCollection2 = context2.Set<EbCollectionEntity>().Find(collection.Id);

            Assert.AreNotEqual(reloadCollection2, collection);    // doesn't return the cached instance (from the other context)
            Assert.AreNotEqual(null, reloadCollection2.Children);
            Assert.AreEqual(item.Id, reloadCollection2.Children.First()?.Id);
        }

        /// <summary>
        /// If there are no items in the collection, lazy loading populates an empty collection.
        /// </summary>
        [TestMethod]
        public void LazyLoad_Empty()
        {
            // Arrange:
            var collection = new EbCollectionEntity()
            {
            };
            _dbContext.Set<EbCollectionEntity>().Add(collection);

            _dbContext.SaveChanges();

            // Act:
            var reloadCollection = _dbContext.Set<EbCollectionEntity>().Find(collection.Id);

            Assert.AreEqual(reloadCollection, collection);
            Assert.AreNotEqual(null, reloadCollection.Children);

            var context2 = new TestDbContext();
            var reloadCollection2 = context2.Set<EbCollectionEntity>().Find(collection.Id);

            // Assert:
            Assert.AreNotEqual(reloadCollection2, collection);    // doesn't return the cached instance (from the other context)
            Assert.AreNotEqual(null, reloadCollection2.Children);
            Assert.AreEqual(0, reloadCollection2.Children.Count);
        }

        /// <summary>
        /// The collection property cannot be assigned by the consumer of the entity.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LazyLoad_TryAssign()
        {
            // Arrange:
            var collection = new EbCollectionEntity()
            {
            };
            _dbContext.Set<EbCollectionEntity>().Add(collection);

            var item = new EbCollectionItemEntity()
            {
                Parent = collection
            };
            _dbContext.SaveChanges();

            var context2 = new TestDbContext();
            var reloadCollection2 = context2.Set<EbCollectionEntity>().Find(collection.Id);

            // Act:
            reloadCollection2.Children = new List<EbCollectionItemEntity>();   // throws InvalidOperationException
            // Assigning in the constructor CollectionItemEntity would also throw this exception.
        }

    }
}
