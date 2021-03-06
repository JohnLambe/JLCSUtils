﻿using JohnLambe.Util.Documentation;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Text;

namespace JohnLambe.Util.Validation
{
    /// <summary>
    /// Base class for attributes that validate an attributed item, with some additional features (added to <see cref="ValidationAttribute"/>).
    /// <para>
    /// Use <see cref="ValidatorEx"/> to invoke validation of a value or property.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)] //TODO: Review
    public abstract class ValidationAttributeBase : System.ComponentModel.DataAnnotations.ValidationAttribute, IProvidesDescription //, IValidator
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

        /// <summary>
        /// <inheritdoc cref="ValidationAttribute.IsValid(object, ValidationContext)"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        /// <remarks>
        /// Override this only to change the logic of this implementation, not to do the actual validation of the derived type.
        /// </remarks>
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
            var initialValue = value;
            IsValid(ref value, validationContext, results);
            if(initialValue != value)
            {
                if (validationContext.GetSupportedFeatures().HasFlag(ValidationFeatures.Modification))  // if modification is supported
                {
                    results.NewValue = value;
                    results.Add(new ValidationResultEx(null, null, ValidationResultType.Updated, null, value));
                }
                else
                {
//TODO: test:                    results.Fail();      // modification was required but not supported
                }
            }
            if (results.IsValid)
                return ValidationResult.Success;
            else
                return results.Result;
        }

        /// <summary>
        /// <see cref="IsValid(object,ValidationContext)"/> overload providing more context information, and the ability to change the value.
        /// Override this instead of the other overloads.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <param name="results"></param>
        protected virtual void IsValid([Nullable] ref object value, [NotNull] ValidationContext validationContext, [NotNull] ValidationResults results)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns>true iff valid.</returns>
        public virtual bool TestValid(object value, [Nullable] ValidationContext validationContext)
        {
            return IsValid(value, validationContext).IsValid();
        }

        /// <summary>
        /// A human-readable text description of this validation rule.
        /// <para>If no description is explicitly defined on an instance (or this is set to null), this returns <see cref="DefaultDescription"/>.</para>
        /// </summary>
        public virtual string Description
        {
            get { return _description ?? DefaultDescription ?? GeneralDescription; }
            set { _description = value; }
        }
        protected string _description;

        /// <summary>
        /// Convert between the stored and displayed value.
        /// </summary>
        /// <param name="toDisplay"></param>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        public virtual void PreProcessForDisplay(bool toDisplay, [Nullable] ref object value, [Nullable] ValidationContext validationContext)
        {
        }

        #region IProvidesDescription

        /// <summary>
        /// Default description for this type of rule, for when a specific description is not given (in the <see cref="Description"/> property).
        /// This may take account of the properties of the instance.
        /// </summary>
        public virtual string DefaultDescription => GeneralDescription;

        /// <summary>
        /// Describes the type of validation rule(s).
        /// This does NOT take account of any properties of the instance.
        /// </summary>
        public virtual string GeneralDescription 
            => GetType().GetCustomAttribute<DescriptionAttribute>().Description;   // defaults to DescriptionAttribute on the class

        #endregion

        #region Formatting

        /// <summary>
        /// Provides a formatter for values of members to which this attribute is applied.
        /// Can implement <see cref="IFormatProvider.GetFormat(Type)"/>.
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public virtual object GetFormat(Type formatType) => null;

        /// <summary>
        /// Format a value of a member to which this attribute is applied (for display).
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public virtual string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return FormatUtil.FormatObject(format, arg, formatProvider);
        }

        #endregion
    }


    /*
    public interface IValidator
    {
//        void IsValid([Nullable] ref object value, [NotNull] ValidationContext validationContext, [NotNull] ValidationResults results);
        string Description { get; }
        void PreProcessForDisplay(bool toDisplay, [Nullable] ref object value, [Nullable] ValidationContext validationContext);
    }
    */
}
