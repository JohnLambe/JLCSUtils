using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Validation;


namespace JohnLambe.Tests.JLUtilsTest.Validation
{
    public class TestModel
    {
        [InvalidValue("X")]
        public string Property1 { get; set; }

        public string SetterValidatedProperty
        {
            get { return _setterValidatedProperty; }
            set
            {
                ValidatorEx.Instance.ValidateValueException<string>(this, ref value, nameof(SetterValidatedProperty));
            }
        }
        protected string _setterValidatedProperty;
    }
}
