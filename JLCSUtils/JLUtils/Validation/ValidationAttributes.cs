using JohnLambe.Util.Math;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Text;
using JohnLambe.Util.TypeConversion;
using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JohnLambe.Util.TimeUtilities;
using System.IO;
using JohnLambe.Util;
using JohnLambe.Util.Io;
using System.Diagnostics;
using JohnLambe.Util.Diagnostic;

namespace JohnLambe.Util.Validation
{
    // Validation attributes not specific to a type:


    /// <summary>
    /// Specifies a value that is not allowed as a value of the attributed item.
    /// </summary>
    public class InvalidValueAttribute : ValidationAttributeBase
    {
        /// <summary/>
        /// <param name="invalidValue"><see cref="InvalidValue"/></param>
        public InvalidValueAttribute(object invalidValue)
        {
            this.InvalidValue = invalidValue;
        }

        /// <summary>
        /// Value that is not allowed.
        /// <para>This is compared using <see cref="object.Equals(object)"/> (so if it is a string, it is case-sensitive).</para>
        /// </summary>
        public virtual object InvalidValue { get; set; }
        //| We could accept an array (list of invalid values), but multiple value can be defined with multiple attributes.
        //| Could provide a StringComparison to use when the value is a string, or when present, to compare the string representations of InvalidValue and the value being validated.

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);
            if ((value == null && InvalidValue == null)
                || (value != null && value.Equals(InvalidValue)))
            {
                results.Add(ErrorMessage ?? "The value '" + InvalidValue + "' is invalid for " + validationContext.DisplayName);
            }
        }

        public override string DefaultDescription => "Not \"" + InvalidValue + "\"";
    }


    /// <summary>
    /// Specifies a collection of valid values.
    /// </summary>
    public class AllowedValuesAttribute : ValidationAttributeBase
    {
        //TODO: Reference a (possibly dynamic) provider/dataset of values that may be chosen.

        /// <summary>
        /// The list of allowed values.
        /// </summary>
        public virtual object[] Values { get; set; }

        /// <summary>
        /// Iff true, values not in the list are allowed.
        /// The list can be provided to choose from, but other values can be enetered.
        /// </summary>
        public virtual bool AllowOtherValues { get; set; } = false;

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);
            if (!AllowOtherValues && !Values.Contains(value))
                results.Fail();
        }
    }


    /// <summary>
    /// Specifies that the default value for the type is not valid.
    /// </summary>
    public class NonBlankValidationAttribute : ValidationAttributeBase
    {
        /*
        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);
            bool valid = true;
            if (value == null)
                valid = false;
            else
                valid = value == TypeUtil.GetDefaultValue(value.GetType());

            if(!valid)
            {
                results.Add("Blank/default value not allowed");
            }
        }
        */

        public override bool IsDefaultAttribute() => true;

        public override string DefaultDescription => "Not blank";
    }


    /// <summary>
    /// Validates that a value is in a given range, or less than or greater than a given value.
    /// Similar to <see cref="RangeAttribute"/>, but it supports any <see cref="IComparable"/> value.
    /// </summary>
    public class RangeValidation : ValidationAttributeBase
    {
        /// <summary>
        /// </summary>
        /// <param name="minimum"><see cref="Minimum"/></param>
        /// <param name="maximum"><see cref="Maximum"/></param>
        public RangeValidation(object minimum = null, object maximum = null)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        /// <summary>
        /// Value which the value being validated must not be less than.
        /// Must implement <see cref="IComparable"/> and support comparison to the value being validated.
        /// null for no minimum.
        /// </summary>
        public virtual object Minimum
        {
            get { return _minimum; }
            set { _minimum = (IComparable)value; }
        }
        protected IComparable _minimum;

        /// <summary>
        /// Value which the value being validated must not be higher than.
        /// Must implement <see cref="IComparable"/> and support comparison to the value being validated.
        /// null for no maximum.
        /// </summary>
        public virtual object Maximum
        {
            get { return _maximum; }
            set { _maximum = (IComparable)value; }
        }
        protected IComparable _maximum;

        //TODO: Specify whether each property is inclusive ?
        //TODO: Specify comparer ?

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);

            if (Minimum != null && _minimum.CompareTo(value) < 0)
                results.Add(validationContext?.DisplayName + " must be higher than or equal to " + _minimum);
            if (Minimum != null && _maximum.CompareTo(value) > 0)
                results.Add(validationContext?.DisplayName + " must be less than or equal to " + _maximum);
        }
    }


    /// <summary>
    /// Validates that specified properties are unique among the collection elements.
    /// <para>
    /// For placment on a property of type <see cref="ICollection{T}"/>.
    /// </para>
    /// </summary>
    public class CollectionUniqueConstraintValidationAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// The properties of the collection element that must (in combination) be unique.
        /// <para>These properties must be defined on the type that is the generic type parameter of the declared type (the collection type) of the attributed item.
        /// </para>
        /// </summary>
        public virtual string[] Properties { get; set; }

        /*TODO:
        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);

            var collection = value as ICollection<object>;  //TODO get type by reflection
            collection.Distinct( ... );
        }
        */
    }



    //TODO: Attribute to set initial value, or default value to be used when null.
    //   or use DefaultValueAttribute ?
}
