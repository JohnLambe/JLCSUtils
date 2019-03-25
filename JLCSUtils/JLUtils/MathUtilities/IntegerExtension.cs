using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JohnLambe.Util.MathUtilities
{
    public static class IntegerExtension
    {
        /// <summary>
        /// Returns the value of one digit of a given number.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digit">The 0-based index of the digit to be returned starting at the least significant - 0 is the least significant, 1 is the second least significant.</param>
        /// <param name="radix">The radix to use for determining the digit.</param>
        /// <returns>the value of the requested digit, in the range 0..<paramref name="radix"/>-1.</returns>
        public static int Digit(this int value, int digit, int radix = 10)
        {
            double digitValue = System.Math.Pow(radix, digit);
            return (int)((value / digitValue) % radix);
        }

        /// <summary>
        /// Changes a digit of a number: Replaces digit <paramref name="digit"/> of <paramref name="value"/> with <paramref name="newDigitValue"/>.
        /// </summary>
        /// <param name="value">the number to modify.</param>
        /// <param name="digit">The 0-based index of the digit to be returned starting at the least significant - 0 is the least significant, 1 is the second least significant.
        /// The can be beyond the number of non-zero digits.
        /// </param>
        /// <param name="newDigitValue"></param>
        /// <param name="radix">The radix to use for determining the digit.</param>
        /// <returns>the modified value.</returns>
        public static int SetDigit(this int value, int digit, int newDigitValue, int radix = 10)
        {
            int digitValue = (int)System.Math.Pow(radix, digit);
            return value + (-value.Digit(digit) + newDigitValue) * digitValue;
        }

    }
}
