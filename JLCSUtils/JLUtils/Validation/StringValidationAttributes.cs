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
    // Validation/metadata of values represented as strings:

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
        /// <summary>
        /// The maximum length of a valid Eircode, inclding the space after the third character.
        /// </summary>
        public const int MaximumValidEircodeLength = 8;

        public EircodeValidationAttribute()
        {
            MaximumLength = MaximumValidEircodeLength;
        }

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

    //TODO-L: ZipCodeValidationAttribute

    /// <summary>
    /// The data item holds an email address.
    /// </summary>
    public class EmailValidationAttribute : StringValidationAttribute
    {
        /// <summary>
        /// The maximum length of valid email address is 254 (see https://stackoverflow.com/questions/386294/what-is-the-maximum-length-of-a-valid-email-address?rq=1).
        /// This is the default for <see cref="StringValidationAttribute.MaximumLength"/> in this class.
        /// In practice long addresses are rare. See https://stackoverflow.com/questions/1297272/how-long-should-sql-email-fields-be?noredirect=1&amp;lq=1 .
        /// </summary>
        public const int MaximumValidEmailLength = 254;

        public EmailValidationAttribute()
        {
            MaximumLength = MaximumValidEmailLength;
        }

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


    #region Filenames

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

    #endregion


    /// <summary>
    /// The data item holds a URL.
    /// </summary>
    public class UrlValidationAttribute : StringValidationAttribute
    {
        public override string GeneralDescription => "A URL";
    }


    /// <summary>
    /// Specifies that the attributed item identifies a device (such as a hardware peripheral),
    /// usually identified by the unique name of its operating system driver.
    /// </summary>
    public class DeviceNameValidationAttribute : StringValidationAttribute
    {
    }

    /// <summary>
    /// Specifies that the attributed item identifies a printer (or equivalent image output device, e.g. fax machine).
    /// </summary>
    public class PrinterValidationAttribute : DeviceNameValidationAttribute
    {
    }

    /// <summary>
    /// Specifies that the attributed item identifies an image acquisition device (document scanner or equivalent).
    /// </summary>
    public class ScannerValidationAttribute : DeviceNameValidationAttribute
    {
    }


    // The attributed item contains a property name in the format used by <see cref="ReflectionUtil.TryGetPropertyValue{T}(object, string, PropertyNullabilityModifier)"/> etc.
    /// <summary>
    /// <para>
    /// Property name. Can be a nested property.
    /// Each property name in the chain can be suffixed with a symbol to specify nullability - see <see cref="PropertyNullabilityModifier"/>.
    /// The format is (ABNF): *( name [modifier] ".") name [modifier] .
    /// </para>
    /// </summary>
    public class PropertyNameAttribute : ValidationAttributeBase
    {
    }
}
