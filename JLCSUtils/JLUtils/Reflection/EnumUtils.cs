using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// </summary>
    public static class EnumUtils
    {
        /// <summary>
        /// True if any of the bits in <paramref name="mask"/> are set in this enum value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        /// <seealso cref="Enum.HasFlag(Enum)"/>
        public static bool HasAnyFlag(this Enum value, int mask)
        {
            return ((int)Enum.ToObject(value.GetType(), value) & mask) != 0;
        }

        /// <summary>
        /// True if any of the bits in <paramref name="mask"/> are set in this enum value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        /// <seealso cref="Enum.HasFlag(Enum)"/>
        public static bool HasAnyFlag(this Enum value, long mask)
        {
            return ((long)Enum.ToObject(value.GetType(), value) & mask) != 0;
        }


        /// <summary>
        /// Tests that the value of the enum is defined as a single enum constant.
        /// This does NOT check for a combination of flags that could match the value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>true iff valid.</returns>
        public static bool ValidateEnumValue(this Enum value)
        {
            return value.GetType().IsEnumDefined(value);
        }

        /*
        public static long GetIntegerValue(this Enum value)
        {

        }

        public static bool ValidateEnum(this Enum value)
        {
            Type enumType = value.GetType();

            if (ValidateEnumValue(value))            // if there is an enum constant that exactly matches the value
                return true;
            if (enumType.IsDefined(typeof(FlagsAttribute), true))
            {
                foreach(Enum enumValue in enumType.GetEnumValues())
                {
                    if(value.HasFlag(enumValue))
                    {
                           
                    }
                }

                /-*
                for (long bitValue = 1; bitValue < 1 << 64; bitValue = bitValue << 1)
                {
                    Enum.
                    if (((int)value & bitValue) != 0)
                        if (enumType.IsEnumDefined(bitValue))
                }
                *-/

            }
            return value.GetType().IsEnumDefined(value);
        }
        */
    }


    /// <summary>
    /// Flags that an enum type contains a mixture of flag bits and a numeric value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
    public class HybridEnumAttribute : Attribute
    {
    }

    /// <summary>
    /// Flags that an enum constant (in an enum flagged with <see cref="HybridEnumAttribute"/>) should be trated as a flag.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumFlagAttribute : Attribute
    {
    }

    /// <summary>
    /// Placed on a bitmask in an enum flagged with <see cref="HybridEnumAttribute"/>.
    /// Specifies how the bits specified by the value of the attributed item are interpreted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumHybridMemberAttribute : Attribute
    {
        /// <summary>
        /// Treat these bits as being this type.
        /// The value of the bits (shifted to make the bring the least significant bit in the mask to bit position 0),
        /// as an integer, is cast to this type.
        /// </summary>
        public virtual Type DataType { get; set; }

        /// <summary>
        /// Treat these bits as a signed (two's complement) value.
        /// The value sign-extended (based on the most significant bit) before casting to <see cref="DataType"/>.
        /// </summary>
        public virtual bool Signed { get; set; }

        // Shift?
    }

}
