using JohnLambe.Util.Documentation;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Validation
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)] //TODO: Review
    public abstract class ValidationAttributeBase : System.ComponentModel.DataAnnotations.ValidationAttribute, IProvidesDescription
    {
        /// <summary>
        /// If this is not <see cref="ValidationResultType.Error"/>, then when an item fails validation,
        /// it generates a message of that type.
        /// If it is <see cref="ValidationResultType.None"/>, the validation of this attribute is not done (it may be used for tagging an item as a certain type, etc.).
        /// </summary>
        public virtual ValidationResultType ResultType { get; set; } = ValidationResultType.Error;

        //
        // Summary:
        //     Validates the specified value with respect to the current validation attribute.
        //
        // Parameters:
        //   value:
        //     The value to validate.
        //
        //   validationContext:
        //     The context information about the validation operation.
        //
        // Returns:
        //     An instance of the System.ComponentModel.DataAnnotations.ValidationResult class.

        // Override this only to change the logic of this implementation, not to do the actual validation of the derived type.
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (ResultType == ValidationResultType.None)
                return ValidationResult.Success;           // treat as always valid

            if (ResultType < ValidationResultType.Error                    // if this can return only warnings / messages, or modify the value
                && !validationContext.GetSupportedFeatures().HasAnyFlag(ValidationFeatures.Warnings | ValidationFeatures.Modification)   // and that is not supported
                )
            {
                return ValidationResult.Success;    // treat as valid
            }

            var results = new ValidationResults(validationContext.GetSupportedFeatures(), ResultType);
            IsValid(ref value, validationContext, results);
            return results.Result;
        }

        /// <summary>
        /// `IsValid` overload providing more context information, and the ability to change the value.
        /// Override this instead of the other overalods.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <param name="results"></param>
        protected virtual void IsValid([Nullable] ref object value, [NotNull] ValidationContext validationContext, [NotNull] ValidationResults results)
        {
        }

        /// <summary>
        /// A human-readable text description of this validation rule.
        /// <para>If no description is explicitly defined on an instance (or this is set to null), this returns <see cref="DefaultDescritpion"/>.</para>
        /// </summary>
        public virtual string Description
        {
            get { return _description ?? DefaultDescritpion ?? GeneralDescription; }
            set { _description = value; }
        }
        protected string _description;

        #region IProvidesDescription

        /// <summary>
        /// Default description for this type of rule, for when a specific description is not given (in the <see cref="Description"/> property).
        /// This may take account of the properties of the instance.
        /// </summary>
        public virtual string DefaultDescritpion => GeneralDescription;

        /// <summary>
        /// Describes the type of validation rule(s).
        /// This does NOT take account of an properties of the instance.
        /// </summary>
        public virtual string GeneralDescription => null;

        #endregion
    }
}
