// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using JohnLambe.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Text
{
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
        /// Characters allowed in identifiers such as those used in code.
        /// </summary>
        public const string IdentifierCharacters = AsciiCapitalLetters + AsciiLowercaseLetters + Digits + "_";

        #endregion

        #region Characters

        /// <summary>
        /// The ASCII ESC (Escape) character.
        /// </summary>
        public const char Asc_ESC = '\x1B';

        #endregion

        /// <summary>
        /// Repeat the character <paramref name="count"/> times.
        /// If <paramref name="count"/> &lt;= 0, "" is returned.
        /// </summary>
        /// <param name="c">The character to repeat.</param>
        /// <param name="count"></param>
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
    }
}
