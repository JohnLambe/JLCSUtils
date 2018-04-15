// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using JohnLambe.Util.Reflection;
using JohnLambe.Util.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Math
{
    /// <summary>
    /// Mathematical functions.
    /// </summary>
    /// <seealso cref="System.Math"/>
    public static class MathUtil
    {
        /// <summary>
        /// Rounds a value of any numeric type (whose type is not necessarily known) and returns the same type.
        /// If the given value is not of a numeric type, this tries to convert it to decimal, round it and convert back to the original type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimalPlaces"></param>
        /// <param name="midPointRounding"></param>
        /// <returns></returns>
        public static object Round(object value, int decimalPlaces, MidpointRounding midPointRounding = MidpointRounding.ToEven)
        {
            if (value == null)
                return null;
            if(value is double)
            {
                return System.Math.Round((double)value, decimalPlaces, midPointRounding);
            }
            else if(value is float)
            {
                return (float)System.Math.Round((float)value, decimalPlaces, midPointRounding);  // round and convert back to float (this uses the `double` overload of Round since there isn't a float one.
            }   // the cast of the parameter to Round is so that if a future .NET framework has a `float` overload, it will use it.
            else if (value is decimal)
            {
                return (decimal)System.Math.Round((decimal)value, decimalPlaces, midPointRounding);
            }
            else if (TypeUtil.IsIntegerType(value.GetType()))  // if integer
            {
                return value;                                  // no rounding required
            }
            else    // non-numeric
            {
                decimal numericValue = GeneralTypeConverter.Convert<decimal>(value);
                numericValue = System.Math.Round(numericValue, decimalPlaces, midPointRounding);
                return GeneralTypeConverter.Convert(numericValue, value.GetType());
            }
        }

        #region RoundToMultiple
        //| Not named 'Round' because the `multiple` parameter could be confused with a number of decimal places (as in System.Math.Round).

        /// <summary>
        /// Round <paramref name="value"/> to the a multiple of <paramref name="multiple"/>.
        /// </summary>
        /// <param name="value">the value to be rounded.</param>
        /// <param name="multiple"></param>
        /// <param name="rounding">Specification for how to round if the value is midway between two other numbers.</param>
        /// <returns>the rounded value</returns>
        public static decimal RoundToMultiple(decimal value, decimal multiple, MidpointRounding rounding = MidpointRounding.ToEven)
        {
            return System.Math.Round(value / multiple, rounding) * multiple;
        }

        /// <summary>
        /// Round <paramref name="value"/> to the a multiple of <paramref name="multiple"/>.
        /// </summary>
        /// <param name="value">the value to be rounded.</param>
        /// <param name="multiple"></param>
        /// <param name="rounding">Specification for how to round if the value is midway between two other numbers.</param>
        /// <returns>the rounded value</returns>
        public static decimal? RoundToMultiple(decimal? value, decimal multiple, MidpointRounding rounding = MidpointRounding.ToEven)
        {
            if (value == null)
                return null;
            else
                return RoundToMultiple(value.Value, multiple, rounding);
        }

        /// <summary>
        /// Round <paramref name="value"/> to the a multiple of <paramref name="multiple"/>.
        /// </summary>
        /// <param name="value">the value to be rounded.</param>
        /// <param name="multiple"></param>
        /// <param name="rounding">Specification for how to round if the value is midway between two other numbers.</param>
        /// <returns>the rounded value</returns>
        public static double RoundToMultiple(double value, double multiple, MidpointRounding rounding = MidpointRounding.ToEven)
        {
            return System.Math.Round(value / multiple, rounding) * multiple;
        }

        /// <summary>
        /// Round <paramref name="value"/> to the a multiple of <paramref name="multiple"/>.
        /// </summary>
        /// <param name="value">the value to be rounded.</param>
        /// <param name="multiple"></param>
        /// <param name="rounding">Specification for how to round if the value is midway between two other numbers.</param>
        /// <returns>the rounded value</returns>
        public static double? RoundToMultiple(double? value, double multiple, MidpointRounding rounding = MidpointRounding.ToEven)
        {
            if (value == null)
                return null;
            else
                return RoundToMultiple(value.Value, multiple, rounding);
        }

        /// <summary>
        /// Round <paramref name="value"/> to the a multiple of <paramref name="multiple"/>.
        /// </summary>
        /// <param name="value">the value to be rounded.</param>
        /// <param name="multiple"></param>
        /// <param name="rounding">Specification for how to round if the value is midway between two other numbers.</param>
        /// <returns>the rounded value</returns>
        public static int RoundToMultiple(int value, int multiple, MidpointRounding rounding = MidpointRounding.ToEven)
        {
            return (int)System.Math.Round(0.0+value / multiple, rounding) * multiple;
            //| Could be optimised by not using floating point.
        }

        /// <summary>
        /// Round <paramref name="value"/> to the a multiple of <paramref name="multiple"/>.
        /// </summary>
        /// <param name="value">the value to be rounded.</param>
        /// <param name="multiple"></param>
        /// <param name="rounding">Specification for how to round if the value is midway between two other numbers.</param>
        /// <returns>the rounded value</returns>
        public static int? RoundToMultiple(int? value, int multiple, MidpointRounding rounding = MidpointRounding.ToEven)
        {
            if (value == null)
                return null;
            else
                return RoundToMultiple(value.Value, multiple, rounding);
        }

        #endregion

        #region Round Nullable
        // Round nullable types. If the input is null, the output is null.

        //
        // Summary:
        //     Rounds a decimal value to the nearest integral value.
        //
        // Parameters:
        //   d:
        //     A decimal number to be rounded.
        //
        // Returns:
        //     The integer nearest parameter d. If the fractional component of d is halfway
        //     between two integers, one of which is even and the other odd, the even number
        //     is returned. Note that this method returns a System.Decimal instead of an integral
        //     type.
        //
        // Exceptions:
        //   T:System.OverflowException:
        //     The result is outside the range of a System.Decimal.
        public static decimal? Round(decimal? d)
        {
            if (d == null)
                return null;
            else
                return System.Math.Round(d.Value);
        }
        //
        // Summary:
        //     Rounds a double-precision floating-point value to the nearest integral value.
        //
        // Parameters:
        //   a:
        //     A double-precision floating-point number to be rounded.
        //
        // Returns:
        //     The integer nearest a. If the fractional component of a is halfway between two
        //     integers, one of which is even and the other odd, then the even number is returned.
        //     Note that this method returns a System.Double instead of an integral type.
        //[SecuritySafeCritical]
        public static double? Round(double? d)
        {
            if (d == null)
                return null;
            else
                return System.Math.Round(d.Value);
        }

        //
        // Summary:
        //     Rounds a decimal value to the nearest integer. A parameter specifies how to round
        //     the value if it is midway between two numbers.
        //
        // Parameters:
        //   d:
        //     A decimal number to be rounded.
        //
        //   mode:
        //     Specification for how to round d if it is midway between two other numbers.
        //
        // Returns:
        //     The integer nearest d. If d is halfway between two numbers, one of which is even
        //     and the other odd, then mode determines which of the two is returned.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     mode is not a valid value of System.MidpointRounding.
        //
        //   T:System.OverflowException:
        //     The result is outside the range of a System.Decimal.
        public static decimal? Round(decimal? d, MidpointRounding mode)
        {
            if (d == null)
                return null;
            else
                return System.Math.Round(d.Value, mode);
        }
        //
        // Summary:
        //     Rounds a decimal value to a specified number of fractional digits.
        //
        // Parameters:
        //   d:
        //     A decimal number to be rounded.
        //
        //   decimals:
        //     The number of decimal places in the return value.
        //
        // Returns:
        //     The number nearest to d that contains a number of fractional digits equal to
        //     decimals.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     decimals is less than 0 or greater than 28.
        //
        //   T:System.OverflowException:
        //     The result is outside the range of a System.Decimal.
        public static decimal? Round(decimal? d, int decimals)
        {
            if (d == null)
                return null;
            else
                return System.Math.Round(d.Value,decimals);
        }
        //
        // Summary:
        //     Rounds a double-precision floating-point value to the nearest integer. A parameter
        //     specifies how to round the value if it is midway between two numbers.
        //
        // Parameters:
        //   value:
        //     A double-precision floating-point number to be rounded.
        //
        //   mode:
        //     Specification for how to round value if it is midway between two other numbers.
        //
        // Returns:
        //     The integer nearest value. If value is halfway between two integers, one of which
        //     is even and the other odd, then mode determines which of the two is returned.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     mode is not a valid value of System.MidpointRounding.
        public static double? Round(double? d, MidpointRounding mode)
        {
            if (d == null)
                return null;
            else
                return System.Math.Round(d.Value, mode);
        }

        //
        // Summary:
        //     Rounds a double-precision floating-point value to a specified number of fractional
        //     digits.
        //
        // Parameters:
        //   value:
        //     A double-precision floating-point number to be rounded.
        //
        //   digits:
        //     The number of fractional digits in the return value.
        //
        // Returns:
        //     The number nearest to value that contains a number of fractional digits equal
        //     to digits.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     digits is less than 0 or greater than 15.
        public static double? Round(double? d, int digits)
        {
            if (d == null)
                return null;
            else
                return System.Math.Round(d.Value,digits);
        }

        //
        // Summary:
        //     Rounds a double-precision floating-point value to a specified number of fractional
        //     digits. A parameter specifies how to round the value if it is midway between
        //     two numbers.
        //
        // Parameters:
        //   value:
        //     A double-precision floating-point number to be rounded.
        //
        //   digits:
        //     The number of fractional digits in the return value.
        //
        //   mode:
        //     Specification for how to round value if it is midway between two other numbers.
        //
        // Returns:
        //     The number nearest to value that has a number of fractional digits equal to digits.
        //     If value has fewer fractional digits than digits, value is returned unchanged.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     digits is less than 0 or greater than 15.
        //
        //   T:System.ArgumentException:
        //     mode is not a valid value of System.MidpointRounding.
        public static double? Round(double? d, int digits, MidpointRounding mode)
        {
            if (d == null)
                return null;
            else
                return System.Math.Round(d.Value,mode);
        }

        //
        // Summary:
        //     Rounds a decimal value to a specified number of fractional digits. A parameter
        //     specifies how to round the value if it is midway between two numbers.
        //
        // Parameters:
        //   d:
        //     A decimal number to be rounded.
        //
        //   decimals:
        //     The number of decimal places in the return value.
        //
        //   mode:
        //     Specification for how to round d if it is midway between two other numbers.
        //
        // Returns:
        //     The number nearest to d that contains a number of fractional digits equal to
        //     decimals. If d has fewer fractional digits than decimals, d is returned unchanged.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     decimals is less than 0 or greater than 28.
        //
        //   T:System.ArgumentException:
        //     mode is not a valid value of System.MidpointRounding.
        //
        //   T:System.OverflowException:
        //     The result is outside the range of a System.Decimal.
        public static decimal? Round(decimal? d, int decimals, MidpointRounding mode)
        {
            if (d == null)
                return null;
            else
                return System.Math.Round(d.Value,mode);
        }

        #endregion

    }

}
