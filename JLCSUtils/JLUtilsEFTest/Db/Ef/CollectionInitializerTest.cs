using JohnLambe.Util.Collections;
using JohnLambe.Util.Db;
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
    public class EbCollectionEntity1 : EntityBase
    {
        [Key]
        public virtual int Id { get; set; }

        [InverseProperty(nameof(EbCollectionItemEntity1.Parent))]
        public virtual ICollection<EbCollectionItemEntity1> Children { get; set; }  // should be initialized

        public virtual IAsyncResult Dummy1 { get; set; }         // not a collection
        public virtual ICollection<EbCollectionItemEntity1> Dummy2 { get; }  // can't be assigned
        public virtual List<EbCollectionItemEntity1> Dummy3 { get; }  // not supported

        [InitializeCollection(false)]
//        [InverseProperty(nameof(EbCollectionItemEntity1.Parent))]
        public virtual ICollection<EbCollectionItemEntity1> Dummy4 { get; set; }  // should NOT be initialized

        [InitializeCollection]
        public virtual ICollection<int> NonEntities { get; set; }  // should be initialized

        [InitializeCollection]
        protected virtual ICollection<EbCollectionItemEntity1> ProtectedCollection { get; set; }  // should be initialized
    }

    public class EbCollectionItemEntity1 : EntityBase
    {
        [Key]
        public virtual int Id { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual EbCollectionEntity1 Parent { get; set; }
        public virtual int? ParentId { get; set; }
    }

    public class EbCollectionEntitySubclass : EbCollectionEntity1
    {
        [InitializeCollection(true)]
        public ICollection<EbCollectionItemEntity1> WriteOnly
        {
            set { GetterForWriteOnly = value; }
        }
        public ICollection<EbCollectionItemEntity1> GetterForWriteOnly { get; protected set; }

        public object GetterForProtected => ProtectedCollection;
    }


    [TestClass]
    public class CollectionInitializerTest
    {
        TestDbContext _dbContext = new TestDbContext();

        /// <summary>
        /// The collection is not initialized on adding and saving (it remains null, NOT an empty collection).
        /// </summary>
        [TestMethod]
        public void InitializeCollections()
        {
            // Act:
            var e1 = new EbCollectionEntity1();

            // Assert:
            Assert.AreNotEqual(null, e1.Children);
            Assert.AreNotEqual(null, e1.NonEntities);

            Assert.AreEqual(null, e1.Dummy1);
            Assert.AreEqual(null, e1.Dummy2);
            Assert.AreEqual(null, e1.Dummy3);
            Assert.AreEqual(null, e1.Dummy4);
        }

        [TestMethod]
        public void InitializeCollections_Subclass()
        {
            // Act:
            var e1 = new EbCollectionEntitySubclass();

            // Assert:
            Assert.AreNotEqual(null, e1.GetterForWriteOnly);

            Assert.AreNotEqual(null, e1.Children);
            Assert.AreNotEqual(null, e1.NonEntities);

            Assert.AreEqual(null, e1.Dummy1);
            Assert.AreEqual(null, e1.Dummy2);
            Assert.AreEqual(null, e1.Dummy3);
            Assert.AreEqual(null, e1.Dummy4);
        }

        [TestMethod]
        public void InitializeCollections_Protected()
        {
            // Arrange:
            var e1 = new EbCollectionEntitySubclass();

            // Act:
            CollectionInitializer.InitializeInstance(e1, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert:
            Assert.AreNotEqual(null, e1.GetterForProtected);

            Assert.AreNotEqual(null, e1.GetterForWriteOnly);

            Assert.AreNotEqual(null, e1.Children);
            Assert.AreNotEqual(null, e1.NonEntities);

            Assert.AreEqual(null, e1.Dummy1);
            Assert.AreEqual(null, e1.Dummy2);
            Assert.AreEqual(null, e1.Dummy3);
            Assert.AreEqual(null, e1.Dummy4);
        }

        /// <summary>
        /// The collection is not initialized on adding and saving (it remains null, NOT an empty collection).
        /// </summary>
        [TestMethod]
        public void InitializeWithCollectionProperty()
        {
            // Arrange:
            var e1 = new EbCollectionEntity1()
            {
            };
            Assert.AreNotEqual(null, e1.Children);  // initialized

            _dbContext.Set<EbCollectionEntity1>().Add(e1);

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
            var collection = new EbCollectionEntity1()
            {
            };
            _dbContext.Set<EbCollectionEntity1>().Add(collection);

            var item = new EbCollectionItemEntity1()   // new item in the collection
            {
                Parent = collection
            };

            Assert.AreNotEqual(null, collection.Children);

            _dbContext.Set<EbCollectionItemEntity1>().Add(item);
            Assert.AreNotEqual(null, collection.Children);     // collection.Children has been populated

            _dbContext.SaveChanges();

            Assert.AreNotEqual(null, collection.Children);

            // Act:
            var reloadCollection = _dbContext.Set<EbCollectionEntity1>().Find(collection.Id);

            Assert.AreEqual(reloadCollection, collection);
            Assert.AreNotEqual(null, reloadCollection.Children);

            var context2 = new TestDbContext();
            var reloadCollection2 = context2.Set<EbCollectionEntity1>().Find(collection.Id);

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
            var collection = new EbCollectionEntity1()
            {
            };
            _dbContext.Set<EbCollectionEntity1>().Add(collection);

            _dbContext.SaveChanges();

            // Act:
            var reloadCollection = _dbContext.Set<EbCollectionEntity1>().Find(collection.Id);

            Assert.AreEqual(reloadCollection, collection);
            Assert.AreNotEqual(null, reloadCollection.Children);

            var context2 = new TestDbContext();
            var reloadCollection2 = context2.Set<EbCollectionEntity1>().Find(collection.Id);

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
            var collection = new EbCollectionEntity1()
            {
            };
            _dbContext.Set<EbCollectionEntity1>().Add(collection);

            var item = new EbCollectionItemEntity1()
            {
                Parent = collection
            };
            _dbContext.SaveChanges();

            var context2 = new TestDbContext();
            var reloadCollection2 = context2.Set<EbCollectionEntity1>().Find(collection.Id);

            // Act:
            reloadCollection2.Children = new List<EbCollectionItemEntity1>();   // throws InvalidOperationException
                                                                                // Assigning in the constructor CollectionItemEntity would also throw this exception.
        }

    }
}