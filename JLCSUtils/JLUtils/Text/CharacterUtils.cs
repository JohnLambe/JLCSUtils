// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util
{
    public static class CharacterUtils
    {
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
        public const string NumericCharacters = "0123456789.,-+";

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
        public const string IdentifierCharacters = AsciiCapitalLetters + AsciiLowercaseLetters + "_";
    }
}
