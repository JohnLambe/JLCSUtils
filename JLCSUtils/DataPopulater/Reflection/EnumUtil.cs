using JohnLambe.Util.Diagnostic;
using JohnLambe.Util.Text;
using JohnLambe.Util.TypeConversion;
using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        /// <summary>
        /// Return the display name of a given enum value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum value)
        {
            var field = GetField(value);
            return field?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                ?? field?.GetCustomAttribute<DisplayAttribute>()?.Name
                ?? CaptionUtil.PascalCaseToCaption(field?.Name)
                ?? value.ToString();
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

        /// <summary>
        /// Convert a value of one enum type to the value with the same name in another enum type.
        /// </summary>
        /// <typeparam name="TDest">The enum type to convert to. Must be an enum or nullable enum type.</typeparam>
        /// <param name="source">The value to convert.</param>
        /// <param name="ignoreCase">Match the name non-case-sensitively.</param>
        /// <returns>The converted value.
        /// null if the type is nullable and the source value is null.
        /// </returns>
        /// <exception cref="ArgumentException">If the source value does not have a corresponding value on the destination type.</exception>
        [return: Nullable]
        public static TDest ConvertEnum<TDest>([Nullable] Enum source, bool ignoreCase = false)
        {
            if (source == null)
                return (TDest)(object)null;     // try to return null (fails if TDest is not nullable)
            Type destType = typeof(TDest).GetNonNullableType();
            Diagnostics.Assert(destType.IsEnum);
            //var name = Enum.GetName(source.GetType(), source);
            return (TDest)Enum.Parse(destType, source.ToString(), ignoreCase);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <param name="defaultValue">Value to return if the source does not have a corresponding value on the destination type.</param>
        /// <param name="ignoreCase">Match the name non-case-sensitively.</param>
        /// <returns>The converted value.
        /// null if the type is nullable and the source value is null.
        /// </returns>
        [return: Nullable]
        public static TDest ConvertEnum<TDest>([Nullable] Enum source, [Nullable] TDest defaultValue, bool ignoreCase = false)
        {
            try
            {
                return ConvertEnum<TDest>(source, ignoreCase);
            }
            catch (ArgumentException)
            {
                return defaultValue;
            }
        }

    }


    /// <summary>
    /// Base class for enum-related attributes.
    /// </summary>
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
    /// Placed on a bitmask in an enum flagged with <see cref="HybridEnumAttribute"/>.
    /// Specifies how the bits specified by the value of the attributed item are interpreted.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumHybridMemberAttribute : EnumAttribute
    {
        /// <summary>
        /// Treat these bits as being this type.
        /// <para>If <see cref="bool"/>, it should be considered true iff ANDing the enum value with the enum constant value yeilds a non-zero value.</para>
        /// <para>
        /// For all other types, the value of the bits (shifted to bring the least significant bit in the mask to bit position 0),
        /// as an integer, is cast to this type.
        /// </para>
        /// </summary>
        public virtual Type DataType { get; set; }

        /// <summary>
        /// Treat these bits as a signed (two's complement) value.
        /// The value is sign-extended (based on the most significant bit) before casting to <see cref="DataType"/>.
        /// </summary>
        public virtual bool Signed { get; set; }

        // Shift?
    }

    /// <summary>
    /// Flags that an enum constant (in an enum flagged with <see cref="HybridEnumAttribute"/>) should be treated as a flag.
    /// <para>This is the same as a Boolean <see cref="EnumHybridMemberAttribute"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumFlagAttribute : EnumHybridMemberAttribute
    {
        public EnumFlagAttribute()
        {
            DataType = typeof(bool);
        }
    }

    /// <summary>
    /// Flags that an enum constant should be treated as a mask,
    /// i.e. it identifies a group of bits (its set bits) which serve a certain purpose,
    /// or the result of ORing a collection or related flag enum constants (so that ANDing with this value extracts that set of flags).
    /// </summary>
    /// <remarks>This may be used by tools that provide a list of options based on an enum (so that the mask values can be omitted when
    /// a list of the actual or flag values is required).</remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumMaskAttribute : EnumAttribute
    {
    }

    /// <summary>
    /// Flags that an enum constant should be hidden in some list
    /// (For example, for special values not shown in a user interface).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumHiddenAttribute : EnumAttribute
    {
    }

    /// <summary>
    /// Flags that the attributed enum field represents a null/blank or similar value.
    /// </summary>
    /// <remarks>Tools may use this to represent the value as blank, or map it to null when converting to other types.</remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumNullValueAttribute : EnumAttribute
    {
    }

    /// <summary>
    /// Flags that an enum field is a duplicate (i.e. there is another one with the same value),
    /// and should be excluded from lists of unique values.
    /// All duplicates except the preferred one (the one to use when showing only unique items) should have this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumDuplicateAttribute : EnumAttribute
    {
    }

    /// <summary>
    /// Flags an enum value as obsolete, i.e. supported only for backward compatibility.
    /// </summary>
    /// <remarks>Tools may use this to display it in a different style, or hide it.
    /// Tools could use <see cref="ObsoleteAttribute"/> for this purpose too,
    /// but it may be desirable to change the display of an item without generating compiler warnings for its use.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class EnumObsoleteAttribute : EnumAttribute
    {
    }
}
