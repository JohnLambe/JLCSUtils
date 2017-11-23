using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLUtilsEFTest.Db.Ef.Collections
{
    public class CollectionEntity
    {
        [Key]
        public virtual int Id { get; set; }

        [InverseProperty(nameof(CollectionItemEntity.Parent))]
        public virtual ICollection<CollectionItemEntity> Children { get; set; }
        // Note:
        //  If we assigned this like this:  = new List<CollectionItemEntity>();
        //  it would NOT be lazy-loaded.
    }

    public class CollectionEntity2
    {
        [Key]
        public virtual int Id { get; set; }

        [InverseProperty(nameof(CollectionItemEntity.Parent))]
        public virtual ICollection<CollectionItemEntity> Children
        {
            get
            {
                if (_children == null)
                    _children = new List<CollectionItemEntity>();
                return _children;
            }
            set { _children = value; }
        }

        protected ICollection<CollectionItemEntity> _children;
    }

    public class CollectionItemEntity
    {
        [Key]
        public virtual int Id { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual CollectionEntity Parent { get; set; }
        public virtual int? ParentId { get; set; }
    }


    [TestClass]
    public class EfCollectionsTest
    {
        TestDbContext _dbContext = new TestDbContext();

        /// <summary>
        /// The collection is not initialized on adding and saving (it remains null, NOT an empty collection).
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        public void InitializeWithCollectionProperty()
        {
            // Arrange:
            var e1 = new CollectionEntity()
            {
            };

            _dbContext.Set<CollectionEntity>().Add(e1);

            Assert.AreEqual(null, e1.Children);  // not populated at this point

            // Act:
            _dbContext.SaveChanges();

            // Assert:
            Assert.AreEqual(null, e1.Children);  // not populated at this point
        }

        /// <summary>
        /// Lazy loading occurs on reading the collection property after loading the referencing entity, 
        /// but not when the cached referencing entity is re-fetched (it couldn't have overriden the property).
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        public void LazyLoad()
        {
            // Arrange:
            var collection = new CollectionEntity()
            {
            };
            _dbContext.Set<CollectionEntity>().Add(collection);
            
            var item = new CollectionItemEntity()   // new item in the collection
            {
                Parent = collection
            };

            Assert.AreEqual(null, collection.Children);

            _dbContext.Set<CollectionItemEntity>().Add(item);
            Assert.AreNotEqual(null, collection.Children);     // collection.Children has been populated

            _dbContext.SaveChanges();

            Assert.AreNotEqual(null, collection.Children);

            // Act:
            var reloadCollection = _dbContext.Set<CollectionEntity>().Find(collection.Id);

            Assert.AreEqual(reloadCollection, collection);
            Assert.AreNotEqual(null, reloadCollection.Children);

            var context2 = new TestDbContext();
            var reloadCollection2 = context2.Set<CollectionEntity>().Find(collection.Id);

            Assert.AreNotEqual(reloadCollection2, collection);    // doesn't return the cached instance (from the other context)
            Assert.AreNotEqual(null, reloadCollection2.Children);
            Assert.AreEqual(item.Id, reloadCollection2.Children.First()?.Id);
        }

        /// <summary>
        /// If there are no items in the collection, lazy loading populates an empty collection.
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        public void LazyLoad_Empty()
        {
            // Arrange:
            var collection = new CollectionEntity()
            {
            };
            _dbContext.Set<CollectionEntity>().Add(collection);

            _dbContext.SaveChanges();

            // Act:
            var reloadCollection = _dbContext.Set<CollectionEntity>().Find(collection.Id);

            Assert.AreEqual(reloadCollection, collection);
            Assert.AreEqual(null, reloadCollection.Children);

            var context2 = new TestDbContext();
            var reloadCollection2 = context2.Set<CollectionEntity>().Find(collection.Id);

            // Assert:
            Assert.AreNotEqual(reloadCollection2, collection);    // doesn't return the cached instance (from the other context)
            Assert.AreNotEqual(null, reloadCollection2.Children);
            Assert.AreEqual(0, reloadCollection2.Children.Count);
        }

        /// <summary>
        /// The collection property cannot be assigned by the consumer of the entity.
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void LazyLoad_TryAssign()
        {
            // Arrange:
            var collection = new CollectionEntity()
            {
            };
            _dbContext.Set<CollectionEntity>().Add(collection);

            var item = new CollectionItemEntity()
            {
                Parent = collection
            };
            _dbContext.SaveChanges();

            var context2 = new TestDbContext();
            var reloadCollection2 = context2.Set<CollectionEntity2>().Find(collection.Id);

            // Act:
            reloadCollection2.Children = new List<CollectionItemEntity>();   // throws InvalidOperationException
            // Assigning in the constructor CollectionItemEntity would also throw this exception.
        }

    }
}
