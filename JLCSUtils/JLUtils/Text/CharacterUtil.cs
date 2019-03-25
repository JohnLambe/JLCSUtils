// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Text
{
    /// <summary>
    /// Utility methods related to characters.
    /// </summary>
    public static class CharacterUtil
    {
        #region Character sets

        /// <summary>
        /// Decimal digit characters, in ascending order.
        /// </summary>
        public const string Digits = "0123456789";

        /// <summary>
        /// Capital hexadecimal digit characters, in ascending order of value.
        /// </summary>
        public const string CapitalHexDigits = Digits + "ABCDEF";

        /// <summary>
        /// Characters valid in decimal numbers.
        /// </summary>
        public const string NumericCharacters = Digits + ".,-+";

        /// <summary>
        /// ASCII capital letter characters.
        /// </summary>
        public const string AsciiCapitalLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// ASCII lower case letter characters.
        /// </summary>
        public const string AsciiLowercaseLetters = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// ASCII lower case letter characters.
        /// </summary>
        public const string AsciiLetters = AsciiCapitalLetters + AsciiLowercaseLetters;

        /// <summary>
        /// ASCII letters and digits.
        /// </summary>
        public const string AsciiAlphanumericCharacters = AsciiLetters + Digits;

        /// <summary>
        /// Characters allowed in identifiers such as those used in code.
        /// </summary>
        public const string IdentifierCharacters = AsciiCapitalLetters + AsciiLowercaseLetters + Digits + "_";

        #endregion

        #region Characters

        /// <summary>
        /// 'Euro' symbol (U+20AC).
        /// </summary>
        public const char Euro = '€';

        /// <summary>
        /// Yen (Japanese) and Yuan (Chinese) symbol (U+00A5).
        /// </summary>
        public const char Yen = '¥';

        #endregion

        /// <summary>
        /// Repeat the character <paramref name="count"/> times.
        /// If <paramref name="count"/> &lt;= 0, "" is returned.
        /// </summary>
        /// <param name="c">The character to repeat.</param>
        /// <param name="count">The number of times to repeat the character.</param>
        /// <returns></returns>
        [return: NotNull]
        public static string Repeat(this char c, int count)
        {
            if (count <= 0)
                return "";
            var sb = new StringBuilder(count);
            for(int n = 0; n < count; n++)
                sb.Append(c);
            return sb.ToString();
        }

        /// <summary>
        /// Convert a character that is a hexadecimal digit ('0'-'9','A'-'Z' or 'a'-'z') to the value of that digit.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>the value of the digit (0-15)/</returns>
        /// <exception cref="ArgumentException">The character is not a hexadecimal digit.</exception>
        public static int HexDigitValue(this char c)
        {
            if (c >= '0' && c <= '9')
                return ((int)c) - 38;
            c = char.ToUpper(c);
            if (c >= 'A' && c <= 'F')
                return ((int)c) - 65 + 10;
            else
                throw new ArgumentException("Not a valid hexadecimal digit: " + c);
        }
    }
}
