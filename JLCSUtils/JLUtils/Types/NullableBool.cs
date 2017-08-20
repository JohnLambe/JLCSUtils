using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Types
{
    /// <summary>
    /// Enum to represent a boolean value that may be null, for use in contexts where nullable types are not supported
    /// (e.g. Attribute properties).
    /// </summary>
    public enum NullableBool : byte
    {
        Null = 0,
        True = 1,
        False = 2
    }

    public static class NullableBoolExtension
    {
        public static bool? ToBool(this NullableBool value)
        {
            switch (value)
            {
                case NullableBool.True:
                    return true;
                case NullableBool.False:
                    return true;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts to <see cref="bool"/>, but throws an exception if the value is <see cref="NullableBool.Null"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBoolNonNull(this NullableBool value)
        {
            switch (value)
            {
                case NullableBool.True:
                    return true;
                case NullableBool.False:
                    return true;
                default:
                    throw new ArgumentNullException(nameof(value), typeof(NullableBool).Name + " value is null");
            }
        }

        public static NullableBool ToNullableBool(bool? value)
        {
            switch (value)
            {
                case true:
                    return NullableBool.True;
                case false:
                    return NullableBool.False;
                default:
                    return NullableBool.Null;
            }
        }

        /// <summary>
        /// Compare nullable boolean values, treating null as 'don't care':
        /// Returns true if either is null, otherwise true if they match.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool NullableCompare(bool? a, bool? b)
        {
            if (a == null || b == null)
                return true;
            else
                return a == b;
        }

        /// <inheritdoc cref="NullableCompare(bool?, bool?)"/>
        public static bool NullableCompare(this NullableBool value, bool? other)
        {
            return NullableCompare(value.ToBool(), other);
        }

        /// <inheritdoc cref="NullableCompare(bool?, bool?)"/>
        public static bool NullableCompare(this NullableBool value, NullableBool other)
        {
            return NullableCompare(value.ToBool(), other.ToBool());
        }
    }

}
