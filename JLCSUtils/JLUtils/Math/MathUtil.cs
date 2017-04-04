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
    }

}
