using JohnLambe.Util.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Validation;
using System.ComponentModel.DataAnnotations;

namespace JohnLambe.Tests.JLUtilsTest.Validation
{
    [TestClass]
    public class ValidatorExTest
    {
        [TestMethod]
        public void ValidateObject()
        {
            _model.Property1 = "X";
            Validator.ValidateObject(_model, new ValidationContext(_model));

        }


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

        public ValidatorEx _validator = new ValidatorEx();
        public TestModel _model = new TestModel();
        public ValidationResults _results = new ValidationResults();
    }
}
