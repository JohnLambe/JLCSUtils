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
    /// <para>
    /// This can be placed on many types of item. For most of them, this is just for documentation purposes.
    /// (On properties, it functions as a normal <see cref="ValidationAttribute"/>.)
    /// </para>
    /// <para>
    /// When applied to a collection, the nullability specification applies to the collection itself, not the elements.
    /// </para>
    /// <para>
    /// Primitive types:<br/>
    /// A <see cref="NullableAttribute"/> on a non-nullable type could never be valid.
    /// This attribute applies to nullable primitives just like reference types, but placing a <see cref="NullabilityAttribute"/>
    /// attribute on them in unnecessary since they can be assumed to be nullable unless otherwise indicated (since they have a non-nullable equivalent),
    /// and using the non-nullable type is preferred to using a <see cref="NotNullAttribute"/>.
    /// </para>
    /// </summary>
    //| This could be allowed on methods to apply the setting to all parameters, but that could be error-prone (a developer might have intended to apply it to the return value).
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue
          | AttributeTargets.Class | AttributeTargets.Delegate | AttributeTargets.Interface,
        AllowMultiple = false, Inherited = true)]
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
    /// <para>When used on parameters, passing null is not valid, and it is recommended that it should cause an <see cref="ArgumentNullException"/>.</para>
    /// <para>
    /// When applied to a type it means that whenever the type is used, it should not be assigned null.
    /// </para>
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

        /// <inheritdoc cref="NullabilityAttribute.IsNullable"/>
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
        /// <inheritdoc cref="NullabilityAttribute.IsNullable"/>
        public override bool IsNullable => true;

        // see Nullability.IsDefaultAttribute() - this must be overridden if any properties settable when declaring an attribute are added here or in a subclass.
    }

}