using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Db.Ef;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JLUtilsEFTest.Db.Ef
{
    [TestClass]
    public class EfUtilTest
    {
        [TestMethod]
        [TestCategory("Db")]
        //[Timeout(5000)]
        public void FormatErrorsException()
        {
            // Arrange:

            var dbSet = _dbContext.Set<Entity1>();

            var instance = new Entity1()
            {
                Name = "012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789",
                IntField = 20
            };
            dbSet.Add(instance);

            // Act:
            Exception result = null;

            try
            {
                _dbContext.SaveChanges();
            }
            catch(DbEntityValidationException ex)
            {
                result = EfUtil.FormatErrorsException(ex);
            }

            Console.WriteLine(result.Message);

            // Assert:
            Assert.AreEqual(@"There is something invalid in the information being saved. The following errors are reported:
Entity 1
  Name: The field Name must be a string or array type with a maximum length of '100'.
  Int Field: The field IntField must be between 0 and 10.
", result.Message);
        }

        TestDbContext _dbContext = new TestDbContext();

    }
}
