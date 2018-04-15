using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Collections;

namespace JohnLambe.Util.Validation
{
    /// <summary>
    /// Validates a nested object.
    /// </summary>
    public class NestedValidationAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// If the attributed member is an <see cref="IEnumerable"/>, its elements are validated.
        /// </summary>
        public virtual bool ValidateElements { get; set; }

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);

            if(value != validationContext.ObjectInstance)   // ignore if it is a reference to the containing object (self-reference) since this would cause an infinite loop
            {
                ValidateNested(ref value, validationContext, results);

                if(ValidateElements)
                {
                    if (value is IEnumerable)
                    {
                        foreach (var element in (IEnumerable)value)
                        {
                            ValidateNested(ref value, validationContext, results);
                        }
                    }
                    else
                    {
                        results.Add("CONFIGURATION ERROR: " + validationContext.DisplayName + " does not implement " + nameof(IEnumerable));
                    }
                }
            }
        }

        protected virtual void ValidateNested(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            List<object> ancestors = (List<object>) validationContext.Items.GetOrCreate(ItemsKey, () => new List<object>());
            if (!ancestors.Contains(value))    // ignore if it is an ancestor of itself (circular reference)
            {
                ancestors.Add(value);

                Validator.TryValidateObject(value, results);
                // Note: messages generated will have field names relative to the contained object.
            }
        }

        protected static object ItemsKey = new object();

        public static IEnumerable<object> GetAncestors(ValidationContext validationContext)
        {
            return validationContext.Items[ItemsKey] as IEnumerable<object>;
        }

        public ValidatorEx Validator => ValidatorEx.Instance;
    }
}
