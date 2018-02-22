using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Text
{
    /// <summary>
    /// Specifies how whitespace in a string is trimmed.
    /// </summary>
    [Flags]
    public enum StringTrimmingOption
    {
        /// <summary>
        /// Don't trim.
        /// </summary>
        None = 0,

        /// <summary>
        /// Trim whitespace at the end of the string.
        /// </summary>
        TrimStart = 1,
        
        /// <summary>
        /// Trim whitespace at the start of the value.
        /// </summary>
        TrimEnd = 2,

        TrimStartAndEnd = TrimStart | TrimEnd,
/*
        /// <summary>
        /// Remove all whitespace in the string.
        /// </summary>
        All = TrimStartAndEnd | 4
*/

        // Additional options:
        //   Trim multiple whitespace characters between words?
        //   Trim all whitespace in a string?
        //   Specify which whitespace characters to trim?
    }

    /// <summary>
    /// Extension methods of <see cref="StringTrimmingOption"/>.
    /// </summary>
    public static class StringTrimmingOptionExtension
    {
        /// <summary>
        /// Apply this option to the given string.
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Trim(this StringTrimmingOption option, string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            switch (option)
            {
                case StringTrimmingOption.TrimStart:
                    return value.TrimStart();
                case StringTrimmingOption.TrimEnd:
                    return value.TrimEnd();
                case StringTrimmingOption.TrimStartAndEnd:
                    return value.Trim();
//                case StringTrimmingOption.All:
//                    return value.RemoveCharacters(" \t");  // Not all (unicode) whitespace characters
                case StringTrimmingOption.None:
                    return value;
                default:
                    Diagnostic.Diagnostics.UnhandledEnum(option);
                    return value;
            }
        }
    }
}
