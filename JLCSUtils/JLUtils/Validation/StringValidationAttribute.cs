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
    /// <remarks>
    /// See https://stackoverflow.com/questions/20958/list-of-standard-lengths-for-database-fields for a discussion on maximum lengths for various string fields.
    /// </remarks>
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

        /// <summary>
        /// All characters allowed in the string. null for all.
        /// </summary>
        /// <seealso cref="AllowedCharactersString"/>
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


        /// <seealso cref="DisallowedCharactersSet"/>
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

        /// <seealso cref="DisallowedCharactersSet"/>
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
        public virtual char PaddingCharacter { get; set; } = ' ';

        /// <summary>
        /// True if the string may have multiple lines of text.
        /// </summary>
        public virtual NullableBool MultiLine { get; set; } = NullableBool.Null;

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

        /// <summary>
        /// Iff true, the validator modifies the value to make it match the rule where possible.
        /// </summary>
        public virtual bool Correction { get; set; } = true;

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);
            if (value != null)
            {
                var stringValue = value.ToString();

                if (validationContext.GetSupportedFeatures().HasFlag(ValidationFeatures.Modification) && Correction)
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

                if (!(validationContext.GetSupportedFeatures().HasFlag(ValidationFeatures.Modification) && Correction))
                {
                    if (Capitalisation != LetterCapitalizationOption.MixedCase && Capitalisation != LetterCapitalizationOption.Unspecified)
                    {

                    }
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


    /// <summary>
    /// Option for padding strings.
    /// </summary>
    //| Could use StringTrimmingOption (generalised).
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
    /// For string values that are validated with a regular expression.
    /// </summary>
    public class RegexValidationAttribute : StringValidationAttribute
    {
        public RegexValidationAttribute(string pattern)
        {
            this.Pattern = pattern;
        }

        /// <summary>
        /// The pattern that the value must match.
        /// If this is null, it is treated as always valid.
        /// It is treated as valid if this pattern has any match in the value.
        /// To provide a pattern to match the whole value, prefix it with "^" and end it with "$".
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
            if (!AllowBlank || (!value?.Equals("") ?? false))
                if (Pattern != null && !Regex.IsMatch(value?.ToString() ?? "", Pattern, Options))
                    results.Fail();
        }
    }

}
