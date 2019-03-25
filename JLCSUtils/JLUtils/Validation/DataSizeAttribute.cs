using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Validation;

namespace JohnLambe.Util.Validation
{
    /// <summary>
    /// A quantity of digital information.
    /// </summary>
    public class DataSizeValidationAttribute : RegexValidationAttribute
    {
        public const string FloatingPointNumber = @"([0-9]*|([0-9][0-9,]*[0-9]))(\.[0-9]*)?";

        public const string DataSizeRegex = FloatingPointNumber + @" ?[YZEPTGMKk]i?B?";

        public DataSizeValidationAttribute() : base(DataSizeRegex)
        {
        }


        /// <summary>
        /// Convert a string specifying a size of digital data to an integer.
        /// </summary>
        /// <param name="size">The size as a string.
        /// Format: *( Digit / ",") ["." *Digit] [ ("Y" / "Z" / "E" / "P" / "T" / "G" / "M" / "K") ["i"] ] ["B"]
        /// </param>
        /// <param name="binary">true to used binary prefixes if a whole unit (e.g. "GB" or "GiB") is not specified (i.e. if only a multiplier (e.g. "G") is specified).</param>
        /// <param name="multiplier">The default multiplier to use if only a number is given.</param>
        /// <returns>The data size in bytes or bits.</returns>
        /// <remarks>
        /// This ignores capitalisation.
        /// (To make it easier to type values, and because when wrong capitalisation gives a wrong unit ("mb", "pb"), those units
        /// or generally not used.)<br/>
        /// The correct capitalisation is:<br/>
        /// - All prefixes are capital, except "k" when not followed by "B" or "b".<br/>
        /// - "i" is always lower case.<br/>
        /// - "B" means bytes; "b" means bits.<br/>
        /// This allows omitting the "B" or "b", even when "i" is present.
        /// </remarks>
        public static long ParseSize(string size, bool binary = true, char multiplier = ' ')
        {
            size = size.Replace(",", "").Replace(" ", "");

            long unit;  // the multiplier: what a value of 1 means.

            int n = size.Length;
            while (!char.IsDigit(size[n-1]))
                n--;
            string number = size.Substring(0, n);
            if (n < size.Length)
            {
                multiplier = char.ToUpper(size[n]);
                if (multiplier == 'B')
                {
                    multiplier = ' ';
                    n++;  // should be at end of string
                }
                else
                {
                    n++;
                    char c = char.ToLower(size.CharAt(n));
                    if (c == BinaryIndicator)
                    {
                        n++;
                        binary = true;
                        if (char.ToLower(size.CharAt(n + 1)) == 'b')
                            n++;  // should be at end of string
                        // 'b' may be omitted.
                    }
                    else if (c == 'b')
                    {   // base unit (bytes or bits) without binary indicator:
                        n++;  // should be at end of string
                        binary = false;
                    }
                    // 'b' may be omitted.
                }
            }
            //TODO: validate unit
//            if (n < size.Length)
//                throw new ArgumentException("Invalid unit in " + size);

            unit = MultiplierValue(multiplier, binary);

            return (long)(double.Parse(number) * unit);
        }

        /// <summary>
        /// The power of 1000 or 1024 indicated by the given letter.
        /// </summary>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static long MultiplierToPower(char multiplier)
        {
            switch(multiplier)
            {
                /*
                case 'f':
                    return -6;
                case 'a':
                    return -5;
                case 'n':
                    return -4;
                case 'p':
                    return -3;
                case 'u':
                case 'μ':
                    return -2;
                case 'm':
                    return -1;
                */
                case ' ':
                case '\0':
                    return 0;
                case 'K':
                case 'k':
                    return 1;
                case 'M':
                    return 2;
                case 'G':
                    return 3;
                case 'T':
                    return 4;
                case 'P':
                    return 5;
                case 'E':
                    return 6;
                case 'Z':
                    return 7;
                case 'Y':
                    return 8;
                default:
                    throw new ArgumentException("Invalid multiplier character: " + multiplier);
            }
        }

        public static int MetricMultiplierToPower(char multiplier)
        {
            switch (multiplier)
            {
                case 'y':
                    return -24;
                case 'z':
                    return -21;
                case 'a':
                    return -18;
                case 'f':
                    return -15;
                case 'n':
                    return -12;
                case 'p':
                    return -9;
                case 'u':
                case 'μ':
                    return -6;
                case 'm':
                    return -3;
                case 'c':
                    return -2;
                case 'd':
                    return -1;
                case ' ':
                case '\0':
                    return 0;
//                case 'D':
//                    return 1;
                case 'h':
                    return 2;
                case 'K':
                case 'k':
                    return 3;
                case 'M':
                    return 6;
                case 'G':
                    return 9;
                case 'T':
                    return 12;
                case 'P':
                    return 15;
                case 'E':
                    return 18;
                case 'Z':
                    return 21;
                case 'Y':
                    return 24;
                default:
                    throw new ArgumentException("Invalid multiplier character: " + multiplier);
            }
        }

        /// <summary>
        /// The value of one unit with the given multiplier character.
        /// </summary>
        /// <param name="multiplier"></param>
        /// <param name="binary">true for a binary multiplier (power of 1024); false for a decimal one (power of 1000).</param>
        /// <returns></returns>
        public static long MultiplierValue(char multiplier, bool binary = true)
        {
            return MathUtilities.MathUtil.Pow(binary ? 1024 : 1000, MultiplierToPower(multiplier));
        }

        /// <summary>
        /// The character in a unit symbol that indicates a binary multiplier.
        /// Must be lower case.
        /// </summary>
        protected const char BinaryIndicator = 'i';

    }
}
