using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Text
{
    /// <summary>
    /// Options for what capitalisation is valid in a string,
    /// or should be applied to it (capitalisation changed to match the pattern).
    /// </summary>
    public enum LetterCapitalizationOption
    {
        /// <summary>
        /// The option is not explicitly specified, so in most cases, it should be treated as <see cref="MixedCase"/> (no correction is applied).
        /// </summary>
        Unspecified = 0,
        
        /// <summary>
        /// Any combination of capital and lowercase letters is valid.
        /// Don't change/correct capitalisation.
        /// </summary>
        MixedCase = 1,
        
        /// <summary>
        /// All letters are lowercase.
        /// </summary>
        AllLowercase,
        
        /// <summary>
        /// All letters are capital.
        /// </summary>
        AllCapital,
        
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
    /// Extension methods of <see cref="LetterCapitalizationOption"/>.
    /// </summary>
    public static class LetterCapitalizationOptionExtension
    {
        /// <summary>
        /// Apply this option to the given string.
        /// </summary>
        /// <param name="capitalisation">The capitalisation option to apply.</param>
        /// <param name="value">The value to have its capitalisation changed.</param>
        /// <param name="culture">Culture to use for letter case conversion.</param>
        /// <returns><paramref name="value"/> with its capitalisation changed, if required.</returns>
        public static string ChangeCapitalization(this LetterCapitalizationOption capitalisation, string value,
            CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(value))     // in this case, capitalization changes can't change it. Code below may assume that `value` has at least one character.
                return value;

            if (culture == null)
                culture = CultureInfo.InvariantCulture;

            switch (capitalisation)
            {
                case LetterCapitalizationOption.AllCapital:
                    return value.ToUpper();
                case LetterCapitalizationOption.AllLowercase:
                    return value.ToLower();
                case LetterCapitalizationOption.FirstLetterCapital:
                    return char.ToUpper(value[0],culture) + value.SafeSubstring(1);
                case LetterCapitalizationOption.FirstLetterCapitalOnly:
                    return char.ToUpper(value[0],culture) + value.SafeSubstring(1).ToLower(culture);
                case LetterCapitalizationOption.TitleCaseOnly:
                case LetterCapitalizationOption.TitleCase:
                    StringBuilder newValue = new StringBuilder(value.Length);
                    bool wordStart = true;   // the first character is the start of a word
                    for(int index = 0; index < value.Length; index++)
                    {
                        char current = value[index];

                        if (wordStart)
                            current = char.ToUpper(current,culture);
                        else if(capitalisation == LetterCapitalizationOption.TitleCaseOnly)
                            current = char.ToLower(current,culture);

                        wordStart = char.IsWhiteSpace(current);  // if this is whitespace, the next character is the start of a word (unless it is also whitespace)

                        newValue.Append(current);
                    }
                    return newValue.ToString();
                default:
                    return value;
            }
        }
    }
}
