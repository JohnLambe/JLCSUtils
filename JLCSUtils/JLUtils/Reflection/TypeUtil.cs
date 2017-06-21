﻿using JohnLambe.Util.Types;
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
        public static bool IsNumeric(object value)
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
        /// <param name="value"></param>
        /// <returns>True iff the given object is of a primitive (or nullable primitive) integer type.
        /// false if the value is null.</returns>
        public static bool IsInteger(object value)
            => value == null ? false : value.GetType().IsIntegerType();

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
        private static bool IsIntegerTypeInternal([Nullable] this Type t)
            => t == typeof(SByte) || t == typeof(Byte)    // 8-bit
                || t == typeof(Int16) || t == typeof(UInt16)
                || t == typeof(Int32) || t == typeof(UInt32)
                || t == typeof(Int64) || t == typeof(UInt64);

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true iff the type represents an non-integer numeric value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsFloatingPointTypeInternal([Nullable] this Type t)
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
        /// Same as <see cref="Type.ToString()"/> except that null is treated as void.
        /// </summary>
        /// <param name="t"></param>
        /// <returns>The type name or "void".</returns>
        [return: NotNull]
        public static string TypeNameOrVoid([Nullable]Type t)
        {
            return TypeOrVoid(t).ToString();
        }

        /// <summary>
        /// Returns the C# keyword for the given type if there is one (for primitive types),
        /// otherwise the short name of the type.
        /// null is treated as <see cref="void"/>.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [return: NotNull]
        public static string CsType([Nullable] Type t)
        {
            if (t == null || t == typeof(void))
                return "void";

            if(t.IsPrimitive)
            { 
                // Integer:
                if(t == typeof(int))
                    return "int";
                if (t == typeof(uint))
                    return "uint";
                if (t == typeof(byte))
                    return "byte";
                if (t == typeof(sbyte))
                    return "sbyte";
                if (t == typeof(short))
                    return "short";
                if (t == typeof(ushort))
                    return "ushort";
                if (t == typeof(long))
                    return "long";
                if (t == typeof(ulong))
                    return "ulong";

                // Floating point:
                if (t == typeof(float))
                    return "float";
                if (t == typeof(double))
                    return "double";
                if (t == typeof(decimal))
                    return "decimal";
            }

            return t.Name;
        }

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>The given type, or typeof(void) if <paramref name="t"/> is null.</returns>
        [return: NotNull]
        public static Type TypeOrVoid([Nullable]Type t)
        {
            return t ?? typeof(void);
        }

        /// <summary>
        /// Determine which type is "more specific", according to this definition:<br/>
        /// X is more specific than Y iff Y is assignable from X (i.e. Y=X is valid) but not vice versa,
        /// except that anything is more specific than null.
        /// <para>Any subclass of a class is more specific than that class.
        /// Classes are more specific than interfaces that they implement.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>
        /// &gt;0 : <paramref name="a"/> is more specific;<br/>
        /// &lt;0 : <paramref name="b"/> is more specific;<br/>
        /// &gt;0 : they are equally specific.
        /// </returns>
        /// <remarks>This is used where the two types are types supported by different handlers.
        /// The one that supports the "more specific" type is a more specific (less general) handler and is usually preferred when both could be used.
        /// <para>The return value is chosen for consistency with <see cref="IComparer{T}"/>.</para>
        /// </remarks>
        public static int IsMoreSpecific([Nullable] Type a, [Nullable] Type b)
        {
            if (a == b)
            {
                return 0;               // equal, therefore equally specific (may be null)
            }
            else  // at least one is not null (if both were null, they'd be equal)
            {
                if (a == null)          // only `a` is null
                    return -1;          // `a` is less specific because everything else is more specific than null
                else if (b == null)     // only `b` is null 
                    return 1;
            }

            bool aFromB = a.IsAssignableFrom(b);
            bool bFromA = b.IsAssignableFrom(a);

            if (aFromB && !bFromA)
                return -1;
            else if (bFromA && !aFromB)
                return 1;
            else
                return 0;
        }
    }
}
