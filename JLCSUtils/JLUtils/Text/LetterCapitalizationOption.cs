using System;
using System.Collections.Generic;
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
        /// <param name="capitalisation"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ChangeCapitalization(this LetterCapitalizationOption capitalisation, string value)
        {
            if (string.IsNullOrEmpty(value))     // in this case, capitalization changes can't change it. Code below may assume that `value` has at least one character.
                return value;

            switch (capitalisation)
            {
                case LetterCapitalizationOption.AllCapital:
                    return value.ToUpper();
                case LetterCapitalizationOption.AllLowercase:
                    return value.ToLower();
                case LetterCapitalizationOption.FirstLetterCapital:
                    return char.ToUpper(value[0]) + value.Substring(1);   // we checked that it has at least character above
                case LetterCapitalizationOption.FirstLetterCapitalOnly:
                    return char.ToUpper(value[0]) + value.Substring(1).ToLower();   // we checked that it has at least character above
                                                                                    /*TODO
                                                                                                    case LetterCapitalizationOption.TitleCaseOnly:
                                                                                                        break;
                                                                                                    case LetterCapitalizationOption.TitleCase:
                                                                                                        break;
                                                                                                        */
                default:
                    return value;
            }
        }
    }
}
