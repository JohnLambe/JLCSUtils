using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)] //TODO: Review
    public class ValidationAttributeBase : Attribute
    {
    }

    public class StringValidationAttribute : ValidationAttributeBase
    {
    }

    /// <summary>
    /// The data item holds a phone number.
    /// </summary>
    public class PhoneNumberValidationAttribute : StringValidationAttribute
    {
    }

    /// <summary>
    /// The data item holds an email address.
    /// </summary>
    public class EmailValidationAttribute : StringValidationAttribute
    {
    }

    public class PasswordAttribute : StringValidationAttribute
    {
    }

    public class RegexValidationAttribute : StringValidationAttribute
    {
        public virtual string Regex { get; set; }
        //TODO: Regex Options ?
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

    public class CurrencyValidationAttribute : NumberValidationAttribute
    {
    }

    public class AllowedValuesAttribute : ValidationAttributeBase
    {
        //TODO: Reference a (possibly dynamic) provider/dataset of values that may be chosen.

        public virtual bool AllowFreeText { get; set;}
    }


}
