using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    //TODO: Remove "Validation" from these names, except RegexValidationAttribute.

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)] //TODO: Review
    public class ValidationAttributeBase : Attribute
    {
    }

    public class StringValidationAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// Characters allowed in the string. null for all.
        /// </summary>
        public virtual string AllowedCharacters { get; set; }
    }

    /// <summary>
    /// The data item holds a phone number.
    /// </summary>
    [Obsolete("Use DataAnnotations")]
    public class PhoneNumberValidationAttribute : StringValidationAttribute
    {
    }

    /// <summary>
    /// The data item holds an email address.
    /// </summary>
    public class EmailValidationAttribute : StringValidationAttribute
    {
    }

    /// <summary>
    /// The data item holds a password.
    /// It is recommended that it is masked/hidden on entering in the user interface.
    /// </summary>
    public class PasswordAttribute : StringValidationAttribute
    {
    }

    public class RegexValidationAttribute : StringValidationAttribute
    {
        public virtual string Regex { get; set; }

        public virtual RegexOptions Options { get; set; } = RegexOptions.None;
    }

    public class NumberValidationAttribute : ValidationAttributeBase
    {
        public virtual int DecimalPlaces { get; set; } = -1;

        public virtual long MinimumValue { get; set; } = int.MinValue;
        public virtual long MaximumValue { get; set; } = int.MaxValue;

        //public virtual bool DigitGrouping { get; set; } ?
    }

    /// <summary>
    /// Holds a percentage value.
    /// e.g. 0.5 means 50%.
    /// <para><see cref="NumberValidationAttribute.MinimumValue"/>, <see cref="NumberValidationAttribute.MaximumValue"/> and <see cref="NumberValidationAttribute.DecimalPlaces"/> are interpreted relating to the stored value.</para>
    /// </summary>
    public class PercentageValidationAttribute : NumberValidationAttribute
    {
    }

    /// <summary>
    /// A currecny amount, in the base unit of the currency (e.g. euro, dollar, yuan).
    /// </summary>
    public class CurrencyValidationAttribute : NumberValidationAttribute
    {
    }

    public class AllowedValuesAttribute : ValidationAttributeBase
    {
        //TODO: Reference a (possibly dynamic) provider/dataset of values that may be chosen.

        public virtual bool AllowFreeText { get; set; }
    }

    public class DateTimeAttribute : ValidationAttributeBase
    {
        public virtual TimePrecision TimeParts { get; set;}

        /// <summary>
        /// Number of decimal places in seconds (for display and entry).
        /// </summary>
        public virtual int DecimalPlaces { get; set; }
    }

    [Flags]
    public enum TimePrecision
    {
        Year = 0x80,
        Month = 0x40,
        Day = 0x20,
        Hour = 0x10,
        Minute = 0x08,
        Second = 0x04,
        SecondsFraction = 0x02,

        Date = Year | Month | Day,
        TimeOfDay = Hour | Minute | Second | SecondsFraction,
        Full = Date | TimeOfDay
    }

}
