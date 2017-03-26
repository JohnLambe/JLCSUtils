using JohnLambe.Util.Diagnostic;
using JohnLambe.Util.TypeConversion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// Utilities for working with enumeration types.
    /// </summary>
    public static class EnumUtil
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
            return (value.GetIntegerValue() & mask) != 0;
        }

        /// <summary>
        /// True if any of the bits in <paramref name="mask"/> are set in this enum value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static bool HasAnyFlag(this Enum value, Enum mask)
        {
            Diagnostics.PreCondition<ArgumentException>(value.GetType() == mask.GetType(), "Enum types don't match");
            return HasAnyFlag(value, mask.GetIntegerValue());
        }

        /// <summary>
        /// Like <see cref="HasAnyFlag(Enum, Enum)"/>, but tests that the enum type has the <see cref="FlagsAttribute"/> attribute.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <returns>True if any of the bits in <paramref name="mask"/> are set in this enum value.</returns>
        public static bool HasAnyFlagValidated(this Enum value, Enum mask)
        {
            if (!value.GetType().IsDefined<FlagsAttribute>())
                throw new ArgumentException("Not a Flags Enum");
            return HasAnyFlag(value, mask);
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
            return (value.GetIntegerValue() & mask) != 0;
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

        /// <summary>
        /// Convert an enum value to its underlying integer representation.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long GetIntegerValue(this Enum value)
        {
            return (long)System.Convert.ChangeType(value, typeof(long));
            // same as:            return (int)Enum.ToObject(value.GetType(), value);
            // but have to cast to Enum.GetUnderlyingType();
            //TODO: Is this more efficient?
        }

        /// <summary>
        /// Get the <see cref="FieldInfo"/> of the Enum type, for the given enum value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FieldInfo GetField(this Enum value)
        {
            var enumType = value.GetType();          // the type of the Enum that the given value is in
            return enumType.GetField(Enum.GetName(enumType, value));    // look up the field by name
            // (There's probably a more efficient way than this string lookup).
        }

        /*
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


    public abstract class EnumAttribute : Attribute
    {
    }

    /// <summary>
    /// Flags that an enum type contains a mixture of flag bits and a numeric value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
    public class HybridEnumAttribute : EnumAttribute
    {
    }

    /// <summary>
    /// Flags that an enum constant (in an enum flagged with <see cref="HybridEnumAttribute"/>) should be trated as a flag.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumFlagAttribute : EnumAttribute
    {
    }

    /// <summary>
    /// Placed on a bitmask in an enum flagged with <see cref="HybridEnumAttribute"/>.
    /// Specifies how the bits specified by the value of the attributed item are interpreted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumHybridMemberAttribute : EnumAttribute
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
