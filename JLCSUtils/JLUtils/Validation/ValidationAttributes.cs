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
    /// <summary>
    /// For validation of string values.
    /// </summary>
    /// <seealso cref="StringLengthAttribute"/>
    /// <seealso cref="MaxLengthAttribute"/>
    public class StringValidationAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// Value of certain properties (currently just <see cref="MaximumLength"/>), to indicate that no value is defined.
        /// (Must be negative.)
        /// </summary>
        public const int Na = -1;

        public override string GeneralDescription
            => "A text value";

        public override string DefaultDescription
            => (!string.IsNullOrEmpty(AllowedCharactersString) ? "Allowed characters: " + AllowedCharactersString + "; " : "")
                + (!string.IsNullOrEmpty(AllowedCharactersString) ? "Invalid characters: " + DisallowedCharactersString + "; " : "")
                + (Capitalisation != LetterCapitalizationOption.Unspecified ? "Capitalisation: " + EnumUtil.GetDisplayName(Capitalisation) + "; " : "")
                + (MinimumLength > 0 ? "Minimum length: " + MinimumLength + "; " : "")
                + (MaximumLength >= 0 ? "Maximum length: " + MaximumLength + "; " : "")
            ;

        /// <summary>
        /// All characters allowed in the string. null for all.
        /// </summary>
        /// <seealso cref="AllowedCharactersSet"/>
        /// <seealso cref="AllowedCharacters"/>
        /// <seealso cref="DisallowedCharactersString"/>
        public virtual string AllowedCharactersString
        {
            get { return _allowedCharacters?.StringValue; }
            set
            {
                if (value == null)
                    _allowedCharacters = null;
                else
                    _allowedCharacters = new StringCharacterSet(value);
            }
        }

        public virtual char[] AllowedCharacters
        {
            get { return _allowedCharacters.ToArray(); }
            set { _allowedCharacters = new StringCharacterSet(StrUtil.Concat(value)); }
        }

        /// <summary>
        /// The set of characters in <see cref="AllowedCharactersString"/>.
        /// null if all characters (except those in <see cref="DisallowedCharactersSet"/>) are allowed.
        /// </summary>
        /// <seealso cref="AllowedCharacters"/>
        /// <seealso cref="AllowedCharactersString"/>
        public virtual ISet<char> AllowedCharactersSet
        {
            get { return _allowedCharacters; }
        }
        protected StringCharacterSet _allowedCharacters;


        public virtual string DisallowedCharactersString
        {
            get { return _disallowedCharacters?.StringValue; }
            set
            {
                if (value == null)
                    _disallowedCharacters = null;
                else
                    _disallowedCharacters = new StringCharacterSet(value);
            }
        }
        protected StringCharacterSet _disallowedCharacters;

        /// <summary>
        /// Characters not allowed in the value.
        /// If any character appears in both this and <see cref="AllowedCharactersSet"/>, it is disallowed.
        /// </summary>
        /// <seealso cref="DisallowedCharacters"/>
        /// <seealso cref="DisallowedCharactersString"/>
        public virtual ISet<char> DisallowedCharactersSet
        {
            get { return _disallowedCharacters; }
        }

        public virtual char[] DisallowedCharacters
        {
            get { return _allowedCharacters.ToArray(); }
            set { _allowedCharacters = new StringCharacterSet(StrUtil.Concat(value)); }
        }

        /// <summary>
        /// Specifies how the string should be capitalised.
        /// </summary>
        public virtual LetterCapitalizationOption Capitalisation { get; set; } //= LetterCapitalizationOption.MixedCase;

        /// <summary>
        /// Whether values should have leading and/or trailing space removed.
        /// </summary>
        public virtual StringTrimmingOption Trimming { get; set; }

        /// <summary>
        /// The minimum length of the string if it is not blank.
        /// </summary>
        public virtual int MinimumLength { get; set; } = 0;

        /// <summary>
        /// The maximum length of the string (after any trimming).
        /// -1 for no maximum.
        /// </summary>
        /// <remarks>
        /// <see cref="MaxLengthAttribute"/> can also define a maximum length, but it may have other effects with other frameworks,
        /// for example Entity Framework (with Code First) uses it to set the maximum length of a mapped field.
        /// This can be used when such behaviour is not desirable. Otherwise, <see cref="MaxLengthAttribute"/> is recommended.
        /// </remarks>
        public virtual int MaximumLength { get; set; } = Na;

        /// <summary>
        /// When setting, this sets <see cref="MinimumLength"/> and <see cref="MaximumLength"/> to the same value.
        /// When getting, this returns the value of those if they are the same, otherwise it throws <see cref="InvalidOperationException"/>.
        /// <para>In attributes, don't set this if also setting <see cref="MinimumLength"/> or <see cref="MaximumLength"/>.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public virtual int FixedLength
        {
            get
            {
                if (MinimumLength == MaximumLength)
                    return MinimumLength;
                else
                    throw new InvalidOperationException("String length is not fixed");
            }
            set
            {
                MinimumLength = value;
                MaximumLength = value;
            }
        }

        /// <summary>
        /// Iff true, when the value is longer than <see cref="MaximumLength"/>, it is silently truncated to it.
        /// Otherwise, a validation error is given and the value is unchanged.
        /// </summary>
        public virtual bool Truncate { get; set; } = false;

        /// <summary>
        /// If not <see cref="PaddingType.None"/>, the string is padded if it is less than the <see cref="MinimumLength"/>.
        /// </summary>
        public virtual PaddingType Padding { get; set; } = PaddingType.None;
        /// <summary>
        /// The character to use for padding if enabled by <see cref="Padding"/>.
        /// </summary>
        public virtual char PaddingCharacter { get; set; }

        /// <summary>
        /// True if the string may have multiple lines of text.
        /// </summary>
        public virtual NullableBool MultiLine { get; set; }

        //TODO: Conversion between null and "":
        // NullToBlank
        // BlankToNull

        /// <summary>
        /// If not null, line separators in value are changed to this.
        /// </summary>
        public virtual string LineSeparator { get; set; }

        /// <summary>
        /// Value that the string must begin with (case-sensitive).
        /// </summary>
        public virtual string Prefix { get; set; }

        /// <summary>
        /// Value that the string must end with (case-sensitive).
        /// </summary>
        public virtual string Suffix { get; set; }
        //TODO: Case-sensitivity.

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);
            if (value != null)
            {
                var stringValue = value.ToString();

                if (validationContext.GetSupportedFeatures().HasFlag(ValidationFeatures.Modification))
                {
                    // Do trimming first (other operations apply to the trimmed value):
                    switch (Trimming)
                    {
                        case StringTrimmingOption.TrimStart:
                            stringValue = stringValue.TrimStart();
                            break;
                        case StringTrimmingOption.TrimEnd:
                            stringValue = stringValue.TrimEnd();
                            break;
                        case StringTrimmingOption.TrimStartAndEnd:
                            stringValue = stringValue.Trim();
                            break;
                        case StringTrimmingOption.None:
                            break;
                        default:
                            Diagnostics.UnhandledEnum(Trimming);
                            break;
                    }

                    // Next, truncate (more efficient to truncate early) or pad (some operations (e.g. validating length) cannot be done before these):
                    if (stringValue.Length < MinimumLength)
                    {
                        switch (Padding)
                        {
                            case PaddingType.Leading:
                                stringValue.PadLeft(MinimumLength, PaddingCharacter);
                                break;
                            case PaddingType.Trailing:
                                stringValue.PadRight(MinimumLength, PaddingCharacter);
                                break;
                            case PaddingType.None:
                                break;
                            default:
                                Diagnostics.UnhandledEnum(Padding);
                                break;
                        }
                    }
                    else if (Truncate && MaximumLength >= 0)
                    {
                        stringValue = stringValue.Truncate(MaximumLength);
                    }

                    // Capitalisation:

                    if (Capitalisation != LetterCapitalizationOption.MixedCase && Capitalisation != LetterCapitalizationOption.Unspecified)
                    {
                        stringValue = Capitalisation.ChangeCapitalization(stringValue);
                        //                    if (newValue != stringValue)         // only if the value is changed
                        //                        value = newValue;                // update it (so that the type is preserved if the capitalisation doesn't change)
                    }

                    // Line separator:

                    if (LineSeparator != null)
                        stringValue = stringValue.ReplaceLineSeparator(LineSeparator);
                }

                // Validate character set:

                if (AllowedCharactersString != null)
                {
                    if (!StrUtil.ContainsOnlyCharacters(stringValue.ToString(), AllowedCharactersSet))
                        results.Add(ErrorMessage ?? "Contains an invalid character: Only the following characters are allowed: " + AllowedCharactersString);
                }
                if (DisallowedCharactersString != null)
                {
                    if (StrUtil.ContainsAnyCharacters(stringValue.ToString(), DisallowedCharactersSet))
                        results.Add(ErrorMessage ?? "Contains an invalid character: The following characters are invalid: " + DisallowedCharactersString);
                }

                // Validate length:

                if (stringValue.ToString().Length < MinimumLength)
                    results.Add(ErrorMessage ?? "Too short");
                if (MaximumLength >= 0 && stringValue.ToString().Length > MaximumLength)
                    results.Add(ErrorMessage ?? "Too long");

                if (!string.IsNullOrEmpty(Prefix) && !stringValue.StartsWith(Prefix))
                    results.Add(ErrorMessage ?? "Must begin with " + Prefix);
                if (!string.IsNullOrEmpty(Prefix) && !stringValue.EndsWith(Suffix))
                    results.Add(ErrorMessage ?? "Must end with " + Suffix);

                if (!value.Equals(stringValue))         // only if the value is changed
                    value = stringValue;                // update it (so that the type is preserved if the value doesn't have to change)
            }
        }
    }


    public enum PaddingType
    {
        /// <summary> No padding. </summary>
        None = 0,
        /// <summary> Padding at the start of the value. </summary>
        Leading = 1,
        /// <summary> Padding at the end of the value. </summary>
        Trailing = 2
    }


    /// <summary>
    /// The data item holds a phone number.
    /// </summary>
    /// <remarks>
    /// <para><see cref="PhoneAttribute"/> is similar but does not accept a blank value, and is sealed.</para>
    /// <para>
    /// Maximum length: This attribute does not have a default maximum length.
    /// The ITU specifies the maximum length of a phone number (not including the country code) as 15 digits,
    /// but a phone number field can contain separators or formatting characters (e.g. space and brackets),
    /// and possibly an extension number.
    /// It is recommended to specify a maximum length with a <seealso cref="MaxLengthAttribute"/>. (See the remarks on <see cref="StringValidationAttribute.MaximumLength"/>).
    /// </para>
    /// </remarks>
    public class PhoneNumberValidationAttribute : StringValidationAttribute
    {
        public override string GeneralDescription => "A phone number";

        /// <summary>
        /// If true, the number must be in international format, beginning with a "+".
        /// <para>This property sets the value read by <see cref="GetIsInternational"/>.</para>
        /// </summary>
        public virtual bool IsInternational
        {   // This bool (not bool?) because attributes cannot set nullable primitive types.
            get { return (bool)_isInternational; }
            set { _isInternational = value; }
        }
        private bool? _isInternational { get; set; }

        /// <summary>
        /// If true, the number must be in international format, beginning with a "+".
        /// If false, it must not be.
        /// If null, either is allowed.
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="IsInternational"/>
        public virtual bool? GetIsInternational() => _isInternational;

        // Testing for national format, or converting between national, international and local, or validating the national or local part,
        // would require information about the local network.
        // An international standard specifies a maximum of 15 digits after the country code.

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            if (value == null || value.ToString() == "")            // blank or null is valid
                return;   // valid

            if (!ValidatePhoneNumber(value))                    // validate using PhoneAttribute
                results.Fail();

            if (GetIsInternational().HasValue)
            {
                string s = value.ToString();
                bool international = PhoneNumberIsInternational(s);
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

        /// <summary>
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>true iff the given phone number is in international format.</returns>
        public static bool PhoneNumberIsInternational(string phoneNumber)
        {
            return phoneNumber.Trim().StartsWith("+");
        }

    }

    /// <summary>
    /// The data item holds a postcode.
    /// </summary>
    public class PostcodeValidationAttribute : StringValidationAttribute
    {
    }

    /// <summary>
    /// The data item holds an eircode (Irish postcode).
    /// <para>The format of an eircode is (ANBF):<br/>
    /// Alphanumeric = ( %x41-5A / DIGIT ) <br/>
    /// Eircode = 3 Alphanumeric SP 4 Alphanumeric
    /// </para>
    /// </summary>
    public class EircodeValidationAttribute : PostcodeValidationAttribute
    {
        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);
            if (value != null)
            {
                string stringValue = value.ToString();
                if (!ValidateEircode(ref stringValue))
                {
                    results.Fail();
                }
                else
                {
                    if (!stringValue.Equals(value))   // if modified
                    {
                        if (validationContext.GetSupportedFeatures().HasFlag(ValidationFeatures.Modification))  // if modification is supported
                            value = stringValue;
                        else
                            results.Fail();    // modification was required but not supported
                    }
                }
            }
        }

        /// <summary>
        /// Validate an eircode (Irish postcode).
        /// </summary>
        /// <param name="value">Eircode to be validated.
        /// May be modified to correct formatting: Leading and trailing space is removed, and a space is added after the third character if there isn't one already. Letters are capitalised.
        /// </param>
        /// <returns>true iff valid (including if modification was required).</returns>
        public static bool ValidateEircode(ref string value)
        {
            string newValue = value.Trim().ToUpper();
            if (newValue.ToString().Length == 7)
            {
                newValue = newValue.Substring(0, 3) + ' ' + newValue.Substring(3);
            }
            bool valid = newValue.Length == 8
                && newValue[3] == ' '
                && (newValue.Substring(0, 3) + newValue.Substring(4)).ContainsOnlyCharacters(CharacterUtil.AsciiAlphanumericCharacters);
            if (valid)
                value = newValue;
            return valid;
        }

        public override string DefaultDescription => "An Eircode (Irish postcode)";
    }

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
                results.Add("The value " + InvalidValue + " is invalid");
            }
        }

        public override string DefaultDescription => "Not \"" + InvalidValue + "\"";
    }

    /// <summary>
    /// The data item holds an email address.
    /// </summary>
    public class EmailValidationAttribute : StringValidationAttribute
    {
        public override string GeneralDescription => "An email address";

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
        public override string GeneralDescription => "A password";
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
        public FilenameValidationAttribute()
        {
            DisallowedCharacters = Path.GetInvalidPathChars();
        }

        public virtual FilePathCompleteness PathType { get; set; } = FilePathCompleteness.Any;
        /// <summary>
        /// Whether the file or directory must exist.
        /// </summary>
        public virtual FileExistsState Exists { get; set; } = FileExistsState.Any;
        /// <summary>
        /// Whether the pathname can or must have an extension.
        /// Null to allow it with or without an extension.
        /// </summary>
        public virtual NullableBool HasExtension { get; set; } = NullableBool.Null;
        /// <summary>
        /// Whether the pathname must be a directory or must be a file.
        /// Null to allow either.
        /// </summary>
        public virtual NullableBool IsDirectory { get; set; } = NullableBool.Null;
        /// <summary>
        /// Whether wildcards are allowed.
        /// True: There must be a wildcard.
        /// False: Wildcards are not allowed.
        /// Null: Wildcards are allowed but not required.
        /// </summary>
        public virtual NullableBool Wildcard { get; set; } = NullableBool.Null;

        /// <summary>
        /// If not null, the filename must have one of the given extensions.
        /// <see cref="DefaultExtension"/> (if not null) is also allowed even if it is not included in this.
        /// </summary>
        public virtual string[] AllowedExtensions { get; set; }

        /// <summary>
        /// The extensions to be shown in a dialog for choosing the file.
        /// If null, <see cref="AllowedExtensions"/> is used.
        /// </summary>
        /// <seealso cref="ExtensionsToShow"/>
        public virtual string[] Extensions { get; set; }

        /// <summary>
        /// The extensions to be shown in a dialog for choosing the file.
        /// </summary>
        /// <seealso cref="Extensions"/>
        public virtual string[] ExtensionsToShow => Extensions ?? AllowedExtensions;

        /// <summary>
        /// Extension to be added if none is given.
        /// </summary>
        /// <seealso cref="AllowedExtensions"/>
        public virtual string DefaultExtension { get; set; }

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);
            if (value != null)
            {
                string stringValue = value.ToString();
                string extension = Path.GetExtension(stringValue);

                if (string.IsNullOrEmpty(extension) && !string.IsNullOrEmpty(DefaultExtension))
                {
                    extension = DefaultExtension;
                    value = Path.ChangeExtension(stringValue, DefaultExtension);
                }

                if (!HasExtension.NullableCompare(!string.IsNullOrEmpty(extension)))
                {
                    if (HasExtension == NullableBool.True)
                        results.Add("A file extension is required");
                    else
                        results.Add("The filename must not have an extension");
                }

                if (!string.IsNullOrEmpty(extension) && AllowedExtensions != null)
                {
                    if (!AllowedExtensions.Contains(extension, StringComparer.InvariantCultureIgnoreCase) && !extension.Equals(DefaultExtension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        results.Add("The extension (" + extension + ") is not allowed");
                    }
                }

                if (!Wildcard.NullableCompare(PathUtil.HasWildcard(stringValue)))
                {
                    if (Wildcard == NullableBool.True)
                        results.Add("A wildcard is required");
                    else
                        results.Add("Wildcards are not allowed");
                }

                if (validationContext.GetState().HasFlag(ValidationState.LiveInput))
                {
                    if (Exists != FileExistsState.Any)
                    {
                        //TODO
                    }
                }

                //TODO

            }
        }

        public override string GeneralDescription => "A file or directory name";
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
        /// A relative path: A path that includes a directory but does not start from a root.
        /// </summary>
        RelativePath = 2,
        /// <summary>
        /// A full (absolute) pathname, including a root.
        /// <para><see cref="Path.IsPathRooted(string)"/> is true in this case.</para>
        /// </summary>
        FullPath = 4,

        Any = LeafName | RelativePath | FullPath
    }


    /// <summary>
    /// The data item holds a URL.
    /// </summary>
    public class UrlValidationAttribute : StringValidationAttribute
    {
        public override string GeneralDescription => "A URL";
    }


    /// <summary>
    /// Metadata or validation and display information for numeric values.
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

        /// <summary>
        /// Iff true, out of range values are replaced with the closest in-range value.
        /// </summary>
        public virtual bool AdjustToRange { get; set; } = false;

        protected override void IsValid([Nullable] ref object value, ValidationContext validationContext, ValidationResults results)
        {
            if (value != null)
            {
                if (RoundTo != NoRounding)
                {
                    value = MathUtil.Round(value, RoundTo);
                }
                double numericValue = GeneralTypeConverter.Convert<double>(value);
                if (numericValue < MinimumValue)
                {
                    if (AdjustToRange)
                        value = MinimumValue;
                    else
                        results.Add("Value too low");
                }
                else if (numericValue > MaximumValue)
                {
                    if (AdjustToRange)
                        value = MaximumValue;
                    else
                        results.Add("Value too high");
                }
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
        public override string GeneralDescription => "A percentage value";

        public override void PreProcessForDisplay(bool toDisplay, [Nullable] ref object value, [Nullable] ValidationContext validationContext)
        {
            if (toDisplay)
            {
                value = GeneralTypeConverter.Convert<decimal>(value) * 100;
            }
            else
            {
                value = GeneralTypeConverter.Convert<decimal>(value) / 100m;
            }
        }
    }

    /// <summary>
    /// A currency amount, in the base unit of the currency (e.g. dollar, euro, yuan).
    /// </summary>
    public class CurrencyValidationAttribute : NumberValidationAttribute
    {
        public override string GeneralDescription => "A monetary amount";
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


    public abstract class TimeOrTimeSpanValidationAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// The components of the date/time that are present.
        /// </summary>
        public virtual TimePrecision TimeParts { get; set; }

        /// <summary>
        /// Number of decimal places in seconds (for display and entry).
        /// </summary>
        public virtual int SecondsDecimalPlaces { get; set; }

    }

    /// <summary>
    /// Specifies that a property is a date and/or time value, and provides metadata relating to it.
    /// It may be a time of day, but not a time interval.
    /// </summary>
    public class DateTimeValidationAttribute : ValidationAttributeBase
    {
        public override string GeneralDescription => "A date and/or time.";

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

            if (validationContext.GetState().HasFlag(ValidationState.LiveInput) && Options != TimeValidationOptions.Any)
            {
                var now = DateTime.Now;  //TODO use ITimeService
                if (timeValue < now && !(Options.HasFlag(TimeValidationOptions.AllowPast)))
                {
                    results.Add("Must not be in the past");
                }
                else if (timeValue > now && !(Options.HasFlag(TimeValidationOptions.AllowFuture)))
                {
                    results.Add("Must not be in the future");
                }
            }

            //TODO other properties
        }
    }

    [Obsolete("Use DateTimeValidationAttribute")]
    public class DateTimeAttribute : DateTimeValidationAttribute
    {
    }

    /// <summary>
    /// Indicates that the data item is a time interval (duration).
    /// <para>
    /// This can be used on <see cref="TimeSpan"/> and numeric types.
    /// </para>
    /// </summary>
    public class TimeSpanValidationAttribute : ValidationAttributeBase
    {
        public override string GeneralDescription => "A time interval.";

        public virtual TimeSpan Multiplier { get; set; }

        /// <summary>
        /// Minimum allowed value.
        /// </summary>
        public virtual TimeSpan Minimum { get; set; } = TimeSpan.MinValue;

        /// <summary>
        /// Maximum allowed value.
        /// </summary>
        public virtual TimeSpan Maximum { get; set; } = TimeSpan.MaxValue;

        //TODO

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
        AllowFuture = 2,

        Any = AllowPast | AllowFuture
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
        /// Must implement <see cref="IComparable"/>.
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
        /// Must implement <see cref="IComparable"/>.
        /// null for no maximum.
        /// </summary>
        public virtual object Maximum
        {
            get { return _maximum; }
            set { _maximum = (IComparable)value; }
        }
        protected IComparable _maximum;

        //TODO: Specify whether each property is inclusive ?

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);

            if (Minimum != null && _minimum.CompareTo(value) < 0)
                results.Add("must be higher than or equal to " + _minimum);
            if (Minimum != null && _maximum.CompareTo(value) > 0)
                results.Add("must be less than or equal to " + _maximum);
        }

    }
}
