using JohnLambe.Util.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnLambe.Util
{
    /// <summary>
    /// Provides a default value.
    /// </summary>
    /// <typeparam name="T">The type of the default value. Should be a type that is assignable to the implementing type.</typeparam>
    public interface IHasDefaultValue<T>
    {
        /// <summary>
        /// A default value for this type.
        /// </summary>
        /// <returns></returns>
        T GetDefaultValue();
    }

    public static class ObjectExtension 
    {
        public static T DefaultValue<T>(this T thisValue) where T : class
        {
            if (thisValue is IHasDefaultValue<T>)
                return ((IHasDefaultValue<T>)thisValue).GetDefaultValue();
            else if (thisValue is String)
                return "" as T;
            else if (TypeUtil.IsNumeric(thisValue))
                return 0 as T;
            else
                return default(T);  // null
        }

        /// <summary>
        /// Returns the default value defined by the <see cref="JohnLambe.Util.IHasDefaultValue{T}"/> implementation
        /// if the instance is null.
        /// If the instance is not null, it is returned, provided that is is of type T (the type of default value).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static T NullToDefault<T>(this IHasDefaultValue<T> thisValue) where T : class
        {
            if (thisValue == null)
            {
                return thisValue.GetDefaultValue();
            }
            else
            {
                return (T)thisValue;
            }
        }

        public static T NullToDefault<T>(T thisValue) where T : class
        {
            if (thisValue == null)
            {
                return thisValue.DefaultValue();
            }
            else
            {
                return thisValue;
            }
        }

        // See MiscUtil.FirstNonNull
        // Use ?? in C# 4.0 or later
        [Obsolete]
        public static T Coalesce<T>(this T thisValue, params T[] values) where T: class
        {
            if (thisValue != null)
            {
                return thisValue;
            }
            else
            {
                foreach (var value in values)
                {
                    if (value != null)
                    {
                        return value;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Throws an exception if this is null.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to be checked for null.</param>
        /// <param name="message">Error message to be given if null.</param>
        /// <param name="exceptionType">The type of exception to throw if <paramref name="value"/> is null.</param>
        /// <returns>value (the object this is called on).</returns>
        /// <exception cref="NullReferenceException">If this is not null.</exception>
        public static T NotNull<T>(this T value, string message = null, Type exceptionType = null)
            where T: class
            // Restricted to reference types. See comment (in this position) on ArgNotNull regarding generic types.
        {
            return CheckNotNull(value, message, exceptionType);
        }

        /// <summary>
        /// Same as NotNull, except that this can be called with any type.
        /// This can be used for generic types which may or may not be nullable.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to be checked for null.</param>
        /// <param name="message">Error message to be given if null.</param>
        /// <param name="exceptionType">The type of exception to throw if <paramref name="value"/> is null.</param>
        /// <returns>value (the object this is called on).</returns>
        public static T CheckNotNull<T>(T value, string message = null, Type exceptionType = null)
        {
            if (value == null)
            {
                if (message != null)
                    throw new NullReferenceException(message);
                else
                    throw new NullReferenceException("Null reference of type " + typeof(T).Name + " (checked with ObjectExtension.NotNull)");
            }
            return value;
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> exception if this is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value to be checked for null.</param>
        /// <param name="parameterName">The name of the parameter being checked (for reporting in the error message).</param>
        /// <param name="message">Error message to be given if null.</param>
        /// <returns>value (the object this is called on).</returns>
        /// <exception cref="ArgumentNullException">If this is not null.</exception>
        public static T ArgNotNull<T>(this T value, string parameterName = null, string message = null)
            where T : class
            // This is restricted to reference types, since it could be annoying to have the extension method appearing in intellisense for types that can't be null.
            // It could be useful when T is a generic type, which may be a value or reference type. In that case CheckArgNotNull can be used.
        {
            return CheckArgNotNull(value, parameterName, message);
        }

        public static T CheckArgNotNull<T>(T value, string parameterName = null, string message = null)
        {
            if (value == null)
            {
                if (message == null)
                {
                    message = "Null argument "
                        + (parameterName == null ? "" : " (" + parameterName + ")")
                        + " of type " + typeof(T).Name + " (checked with ObjectExtension.ArgNotNull)";
                }
                throw new ArgumentNullException(parameterName, message);
            }
            return value;
        }

    }

}
