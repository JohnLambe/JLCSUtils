using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnLambe.Util.Validation;
using System.ComponentModel.DataAnnotations;

namespace JohnLambe.Tests.JLUtilsTest.Validation
{
    [TestClass]
    public class ValidatorExTest
    {
        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateObject()
        {
            // Arrange:
            _model.Property1 = "X";

            // Act:
//            Validator.ValidateObject(_model, new ValidationContext(_model));
            _validator.ValidateObject(_model);
        }

        [TestMethod]
        public void TryValidateObject()
        {
            // Arrange:
            _model.Property1 = "X";
            var results = new ValidationResults();

            // Act:
            //            var result = Validator.TryValidateObject(_model, new ValidationContext(_model), results);
            var result = _validator.TryValidateObject(_model, results);

            // Assert:
            Assert.AreEqual(false, result);
            Assert.AreEqual(false, results.IsValid);
            Assert.IsTrue(results.Count > 0);
            Console.Out.WriteLine("Error message: " + results.First().ErrorMessage);
            Console.Out.WriteLine("Validation result: " + results.First());
        }


        #region ValidateProperty

        [TestMethod]
        public void ValidateProperty()
        {
            // Act:
            _validator.TryValidateProperty(_model, nameof(_model.Property1), _results);
            Console.WriteLine(_results);

            // Assert:
            Assert.IsTrue(_results.IsValid);
        }

        [TestMethod]
        public void ValidateProperty_Fail()
        {
            _model.Property1 = "X";
            _validator.TryValidateProperty(_model, nameof(_model.Property1), _results);
            Console.WriteLine(_results);

            // Assert:
            Assert.IsFalse(_results.IsValid);
        }

        #endregion

        #region SetValue

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void SetValue_Fail()
        {
            _model.SetterValidatedProperty = "Invalid";
        }

        [TestMethod]
        public void SetValue_Success()
        {
            _model.SetterValidatedProperty = "Valid";
        }

        #endregion

        public ValidatorEx _validator = new ValidatorEx();
        public TestModel _model = new TestModel();
        public ValidationResults _results = new ValidationResults();
    }
}
