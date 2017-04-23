using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JohnLambe.Tests.JLUtilsTest.Types
{
    /// <summary>
    /// Specifies that the attributed item must not be null.
    /// </summary>
    [TestClass]
    public class NotNullAttributeTest
    {
        [TestMethod]
        public void NotNull()
        {
            // Arrange:
            var attribute = new NotNullAttribute();

            // Act/Assert:
            Assert.IsTrue(attribute.IsDefaultAttribute());
            Assert.IsTrue(attribute.IsValid(5));
            Assert.IsTrue(attribute.IsValid(""));
            Assert.IsFalse(attribute.IsValid(null));
        }
    }
}
