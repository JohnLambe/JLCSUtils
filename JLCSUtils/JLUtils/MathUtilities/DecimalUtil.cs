using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.MathUtilities
{
    public static class DecimalUtil
    {
        // Decimal range: 79,228,162,514,264,337,593,543,950,335 to -79,228,162,514,264,337,593,543,950,335

        /// <summary>
        /// Converts a <see cref="decimal"/> to a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToFloatingPointString(this decimal? value)
        {
            string stringValue = value.ToString();    // this may have trailing '0's after the decimal point, depending on the exponent
            if (stringValue.Contains('.'))
                return stringValue.TrimEnd('0').TrimEnd('.');  // remove the '.' if it is the last character remaining after removing the trailing '0's
            else
                return stringValue;
        }

        /// <summary>
        /// Returns the exponent value of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <seealso cref="decimal.GetBits(decimal)"/>
        public static int GetExponent(this decimal value)
        {
            return (decimal.GetBits(value)[3] >> 16) & 0xFF;
        }
    }
}
