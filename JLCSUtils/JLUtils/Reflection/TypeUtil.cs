using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// Type-related utilities.
    /// </summary>
    public static class TypeUtil
    {
        #region Numeric

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <returns>True iff the given object is of a primitive (or nullable primitive) numeric type.
        /// false if the value is null.</returns>
        public static bool IsNumeric(Object value)
            => value == null ? false : value.GetType().IsNumericType();

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true iff the type is a primitive type (or nullable primitive type) representing a numeric value.</returns>
        public static bool IsNumericType(this Type t)
        {
            t = t.GetNonNullableType();
            return IsIntegerTypeInternal(t) || IsFloatingPointTypeInternal(t);
        }

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true iff the type is a primitive type (or nullable primitive type) representing an integer value.</returns>
        public static bool IsIntegerType(this Type t)
            => IsIntegerTypeInternal(t.GetNonNullableType());

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true iff the type is a primitive type (or nullable primitive type) representing an non-integer numeric value.</returns>
        public static bool IsFloatingPointType(this Type t)
            => IsFloatingPointTypeInternal(t.GetNonNullableType());

        #region Non-nullable

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true iff the type represents an integer value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsIntegerTypeInternal(this Type t)
            => t == typeof(SByte) || t == typeof(Byte)    // 8-bit
                || t == typeof(Int16) || t == typeof(UInt16)
                || t == typeof(Int32) || t == typeof(UInt32)
                || t == typeof(Int64) || t == typeof(UInt64);

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true iff the type represents an non-integer numeric value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsFloatingPointTypeInternal(this Type t)
            => t == typeof(Double) || t == typeof(Single)
                || t == typeof(Decimal);

        #endregion

        #endregion

        #region Text

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>True if the given type is a primitive string or character type.</returns>
        public static bool IsText(object value)
            => value==null ? false : IsTextType(value.GetType());

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>True if the given type is a primitive string or character type.</returns>
        public static bool IsTextType(this Type t)
            => IsTextTypeInternal(t.GetNonNullableType());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsTextTypeInternal(this Type t)
            => t == typeof(Char) || t == typeof(String);

        #endregion

        /// <summary>
        /// Same as <see cref="Type.ToString()"/> except that it returns "void" is passed null.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string TypeNameOrVoid(Type t)
        {
            return t?.ToString() ?? "void";
        }

    }
}
