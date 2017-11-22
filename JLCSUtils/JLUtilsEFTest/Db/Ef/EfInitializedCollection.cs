using JohnLambe.Util.Db.Ef;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLUtilsEFTest.Db.Ef.CollectionsInitialized
{
    public class CollectionEntityInit
    { 
        [Key]
        public virtual int Id { get; set; }

        [InverseProperty(nameof(CollectionItemEntityInit.Parent))]
        public virtual ICollection<CollectionItemEntityInit> Children
        {
            get
            {
                if (_children == null && !EfUtil.IsEfClass(this))              // don't initialize if this is the generated Entity Framework subclass (because that would prevent lazy loading)
                    _children = new List<CollectionItemEntityInit>();
                return _children;
            }
            set { _children = value; }
        }

        protected ICollection<CollectionItemEntityInit> _children;
    }

    public class CollectionItemEntityInit
    {
        [Key]
        public virtual int Id { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual CollectionEntityInit Parent { get; set; }
        public virtual int? ParentId { get; set; }
    }


    [TestClass]
    public class EfCollectionsTest
    {
        TestDbContext _dbContext = new TestDbContext();

        /// <summary>
        /// Collection is initialized when created by calling constructor directly,
        /// but not when created by Entity Framework (so that it will be lazily loaded).
        /// </summary>
        [TestMethod]
        [TestCategory("Db")]
        public void LazyLoad_Initialized()
        {
            // Arrange:
            var collection = new CollectionEntityInit()
            {
            };
            Assert.AreNotEqual(collection.Children, null);  // initialized to empty
            _dbContext.Set<CollectionEntityInit>().Add(collection);

            var item = new CollectionItemEntityInit()   // new item in the collection
            {
                Parent = collection
            };
            _dbContext.Set<CollectionItemEntityInit>().Add(item);
            _dbContext.SaveChanges();

            Assert.AreNotEqual(collection.Children, null);  // initialized to empty

            // Act:
            var reloadCollection = _dbContext.Set<CollectionEntityInit>().Find(collection.Id);

            Assert.AreEqual(reloadCollection, collection);
            Assert.AreNotEqual(reloadCollection.Children, null);  // initialized to null

            var context2 = new TestDbContext();
            var reloadCollection2 = context2.Set<CollectionEntityInit>().Find(collection.Id);

            // Assert:
            Assert.AreNotEqual(reloadCollection2, collection);    // doesn't return the cached instance (from the other context)
            Assert.AreNotEqual(null, reloadCollection2.Children);
            Assert.AreEqual(1, reloadCollection2.Children.Count);
            Assert.AreEqual(item.Id, reloadCollection2.Children.First()?.Id);
        }

        /*
        /// <summary>
        /// The collection property cannot be assigned by the consumer of the entity.
        /// </summary>
        [TestMethod]
        public void LazyLoad_Initialized()
        {
            // Arrange:
            var collection = new CollectionEntityInit()
            {
            };
            _dbContext.Set<CollectionEntityInit>().Add(collection);

            var item = new CollectionItemEntityInit()
            {
                Parent = collection
            };
            _dbContext.SaveChanges();

            var context2 = new TestDbContext();
            var reloadCollection2 = context2.Set<CollectionEntityInit>().Find(collection.Id);

            // Act:
            reloadCollection2.Children = new List<CollectionItemEntityInit>();   // throws InvalidOperationException
        }
        */
    }
}
