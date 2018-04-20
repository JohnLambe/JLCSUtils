using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Validation
{
    /// <summary>
    /// Can specify restrictions on a <see cref="Type"/> value.
    /// If placed on a type other than <see cref="Type"/>, its string value (<see cref="object.ToString()"/>) must be a type name.
    /// </summary>
    public class TypeValidationAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// Whether interfaces are allowed or required.
        /// </summary>
        public virtual bool IsInterface
        {
            get { return (bool)IsInterfaceValue; }
            set { IsInterfaceValue = value; }
        }
        public virtual bool? IsInterfaceValue { get; protected set; }

        /// <summary>
        /// Whether classes are allowed or required.
        /// </summary>
        public virtual bool IsClass
        {
            get { return (bool)IsClassValue; }
            set { IsClassValue = value; }
        }
        public virtual bool? IsClassValue { get; protected set; }

        /// <summary>
        /// Whether primitive types are allowed or required.
        /// </summary>
        public virtual bool IsPrimitive
        {
            get { return (bool)IsPrimitiveValue; }
            set { IsPrimitiveValue = value; }
        }
        public virtual bool? IsPrimitiveValue { get; protected set; }

        /// <summary>
        /// Whether enum types are allowed or required.
        /// </summary>
        public virtual bool IsEnum
        {
            get { return (bool)IsEnumValue; }
            set { IsEnumValue = value; }
        }
        public virtual bool? IsEnumValue { get; protected set; }

        /// <summary>
        /// Whether value types are allowed or required.
        /// </summary>
        public virtual bool IsValueType
        {
            get { return (bool)IsValueTypeValue; }
            set { IsValueTypeValue = value; }
        }
        public virtual bool? IsValueTypeValue { get; protected set; }

        /// <summary>
        /// Whether abstract types are allowed or required.
        /// </summary>
        public virtual bool IsAbstract
        {
            get { return (bool)IsAbstractValue; }
            set { IsAbstractValue = value; }
        }
        public virtual bool? IsAbstractValue { get; protected set; }

        /// <summary>
        /// Whether numeric primitive types are allowed or required.
        /// </summary>
        public virtual bool IsNumericType
        {
            get { return (bool)IsNumericTypeValue; }
            set { IsNumericTypeValue = value; }
        }
        public virtual bool? IsNumericTypeValue { get; protected set; }


        /// <summary>
        /// Iff not null, the Type must be assignable from the given type (i.e. {instance of attributed type} = {instance of AssignableFrom} is valid).
        /// </summary>
        public virtual Type AssignableFrom { get; set; }

        /// <summary>
        /// Iff not null, the Type must be assignable to the given type (i.e. {instance of AssignableFrom} =  {instance of attributed type} is valid).
        /// </summary>
        public virtual Type AssignableTo { get; set; }

        /// <summary>
        /// Iff not null, the Type item must implement this interface.
        /// </summary>
        public virtual Type Implements { get; set; }

        /// <summary>
        /// Iff not null, the Type must be a subclass of this one (and not this class itself).
        /// </summary>
        public virtual Type SubclassOf { get; set; }

        /// <summary>
        /// Iff not null, the Type must be a subclass of this one or this type itself.
        /// </summary>
        public virtual Type TypeOrSubclassOf { get; set; }

        /// <summary>
        /// If not null, the Type must have an attribute of the given type defined on it.
        /// </summary>
        [TypeValidation(TypeOrSubclassOf = typeof(Attribute))]
        public virtual Type HasAttribute { get; set; }

        /// <summary>
        /// If not null, the type must be in or under the given namespace.
        /// </summary>
        public virtual string Namespace { get; set; }
        //TODO: Allow string value that is a partial class name within this namespace.
        //TODO: Support list of allowed namespaces?

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);

            if (value != null)      // null is valid (a NotNullAttribute can be used in combination with this if a non-null Type is required)
            {
                Type typeValue = value as Type;

                if (typeValue == null)
                    typeValue = Type.GetType(value.ToString());

                bool valid =
                    IsInterfaceValue.CompareNonNull(typeValue.IsInterface)
                    && IsClassValue.CompareNonNull(typeValue.IsClass)
                    && IsAbstractValue.CompareNonNull(typeValue.IsAbstract)
                    && IsEnumValue.CompareNonNull(typeValue.IsEnum)
                    && IsPrimitiveValue.CompareNonNull(typeValue.IsPrimitive)
                    && IsValueTypeValue.CompareNonNull(typeValue.IsValueType)
                    && IsNumericTypeValue.CompareNonNull(typeValue.IsNumericType())

                    && (HasAttribute == null || typeValue.IsDefined(HasAttribute))
                    && (Implements == null || typeValue.Implements(Implements))
                    && (AssignableFrom == null || typeValue.IsAssignableFrom(AssignableFrom))
                    && (AssignableTo == null || AssignableTo.IsAssignableFrom(typeValue))

                    && (SubclassOf == null || typeValue.IsSubclassOf(SubclassOf))
                    && (TypeOrSubclassOf == null || typeValue.IsTypeOrSubclassOf(TypeOrSubclassOf))

                    && (Namespace == null || typeValue.Namespace == Namespace)  //TODO: Support sub-namespaces
                ;

                if (!valid)
                {
                    results.Add(ErrorMessage ?? "The given type (" + typeValue.FullName + ") is not valid" 
                        + (validationContext?.DisplayName != null ? " for " + validationContext?.DisplayName : "" ) );
                }
            }
        }
    }

}
