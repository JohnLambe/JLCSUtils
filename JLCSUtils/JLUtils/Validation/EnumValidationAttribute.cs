using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Reflection;

namespace JohnLambe.Util.Validation
{
    public class EnumValidationAttribute : ValidationAttributeBase
    {
        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            if (!EnumUtil.ValidateEnumValue((Enum)value))
                results.Add(ErrorMessage ?? validationContext.DisplayName + " has an invalid value");
        }
    }
}