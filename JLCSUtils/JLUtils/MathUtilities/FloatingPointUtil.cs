using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.MathUtilities
{
    public static class FloatingPointUtil
    {
        /// <summary>
        /// Return the number of digits in a number.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="radix"></param>
        /// <returns>The number of digits in the number before the (decimal) point. Always positive.</returns>
        public static int NumberOfDigits(decimal value, int radix = 10)
        {
            decimal maxValue = Decimal.MaxValue / radix;
            value = System.Math.Abs(value);
            decimal digitValue = radix;
            int digits = 1;
            while (digitValue < maxValue)
            {
                if (value < digitValue)
                    return digits;
                digits++;
                digitValue *= radix;
            }
            return digits;
        }

        /// <summary>
        /// Returns the number of digits in a number after the (decimal) point.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="radix"></param>
        /// <returns></returns>
        public static int FractionNumberOfDigits(decimal value, int radix = 10)
        {
            decimal maxValue = Decimal.MaxValue / radix;
            value = System.Math.Abs(value);
            decimal digitValue = radix;
            int digits = 1;
            while (digitValue < maxValue)
            {
                if (value < digitValue)
                    return digits;
                digits++;
                digitValue *= radix;
            }
            return digits;
        }

    }
}
