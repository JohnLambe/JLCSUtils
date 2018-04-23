using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Reflection;

namespace JohnLambe.Util.Validation
{
    /// <summary>
    /// Validates that the attributed item has a valid value for its enum type (validated by <see cref="EnumUtil.ValidateEnumValue(Enum)"/>).
    /// null values are ignored (validated as valid).
    /// Validation fails if the value is not null and not an enum type.
    /// </summary>
    public class EnumValidationAttribute : ValidationAttributeBase
    {
        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            if (value != null && (!(value is Enum) || !EnumUtil.ValidateEnumValue((Enum)value)))
                results.Add(ErrorMessage ?? validationContext.DisplayName + " has an invalid value: " + value.ToString());
        }
    }
}