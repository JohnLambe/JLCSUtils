using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Types
{
    /// <summary>
    /// Attribute that specifies whether an item can be null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class NullabilityAttribute : ValidationAttribute
    {
        /// <summary>
        /// True iff the item is allowed to be null.
        /// </summary>
        public abstract bool IsNullable { get; }

        /// <summary>
        /// Indicates whether the value of this instance is the default value for this class.
        /// </summary>
        /// <returns>Always true since there are no added properties.</returns>
        public override bool IsDefaultAttribute() => true;
    }

    /// <summary>
    /// Specifies that the attributed item must not be null.
    /// <para>
    /// This can be placed on many types of item. For most of them, this is just for documentation purposes.
    /// (On properties, it functions as a normal <see cref="ValidationAttribute"/>.)
    /// </para>
    /// <para>When used on parameters, passing null is not valid, and it is recommended that it should cause an <see cref="ArgumentNullException"/>.</para>
    /// </summary>
    //| This could be validated automatically on parameters by LooselyCoupledEvent.
    public class NotNullAttribute : NullabilityAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
                return ValidationResult.Success;
            else
                return new ValidationResult("Value cannot be null");
        }

        public override bool IsNullable => false;

        // see Nullability.IsDefaultAttribute() - this must be overridden if any properties settable when declaring an attribute are added here or in a subclass.
    }

    /// <summary>
    /// Specifies that the attributed item is allowed to be null (for documentation purposes).
    /// <para>When used on parameters, passing null must not cause a <see cref="NullReferenceException"/>.</para>
    /// <para>Always validates as valid.</para>
    /// </summary>
    public class NullableAttribute : NullabilityAttribute
    {
        public override bool IsNullable => true;

        // see Nullability.IsDefaultAttribute() - this must be overridden if any properties settable when declaring an attribute are added here or in a subclass.
    }
}