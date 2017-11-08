using JLUtilsEFTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Db.Ef
{
    public class ProtectedPropertyEntity
    {
        public int Id { get; set; }

        [NotMapped]
        public double PublicProperty
        {
            get { return ProtectedSetter / 100.0; }
            set { ProtectedSetter = (int)(value * 100); }
        }

        public virtual int ProtectedSetter { get; protected set; }  // Mapped

        public virtual int ProtectedGetter { protected get; set; }  // Not mapped

        [Column("ProtectedProperty")]
        protected virtual int ProtectedProperty { get; set; }    // Not mapped
    }


    [TestClass]
    public class ProtectedPropertiesTest
    {
        TestDbContext _dbContext = new TestDbContext();

        [TestMethod]
        public void ProtectedSetter()
        {
            // Arrange:
            var e1 = new ProtectedPropertyEntity()
            {
                PublicProperty = 123.45
            };

            // Act:
            _dbContext.Set<ProtectedPropertyEntity>().Add(e1);
            _dbContext.SaveChanges();

            var dbContext2 = new TestDbContext();
            var reloaded = dbContext2.Set<ProtectedPropertyEntity>().Find(e1.Id);

            // Assert:
            Assert.AreEqual(reloaded.PublicProperty, 123.45);
            Assert.AreEqual(reloaded.ProtectedSetter, 12345);
        }

    }

}

