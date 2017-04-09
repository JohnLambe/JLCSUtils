using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Text;

namespace JohnLambe.Util.Validation
{
    /// <summary>
    /// Validate objects or members according to <see cref="ValidationAttribute"/>s - extension of <see cref="Validator"/>.
    /// </summary>
    public class ValidatorEx
    {
        public ValidatorEx(ValidationFeatures features = ValidationFeatures.All)
        {
            SupportedFeatures = features;
        }

        #region Validate Object

        /// <summary>
        /// Validate the given object (all properties).
        /// If null, it is treated as valid.
        /// </summary>
        /// <param name="instance">The object to be validated. null is treated as valid.</param>
        /// <param name="results">This is populated with validation errors and warnings.</param>
        public virtual bool TryValidateObject(object instance, ValidationResults results)
        {
            if (instance == null)
                return false;
            else
                return Validator.TryValidateObject(instance, GetContext(instance), results.Results);
        }

        /// <summary>
        /// Throw an exception if the given object has an invalid value.
        /// </summary>
        /// <param name="instance">The object to be validated.</param>
        public virtual void ValidateObject(object instance)
        {
            if (instance != null)
            {
                ValidationResults results = new ValidationResults();
                TryValidateObject(instance, results);
                results.ThrowIfInvalid();
            }
        }

        #endregion

        #region Validate Property

        /// <summary>
        /// Validate a specified property of an object (i.e. test that its current value is valid).
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName">The name of the property to be validated (case-sensitive).</param>
        /// <param name="results">This is populated with validation errors and warnings.</param>
        public virtual bool TryValidateProperty(object instance, string propertyName, ValidationResults results)
        {
            instance.ArgNotNull(nameof(instance));
            return TryValidateProperty(instance, instance.GetType().GetProperty(propertyName), results);
        }

        /// <summary>
        /// Validate a specified property of an object (i.e. test that its current value is valid).
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName">The property to be validated.</param>
        /// <param name="results">This is populated with validation errors and warnings.</param>
        public virtual bool TryValidateProperty(object instance, PropertyInfo property, ValidationResults results)
        {
            instance.ArgNotNull(nameof(instance));
            return TryValidateValue(instance, property.GetValue(instance), property, results);
        }

        /// <summary>
        /// Throw an exception if the given property has an invalid value.
        /// </summary>
        /// <param name="instance">The object on which the property is to be validated.</param>
        /// <param name="propertyName">The name of the member (usually a property) of <paramref name="instance"/> to be validated.</param>
        public virtual void ValidateProperty(ref object instance, string propertyName)
        {
            ValidationResults results = new ValidationResults();
            TryValidateProperty(instance, propertyName, results);
            results.ThrowIfInvalid();
            if (results.Modified)
                instance = results.NewValue;
        }

        #endregion

        #region Validate Value

        /// <summary>
        /// Throw an exception if the given value is invalid for the specified property (i.e. if it would not be valid to assign it to that property).
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <param name="propertyName">The name of the property (of type <typeparamref name="T"/>).</param>
        /// <typeparam name="T">The type that this property is on.</typeparam>
        public virtual void ValidateValueForProperty<T>(ref object value, string propertyName)
        {
            ValidateValue(null, ref value, typeof(T).GetProperty(propertyName));
        }

        /// <summary>
        /// Throw an exception if the given value is invalid for the specified property.
        /// </summary>
        /// <param name="value">The object on which the property is to be validated.</param>
        /// <param name="propertyName">The name of the member (usually a property) of <paramref name="value"/> to be validated.</param>
        public virtual void ValidateValue<TValue>(object instance, ref TValue value, string propertyName)
        {
            instance.ArgNotNull(nameof(instance));
            ValidateValue<TValue>(instance, ref value, instance.GetType().GetProperty(propertyName));
        }

        /// <summary>
        /// Throw an exception if the given value is invalid for the specified property.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <returns>true if a validator modif</returns>
        public virtual bool ValidateValue<TValue>(object instance, ref TValue value, MemberInfo property)
        {
            ValidationResults results = new ValidationResults();
            TryValidateValue(instance, value, property, results);
            results.ThrowIfInvalid();
            return results.Modified;
        }

        /// <summary>
        /// Set a referenced item (<paramref name="targetValue"/>) to the given <paramref name="value"/>,
        /// if it is valid. (If it is not valid, an exception is thrown and <paramref name="targetValue"/> is not set.
        /// <para>This is useful when setting a field in a property setter.</para>
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="targetValue"></param>
        /// <param name="value"></param>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        //| Could be called SetPropertyValue.
        //| Could make this an extension method in another namespace (so that it is seen only in code files that need it).
        //| Could inspect stack to get property name (assuming that this is being called from the setter)? Could set a field located by a naming convention?
        public virtual void SetValue<TValue>(object instance, ref TValue targetValue, TValue value, string propertyName)
        {
            ValidateValue<TValue>(instance, ref value, propertyName);
            // If no exception was thrown, assign it:
            targetValue = value;
        }

        /// <summary>
        /// Validate the given value as a value to be assigned to the given property.
        /// </summary>
        /// <param name="value">The value to be validated (as valid to be assigned to <paramref name="member"/>).</param>
        /// <param name="member">The member (usually a property) to be validated.</param>
        /// <param name="results">This is populated with validation errors and warnings.</param>
        public virtual bool TryValidateValue(object instance, object value, MemberInfo member, ValidationResults results)
        {
            return TryValidateValue(instance, value, member.GetCustomAttributes(true).Cast<ValidationAttribute>(), results, member.Name, CaptionUtil.GetDisplayName(member));
        }

        /// <summary>
        /// Validate a value according to a set of attributes.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <param name="attributes">The attributes providing the validation rules.</param>
        /// <param name="results">This is populated with validation errors and warnings.</param>
        /// <param name="memberName">The name of the member being validated.</param>
        /// <param name="displayName">The display name of the member being validated (it may appear in error messages).</param>
        public virtual bool TryValidateValue(object instance, object value, IEnumerable<ValidationAttribute> attributes, ValidationResults results, string memberName = null, string displayName = null)
        {
            return Validator.TryValidateValue(value, GetContext(instance, memberName, displayName), results.Results, attributes);
        }

        #endregion

        /// <summary>
        /// Creates a <see cref="ValidationContext"/>.
        /// </summary>
        /// <param name="instance">The object to be validated.</param>
        /// <param name="memberName">The name of the member being validated.</param>
        /// <param name="displayName">The display name of the member being validated (it may appear in error messages).</param>
        /// <returns>The context for the given item.</returns>
        protected virtual ValidationContext GetContext(object instance, string memberName = null, string displayName = null)
        {
            var context = new ValidationContext(instance ?? ValidationContextExtension.NoObject);
            if (memberName != null)
                context.MemberName = memberName;
            if (displayName != null)
                context.DisplayName = displayName;
            ValidationContextExtension.SetSupportedFeatures(context,SupportedFeatures);
            return context;
        }

        /// <summary>
        /// Features supported by the system that this validator is used in.
        /// </summary>
        public virtual ValidationFeatures SupportedFeatures { get; protected set; }

        /// <summary>
        /// Instance with default settings.
        /// </summary>
        public static ValidatorEx Instance { get; private set; } = new ValidatorEx(ValidationFeatures.All);

    }

}
