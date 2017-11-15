using JohnLambe.Util.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Validation;

namespace JohnLambe.Tests.JLUtilsTest.Validation
{
    [TestClass]
    public class ValidationContextExtensionTest
    {
        [TestMethod]
        public void GetState_Null()
        {
            // Arrange:
            ValidationContext ctx = null;
            // Act/Assert:
            Assert.AreEqual(ValidationState.Default, ctx.GetState());
        }

        [TestMethod]
        public void GetState_Empty()
        {
            // Arrange:
            ValidationContext ctx = new ValidationContext(new object());
            // Act/Assert:
            Assert.AreEqual(ValidationState.Default, ctx.GetState());
        }

        [TestMethod]
        public void SetGetState()
        {
            // Arrange:
            ValidationContext ctx = new ValidationContext(this);
            ValidationContextExtension.SetState(ctx, ValidationState.LiveInput);

            // Act/Assert:
            Assert.AreEqual(ValidationState.LiveInput, ctx.GetState());
        }

        [TestMethod]
        public void SetGetSupportedFeatures()
        {
            // Arrange:
            ValidationContext ctx = new ValidationContext(5);
            Assert.AreEqual(ValidationFeatures.None, ctx.GetSupportedFeatures());

            ValidationContextExtension.SetSupportedFeatures(ctx, ValidationFeatures.Warnings);
            ValidationContextExtension.SetSupportedFeatures(ctx, ValidationFeatures.ValidateWithoutObject | ValidationFeatures.Modification);  // overwrite
            ValidationContextExtension.SetState(ctx, ValidationState.LiveInput);  // should have no effect

            // Act/Assert:
            Assert.AreEqual(ValidationFeatures.ValidateWithoutObject | ValidationFeatures.Modification, ctx.GetSupportedFeatures());
        }

        [TestMethod]
        public void GetSupportedFeatures_Null()
        {
            // Arrange:
            ValidationContext ctx = null;
            // Act/Assert:
            Assert.AreEqual(ValidationFeatures.None, ctx.GetSupportedFeatures());
        }
    }
}
