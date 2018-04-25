using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Text;

namespace JohnLambe.Util.Validation
{
    /// <summary>
    /// Validation attribute that compares the attributed property to another property on the same instance.
    /// <para>
    /// If either value is null, it validates as valid.
    /// </para>
    /// </summary>
    public class ComparePropertyValidationAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// The name of the property to compare to.
        /// This can support nested properties in the format of <see cref="ReflectionUtil.TryGetPropertyValue{T}(object, string, PropertyNullabilityModifier)"/>.
        /// </summary>
        [PropertyName]
        public virtual string PropertyName { get; set; }

        //public virtual bool Not { get; set; }

        public virtual ComparisonOperator Operator { get; set; } = ComparisonOperator.Any;

        //public virtual StringComparisonOperator StringOperator { get; set; }

        //public virtual bool Convert { get; set; } = false;  // convert values to the same type.

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);

            if (value != null)
            {
                var instance = validationContext.ObjectInstance;
                var property = ReflectionUtil.GetProperty(ref instance, PropertyName);  // get the property to compare to

                if (property == null)
                {
                    results.Add("Invalid property name: " + PropertyName);
                }

                var propertyValue = property.GetValue(instance);

                if (propertyValue != null)
                {
                    if (Operator != ComparisonOperator.Any)
                    {
                        if (!Operator.Compare(value, propertyValue))
                        {
                            results.Add(validationContext.DisplayName + " must be " + Operator.DisplayName() + " " + CaptionUtil.GetDisplayName(property));
                        }
                    }
                }
            }
        }
    }

}
