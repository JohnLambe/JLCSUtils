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

        [InvalidValue("X")]
        [InvalidValue("Invalid")]
        public string SetterValidatedProperty
        {
            get { return _setterValidatedProperty; }
            set
            {
                ValidatorEx.Instance.SetValue(this, ref _setterValidatedProperty, value, nameof(SetterValidatedProperty));
                /*
                ValidatorEx.Instance.ValidateValue<string>(this, ref value, nameof(SetterValidatedProperty));
                _setterValidatedProperty = value;
                */
            }
        }
        protected string _setterValidatedProperty;
    }
}
