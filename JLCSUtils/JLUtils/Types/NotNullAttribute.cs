
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Types
{
    /// <summary>
    /// Specifies that the attributed item must not be null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotNullAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
                return ValidationResult.Success;
            else
                return new ValidationResult("Value cannot be null");
        }

        /// <summary>
        /// Indicates whether the value of this instance is the default value for this class.
        /// </summary>
        /// <returns>Always true since there are no added properties.</returns>
        public override bool IsDefaultAttribute() => true;
    }
}