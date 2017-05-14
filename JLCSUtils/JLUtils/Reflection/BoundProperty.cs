using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Diagnostic;
using JohnLambe.Util.Text;
using JohnLambe.Util.Validation;


namespace JohnLambe.Util.Reflection
{
    /*
    public class BoundProperty
    {
        public static BoundProperty<TTarget, TProperty> Create<TTarget, TProperty>(TTarget target, PropertyInfo property)
        {
            return new BoundProperty<TTarget, TProperty>(target, property);
        }
    }
    */

    /// <summary>
    /// Metadata of a property (of a class, struct, interface, dictionary, etc.),
    /// and the object it is defined on.
    /// </summary>
    /// <typeparam name="TTarget">The type of the object of the property.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    public class BoundProperty<TTarget, TProperty> : ICustomAttributeProvider
    {
        /// <summary>
        /// </summary>
        /// <param name="target">The object on which the property is defined.
        /// </param>
        /// <param name="propertyName">The name of the property. This may be a nested property name (with ".").</param>
//        /// <exception cref="ArgumentException">If the property, <paramref name="propertyName"/>, is not found.</exception>
        //TODO: Throw exception if target==null and property is not static ?
        public BoundProperty(TTarget target, string propertyName)
        {
            object targetObject = target;
            Property = ReflectionUtil.GetProperty(ref targetObject, propertyName);
//                .NotNull("Property not found: " + typeof(TTarget) + "." + propertyName, typeof(ArgumentException));
            this.Target = (TTarget)targetObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">The object on which the property is defined.</param>
        /// <param name="property">The property itself.</param>
        public BoundProperty(TTarget target, PropertyInfo property)
        {
            ObjectExtension.CheckArgNotNull(target, nameof(target));
            property.ArgNotNull(nameof(property));
            Diagnostics.PreCondition(property.DeclaringType.IsAssignableFrom(target.GetType()));
            this.Target = target;
            this.Property = property;
        }

        /// <summary>
        /// True if the property is readable (not write-only).
        /// </summary>
        public virtual bool CanRead
            => Property?.CanRead ?? false;

        /// <summary>
        /// True if the property is settable (not read-only).
        /// </summary>
        public virtual bool CanWrite
            => Property?.CanWrite ?? false;

        /// <summary>
        /// The value of this property on the bound object.
        /// </summary>
        public virtual TProperty Value
        {
            get
            {
                return (TProperty)Property?.GetValue(Target);
            }
            set
            {
                if (Property != null)
                {
                    Validator?.ValidateValue(Target, ref value, Property);

                    Property?.SetValue(Target, value);
                }
            }
        }

        /// <summary>
        /// The name of the property (as used in code).
        /// </summary>
        public virtual string Name
            => Property.Name;

        /// <summary>
        /// A caption or name of the property for display to a user.
        /// </summary>
        public virtual string DisplayName
            => CaptionUtil.GetDisplayName(Property);

        public virtual string Description
            => CaptionUtil.GetDescriptionFromAttribute(Property);

        /// <summary>
        /// The object that declares this property.
        /// If this object was created using a nested property name, this returns the nested object.
        /// </summary>
        public virtual TTarget Target { get; }

        /// <summary>
        /// The Property metadata. This may be null if this object does not represent a class/struct/interface
        /// (e.g. it could be a dictionary or XML file).
        /// </summary>
        public virtual PropertyInfo Property { get; }

        /// <summary>
        /// Validator used to validate the property when set.
        /// </summary>
        protected virtual ValidatorEx Validator { get; set; } = new ValidatorEx(); //TODO: Make injectable

        /// <summary>
        /// Test whether the given value is valid for assignment to the property.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public virtual bool TryValidateValue(TProperty value, ValidationResults results)
        {
            return Validator?.TryValidateValue(Target, value, Property, results)
                ?? true;
        }

        /// <summary>
        /// Validate the given value for assignment to the property, throwing an exception if it is invalid.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>true iff a validator modified the value.</returns>
        /// <exception cref="ValidationException">If invalid.</exception>
        public virtual bool ValidateValue(ref TProperty value)
        {
            return Validator?.ValidateValue(Target, ref value, Property) ?? false;
        }

        #region ICustomAttributeProvider
        // Delegates to Property.

        public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return ((ICustomAttributeProvider)Property).GetCustomAttributes(attributeType, inherit);
        }

        public virtual object[] GetCustomAttributes(bool inherit)
        {
            return ((ICustomAttributeProvider)Property).GetCustomAttributes(inherit);
        }

        public virtual bool IsDefined(Type attributeType, bool inherit)
        {
            return ((ICustomAttributeProvider)Property).IsDefined(attributeType, inherit);
        }

        #endregion

        //| Could, alternatively, subclass PropertyInfo.
        //| Most members would have to delegate to an underlying PropertyInfo (the Reflection calls can return any subclass; PropertInfo is abstract).
        //| That could be broken by changes to PropertyInfo in future .NET framework versions.
    }
}
