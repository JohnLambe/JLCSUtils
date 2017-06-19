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

namespace JohnLambe.Util.Validation
{
    //TODO: Remove "Validation" from these names, except RegexValidationAttribute ??


    /// <summary>
    /// For validation of string values.
    /// </summary>
    public class StringValidationAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// All characters allowed in the string. null for all.
        /// </summary>
        public virtual string AllowedCharacters
        {
            get { return _allowedValues?.StringValue; }
            set
            {
                if (value == null)
                    _allowedValues = null;
                else
                    _allowedValues = new StringCharacterSet(value);
            }
        }

        /// <summary>
        /// The set of characters in <see cref="AllowedCharacters"/>.
        /// null if all characters are allowed.
        /// </summary>
        public virtual ISet<char> AllowedCharactersSet
        {
            get { return _allowedValues; }
        }
        protected StringCharacterSet _allowedValues;

        /// <summary>
        /// Specifies how the string should be capitalised.
        /// </summary>
        public virtual LetterCapitalizationOption Capitalisation { get; set; } //= LetterCapitalizationOption.MixedCase;

        /// <summary>
        /// Whether values should have leading and/or trailing space removed.
        /// </summary>
        public virtual StringTrimmingOption Trimming { get; set; }

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);
            if (value != null && AllowedCharacters != null)
            {
                if (StrUtil.ContainsOnlyCharacters(value.ToString(), AllowedCharactersSet))
                    results.Add(ErrorMessage ?? "Contains an invalid character");
            }

            if(value != null && Capitalisation != LetterCapitalizationOption.MixedCase && Capitalisation != LetterCapitalizationOption.Unspecified)
            {
                var stringValue = value.ToString();
                var newValue = Capitalisation.ChangeCapitalization(stringValue);
                if (newValue != stringValue)         // only if the value is changed
                    value = newValue;                // update it (so that the type is preserved if the capitalisation doesn't change)
            }

        }
    }


    /// <summary>
    /// The data item holds a phone number.
    /// </summary>
    /// <remarks><see cref="PhoneAttribute"/> is similar but does not accept a blank value, and is sealed.</remarks>
    public class PhoneNumberValidationAttribute : StringValidationAttribute
    {
        public override string DefaultDescritpion => "A phone number";

        /// <summary>
        /// If true, the number must be in international format, beginning with a "+".
        /// </summary>
        public virtual bool IsInternational
        {
            get { return (bool)_isInternational; }
            set { _isInternational = value; } }
        private bool _isInternational { get; set; }
        public bool? GetIsInternational() =>  _isInternational;

        // Testing for national format, or converting between national, international and local, or validating the national or local part,
        // would require information about the local network.
        // An international standard specifies a maximum of 15 digits after the country code.

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            if (value == null || value.ToString() == "")            // blank or null is valid
                return;   // valid

            if (!ValidatePhoneNumber(value))                    // validate using PhoneAttribute
                results.Fail();

            if(GetIsInternational().HasValue)
            {
                string s = value.ToString();
                bool international = s.StartsWith("+");
                if (international != GetIsInternational())
                {
                    if (international)
                        results.Add("Phone number must not be in international format");
                    else
                        results.Add("Phone number must be in international format");
                }
            }

            base.IsValid(ref value, validationContext, results);
        }

        /// <summary>
        /// Validates a phone number in the same way as <see cref="PhoneAttribute"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ValidatePhoneNumber(object value)
            => _phoneAttribute.IsValid(value);

        /// <summary>
        /// Instance to use for validating values passed to instances of this class.
        /// This class does not change its state.
        /// </summary>
        private static PhoneAttribute _phoneAttribute = new PhoneAttribute();
    }

    /// <summary>
    /// The data item holds a postcode.
    /// </summary>
    public class PostcodeValidationAttribute : StringValidationAttribute
    {
    }

    /// <summary>
    /// Specifies a value that is not allowed as a value of the attributed item.
    /// </summary>
    public class InvalidValueAttribute : ValidationAttributeBase
    {
        public InvalidValueAttribute(object invalidValue)
        {
            this.InvalidValue = invalidValue;
        }

        /// <summary>
        /// Value that is not allowed.
        /// </summary>
        public virtual object InvalidValue { get; set; }

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);
            if((value == null && InvalidValue == null)
                || (value != null && value.Equals(InvalidValue)))
            {
                results.Add("The value " + InvalidValue + " is invalid");
            }
        }
    }

    /// <summary>
    /// The data item holds an email address.
    /// </summary>
    public class EmailValidationAttribute : StringValidationAttribute
    {
        public override string DefaultDescritpion => "An email address";

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            if (value == null || value.ToString() == "")            // blank or null is valid
                return;   // valid

            if (!ValidateEmailAddress(value))                    // validate using EmailAddressAttribute
                results.Fail();

            base.IsValid(ref value, validationContext, results);
        }

        /// <summary>
        /// Validates an email address in the same way as <see cref="EmailAddressAttribute"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ValidateEmailAddress(object value)
            => _emailAddressAttribute.IsValid(value);

        /// <summary>
        /// Instance to use for validating values passed to instances of this class.
        /// This class does not change its state.
        /// </summary>
        private static EmailAddressAttribute _emailAddressAttribute = new EmailAddressAttribute();
    }

    /// <summary>
    /// The data item holds a password.
    /// It is recommended that it is masked/hidden on entering in the user interface.
    /// </summary>
    public class PasswordAttribute : StringValidationAttribute
    {
    }

    /// <summary>
    /// For string values that are validated with a regular expression.
    /// </summary>
    public class RegexValidationAttribute : StringValidationAttribute
    {
        public RegexValidationAttribute(string pattern)
        {
            this.Pattern = pattern;
        }

        /// <summary>
        /// The pattern that the must match.
        /// If this is null, it is treated as always valid.
        /// </summary>
        public virtual string Pattern { get; set; }

        /// <summary>
        /// Options to use when evaluating the regular expression.
        /// </summary>
        public virtual RegexOptions Options { get; set; } = RegexOptions.None;

        /// <summary>
        /// Iff true, a value of "" is allowed, regardless of <see cref="Pattern"/>.
        /// If false, "" is still allowed if <see cref="Pattern"/> supports it.
        /// </summary>
        public virtual bool AllowBlank { get; set; }

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);
            if (!AllowBlank || !value.Equals(""))
                if (Pattern != null && !Regex.IsMatch(value?.ToString() ?? "", Pattern, Options))
                    results.Fail();
        }
    }


    /// <summary>
    /// Flags that the attributed item holds a filename or directory name.
    /// </summary>
    public class FilenameValidationAttribute : StringValidationAttribute
    {
        public virtual FilePathCompleteness PathType { get; set; } = FilePathCompleteness.Any;
        public virtual FileExistsState Exists { get; set; } = FileExistsState.Any;
        public virtual NullableBool HasExtension { get; set; } = NullableBool.Null;
        public virtual NullableBool IsDirectory { get; set; } = NullableBool.Null;
        public virtual bool AllowWildcard { get; set; } = true;

        public virtual string[] Extensions { get; set; }

        public override string DefaultDescritpion => "A file or directory name";
    }

    [Flags]
    public enum FileExistsState
    {
        /// <summary>
        /// No file/directory exists with the given filename/pathname.
        /// </summary>
        MatchNone = 1,

        /// <summary>
        /// The filename/pathname matches exactly one file or directory.
        /// </summary>
        MatchOne = 2,
        
        /// <summary>
        /// The (wildcarded) filename/pathname matches multiple files or directories.
        /// </summary>
        MatchMultiple = 3,

        Exists = MatchOne | MatchMultiple,
        Any = MatchNone | Exists
    }

    [Flags]
    public enum FilePathCompleteness
    {
        /// <summary>
        /// A filename with no directory information.
        /// </summary>
        LeafName = 1,
        /// <summary>
        /// A relative path: A path that does not start from a root.
        /// </summary>
        RelativePath = 2,
        /// <summary>
        /// A full (absolute) pathname, including a root.
        /// </summary>
        FullPath = 4,

        Any = LeafName | RelativePath | FullPath
    }


    /// <summary>
    /// The data item holds a URL.
    /// </summary>
    public class UrlValidationAttribute : StringValidationAttribute
    {
        public override string DefaultDescritpion => "A URL";
    }


    /// <summary>
    /// Metadata or validation information for numberic values.
    /// </summary>
    public class NumberValidationAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// Value of some properties to indicate that the number should not be rounded.
        /// </summary>
        public const int NoRounding = -1;

        /// <summary>
        /// Number of decimal places to show when displaying this value.
        /// <see cref="NoRounding"/> for floating point.
        /// </summary>
        public virtual int DecimalPlaces { get; set; } = NoRounding;

        /// <summary>
        /// Number of decimal places that the value should be rounded to on assignment.
        /// <see cref="NoRounding"/> to not round.
        /// </summary>
        public virtual int RoundTo { get; set; } = NoRounding;

        /// <summary>
        /// Specifies if and how numbers are rounded.
        /// </summary>
        public virtual DigitGroupingOption DigitGrouping { get; set; } = DigitGroupingOption.Default;

        /// <summary>
        /// The lowest valid value.
        /// </summary>
        public virtual double MinimumValue { get; set; } = double.MinValue;
        /// <summary>
        /// The highest valid value.
        /// </summary>
        public virtual double MaximumValue { get; set; } = double.MaxValue;

        protected override void IsValid([Nullable] ref object value, ValidationContext validationContext, ValidationResults results)
        {
            if (value != null)
            {
                if (RoundTo != NoRounding)
                {
                    value = MathUtil.Round(value,RoundTo);
                }
                double numericValue = GeneralTypeConverter.Convert<double>(value);
                if (numericValue < MinimumValue)
                    results.Add("Value too low");
                else if (numericValue > MaximumValue)
                    results.Add("Value too high");
            }
        }
    }

    public enum DigitGroupingOption  // refactor to objects ?
    {
        /// <summary>
        /// Whether and how digits are grouped is determined by a global/general setting or by the locale.
        /// </summary>
        Default = 0,
        /// <summary>
        /// No digit grouping.
        /// </summary>
        None = 1,
        /// <summary>
        /// Digits are grouped, using the normal digit grouping for the locale or application
        /// or determined by a global/general setting.
        /// </summary>
        Normal,

        /// <summary>
        /// Digits are grouped in threes (western convention), e.g. 1,000,000,000,000.
        /// </summary>
        Thousands,
        /// <summary>
        /// Grouped into lakh crore (10,00,00,00,00,000), crore (1,00,00,000) and lakh (1,00,000) (Indian digit grouping, widely used in South Asia).
        /// The least significant three digits are in one group. The rest are in groups of two digits.
        /// </summary>
        CroreLakh,

        Custom1 = 100,
        Custom2,
        Custom3,
        Custom4,

        // See https://en.wikipedia.org/wiki/Decimal_mark#Digit_grouping
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
    /// A currency amount, in the base unit of the currency (e.g. dollar, euro, yuan).
    /// </summary>
    public class CurrencyValidationAttribute : NumberValidationAttribute
    {
        public override string DefaultDescritpion => "A monetary amount";
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
    /// Specifies that a property is date/time value, and provides metadata relating to it.
    /// </summary>
    public class DateTimeAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// The components of the date/time that are present.
        /// </summary>
        public virtual TimePrecision TimeParts { get; set;}

        /// <summary>
        /// Number of decimal places in seconds (for display and entry).
        /// </summary>
        public virtual int SecondsDecimalPlaces { get; set; }

        /// <summary>
        /// What the time part should be populated with when the value represents only a date.
        /// </summary>
        public virtual TimePartOption TimePartOption { get; set; }
        //TODO: Set the time part on validation.

        public virtual TimeValidationOptions Options { get; set; }

        /// <summary>
        /// Minimum (earliest) allowed value.
        /// </summary>
        public virtual DateTime Minimum { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Maximum (latest) allowed value.
        /// </summary>
        public virtual DateTime Maximum { get; set; } = DateTime.MaxValue;

        // Options to add:
        //   Restrict days of week?
        //   Min. and Max. time of day?
        //   Min. and Max. difference to current time (TimeSpans).
        //   Default to current time?

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);
            DateTime timeValue = GeneralTypeConverter.Convert<DateTime>(value);

            if (timeValue < Minimum || timeValue > Maximum)
                results.Add("Date/time is outside the allowed range");  //TODO Show range

            switch (TimePartOption)
            {
                case TimePartOption.EndOfDay:
                    timeValue = timeValue.EndOfDay();
                    value = timeValue;
                    break;
                case TimePartOption.StartOfDay:
                    timeValue = timeValue.Date;
                    value = timeValue;
                    break;
            }
            //TODO other properties
        }
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

    /// <summary>
    /// Specifies what the time part of a datetime value holds.
    /// </summary>
    public enum TimePartOption
    {
        Unspecified = 0,

        /// <summary>
        /// The value represents a date and time.
        /// </summary>
        Normal = 1,
        
        /// <summary>
        /// The value represents a date only.
        /// The time part should be midnight (00:00:00).
        /// </summary>
        StartOfDay,

        /// <summary>
        /// The value represents a date only.
        /// The time part should be latest value representable (~23:59:59.999).
        /// It may be the highest value representable in a system (e.g. a database) that the value is stored in rather than <see cref="DateTime"/>.
        /// <para>
        /// This may be used for dates at the end of a range (or expiry dates or deadlines that include the day itself),
        /// so that comparing a full date/time (less than or greater than) to this value yields the expected result.
        /// </para>
        /// </summary>
        EndOfDay,
    }

    [Flags]
    public enum TimeValidationOptions
    {
        /// <summary>
        /// Time values in the past are allowed.
        /// </summary>
        AllowPast = 1,

        /// <summary>
        /// Time values in the future are allowed.
        /// </summary>
        AllowFuture = 2
    }

}
