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
    }


}
