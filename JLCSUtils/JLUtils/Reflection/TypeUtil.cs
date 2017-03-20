using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    public static class TypeUtil
    {
        #region Numeric

        public static bool IsNumeric(Object value)
            => value.GetType().IsNumericType();
        /*            return value is SByte || value is Byte    // 8-bit
                        || value is Int16 || value is UInt16
                        || value is Int32 || value is UInt32
                        || value is Int64 || value is UInt64  // long
                        || value is Double
                        || value is Single
                        || value is Decimal
                        ;*/

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true iff the type represents a numeric value.</returns>
        public static bool IsNumericType(this Type t)
        {
            t = t.GetNonNullableType();
            return IsIntegerTypeInternal(t) || IsFloatingPointTypeInternal(t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true iff the type represents an integer value.</returns>
        public static bool IsIntegerType(this Type t)
            => IsIntegerTypeInternal(t.GetNonNullableType());

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true iff the type represents an non-integer numeric value.</returns>
        public static bool IsFloatingPointType(this Type t)
            => IsFloatingPointTypeInternal(t.GetNonNullableType());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true iff the type represents an integer value.</returns>
        private static bool IsIntegerTypeInternal(this Type t)
            => t == typeof(SByte) || t == typeof(Byte)    // 8-bit
                || t == typeof(Int16) || t == typeof(UInt16)
                || t == typeof(Int32) || t == typeof(UInt32)
                || t == typeof(Int64) || t == typeof(UInt64);

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true iff the type represents an non-integer numeric value.</returns>
        private static bool IsFloatingPointTypeInternal(this Type t)
            => t == typeof(Double) || t == typeof(Single)
                || t == typeof(Decimal);

        #endregion

        #region Text

        public static bool IsTextType(this Type t)
            => IsTextTypeInternal(t.GetNonNullableType());

        private static bool IsTextTypeInternal(this Type t)
            => t == typeof(Char) || t == typeof(String);

        #endregion
    }
}
