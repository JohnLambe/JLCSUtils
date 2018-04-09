using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JohnLambe.Util.Reflection;

namespace JohnLambe.Util.TypeConversion
{
    /// <summary>
    /// Static class for use instead of System.Config.ChangeType(),
    /// to enable changing the implementation in future (to support more conversions).
    /// </summary>
    public static class GeneralTypeConverter
    {
        /// <summary>
        /// Try to convert <paramref name="source"/> to type <typeparamref name="T"/>.
        /// May throw an exception if it cannot be converted.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="source">the value to be converted.</param>
        /// <returns>the converted value.</returns>
        public static T Convert<T>(object source)
        {
            return Convert<T>(source, typeof(T));
        }

        /// <summary>
        /// Try to convert <paramref name="source"/> to type <paramref name="requiredType"/>.
        /// May throw an exception if it cannot be converted.
        /// </summary>
        /// <typeparam name="T">The type to return, which must be assignable from <paramref name="requiredType"/>.</typeparam>
        /// <param name="source">the value to be converted.</param>
        /// <param name="requiredType">The type to convert to.</param>
        /// <returns>the converted value.</returns>
        public static T Convert<T>(object source, Type requiredType)
        {
            /*
            if(source == null && (!requiredType.IsValueType || requiredType.IsNullableValueType()))  // if nullable (either a reference type or a nullable value type)
            {
                return default(T);   // return the default. This will be null if `T` is requiredType.
            }
            */
            if (source == null ||                                                      // converting null to anything,
                (typeof(T).IsValueType && (source?.ToString()?.Equals("") ?? true)))   // converting null or "" (or ToString() returns null) to a value type
            {
                return default(T);
            }

            if (source != null && requiredType.IsAssignableFrom(source.GetType()))        // if it can be cast (this has to be tried first, since ChangeType supports only IConvertible)
            {
                try
                {
                    return (T)source;                            // cast it
                }
                catch (InvalidCastException)
                {   // if this fails, ignore the exception. We'll try to convert it below.
                }
            }

            Type nullableUnderlyingType = Nullable.GetUnderlyingType(requiredType);  // null if not nullable
            Type targetType = nullableUnderlyingType ?? requiredType;

            try
            {
                // Enum:
                if(targetType.IsEnum && !TypeUtil.IsInteger(source))  // if converting a non-integer to an enum
                {                                                     // this should work for integers too, but would probably be less efficient than later methods
                    try
                    {   // try to convert from an enum name as a string:
                        return (T) Enum.Parse(targetType, source.ToString().Trim());
                    }
                    catch(SystemException ex) when(ex is ArgumentException || ex is OverflowException)
                    {   // ignore exception and continue to try other methods
                    }
                }

                // Guid:
                if (targetType == typeof(Guid) && !TypeUtil.IsInteger(source))  // if converting a non-integer to an enum
                {                                                     // this should work for integers too, but would probably be less efficient than later methods
                    try
                    {
                        if (source is byte[])
                            return (T) (object) new Guid((byte[])source);

                        return (T) (object) new Guid(source.ToString());
                    }
                    catch (SystemException ex) when (ex is ArgumentException || ex is OverflowException || ex is FormatException)
                    {   // ignore exception and continue to try other methods
                    }
                }

                return (T)System.Convert.ChangeType(source, targetType);
                // if nullable, try to convert to the underlying type. System.Convert fails on trying to convert to Nullable<T>.
            }
            catch(InvalidCastException) when (source != null)  // if the cast failed, even if the target type is not a string (because the result of ToString() might be convertible to the required type)
            //when(requiredType == typeof(string))  // if failed and a string is required
            {
                return (T)System.Convert.ChangeType(source.ToString(), targetType);
            }
        }

        /// <summary>
        /// Same as Convert&lt;object&gt;.
        /// </summary>
        /// <param name="source">the value to be converted.</param>
        /// <param name="requiredType">The type to convert to.</param>
        /// <returns>the converted value.</returns>
        public static object Convert(object source, Type requiredType)
        {
            return Convert<object>(source, requiredType);
        }
    }
}