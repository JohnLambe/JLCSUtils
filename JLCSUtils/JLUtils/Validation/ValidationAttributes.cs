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

namespace JohnLambe.Util.Validation
{
    //TODO: Remove "Validation" from these names, except RegexValidationAttribute.

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)] //TODO: Review
    public abstract class ValidationAttributeBase : System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        /// <summary>
        /// If this is not <see cref="ValidationResultType.Error"/>, then when an item fails validation,
        /// it generates a message of that type.
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
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
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

        protected virtual void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
        }
    }

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
        public virtual LetterCapitalisationOption Capitalisation { get; set; } = LetterCapitalisationOption.MixedCase;

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);
            if (value != null && AllowedCharacters != null)
            {
                if (StrUtil.ContainsOnlyCharacters(value.ToString(), AllowedCharactersSet))
                    results.Add(ErrorMessage ?? "Contains an invalid character");
            }
        }
    }

    /// <summary>
    /// Options for what capitalisation is valid in a string,
    /// or should be applied to it (capitalisation changed to match the pattern).
    /// </summary>
    public enum LetterCapitalisationOption
    {
        /// <summary>
        /// Any combination of capital and lowercase letters is valid.
        /// Don't change/correct capitalisation.
        /// </summary>
        MixedCase = 0,
        /// <summary>
        /// All letters are lowercase.
        /// </summary>
        AllLowercase,
        /// <summary>
        /// All letters are capital.
        /// </summary>
        AllCaptial,
        /// <summary>
        /// The first letter of each word is captial.
        /// Capital letters in other positions are also valid.
        /// </summary>
        TitleCase,
        /// <summary>
        /// The first letter of each word is captial.
        /// All other letters must be lowercase.
        /// </summary>
        TitleCaseOnly,
        /// <summary>
        /// The first letter of the string is capital.
        /// Capital letters in other positions are also valid.
        /// </summary>
        FirstLetterCapital,
        /// <summary>
        /// The first letter of the string is capital.
        /// All other letters must be lowercase.
        /// </summary>
        FirstLetterCapitalOnly

        // These options relate to human-readable text.
        //| Code identifiers also have camelCase, PascalCase, and options relating to underscores.
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

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
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
        public virtual TimePrecision TimeParts { get; set;}

        /// <summary>
        /// Number of decimal places in seconds (for display and entry).
        /// </summary>
        public virtual int SecondsDecimalPlaces { get; set; }
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
