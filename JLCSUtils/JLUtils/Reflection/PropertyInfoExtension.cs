using JohnLambe.Util.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// Extension methods for <see cref="PropertyInfo"/>.
    /// </summary>
    public static class PropertyInfoExtension
    {
        /// <summary>
        /// Set the value of a given property on a given object, converting the value if necessary.
        /// (Same as <see cref="PropertyInfo.SetValue(object, object, object[])"/> except for the conversion.)
        /// </summary>
        /// <param name="propertyInfo">The property to set. This must be a property of the type of <see cref="target"/>.</param>
        /// <param name="target">The object to set the property on.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="index">Optional index values for indexed properties. This value should be null for non-indexed properties.
        ///     (From <see cref="PropertyInfo.SetValue(object, object, object[])"/> documentation.)
        /// </param>
        /// <exception cref="System.ArgumentException">
        ///     The index array does not contain the type of arguments needed.-or- The property's
        ///     set accessor is not found. -or-value cannot be converted to the type of System.Reflection.PropertyInfo.PropertyType.
        ///     (From <see cref="PropertyInfo.SetValue(object, object, object[])"/> documentation.)
        /// </exception>
        /// <exception cref="System.Reflection.TargetException">
        ///     In the .NET for Windows Store apps or the Portable Class Library, catch System.Exception
        ///     instead.The object does not match the target type, or a property is an instance
        ///     property but obj is null.
        ///     (From <see cref="PropertyInfo.SetValue(object, object, object[])"/> documentation.)
        /// </exception>
        /// <exception cref="System.Reflection.TargetParameterCountException">
        ///     The number of parameters in index does not match the number of parameters the
        ///     indexed property takes.
        ///     (From <see cref="PropertyInfo.SetValue(object, object, object[])"/> documentation.)
        /// </exception>
        /// <exception cref="System.MethodAccessException">
        ///     In the .NET for Windows Store apps or the Portable Class Library, catch the base
        ///     class exception, System.MemberAccessException, instead.There was an illegal attempt
        ///     to access a private or protected method inside a class.
        ///     (From <see cref="PropertyInfo.SetValue(object, object, object[])"/> documentation.)
        /// </exception>
        /// <exception cref="System.Reflection.TargetInvocationException">
        ///     An error occurred while setting the property value. For example, an index value
        ///     specified for an indexed property is out of range. The System.Exception.InnerException
        ///     property indicates the reason for the error.
        ///     (From <see cref="PropertyInfo.SetValue(object, object, object[])"/> documentation.)
        /// </exception>
        public static void SetValueConverted(this PropertyInfo propertyInfo, object target, object value, object[] index = null)
        {
            object convertedValue = GeneralTypeConverter.Convert(value, propertyInfo.PropertyType);
            propertyInfo.SetValue(target, convertedValue, index);
        }

        /// <summary>
        /// Reads the value of the property on a given object, and converts it to a requested type.
        /// (Same as <see cref="PropertyInfo.GetValue(object, object[])"/> except for the conversion.)
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="propertyInfo"></param>
        /// <param name="target">The object whose property value will be returned.</param>
        /// <param name="index">Optional index values for indexed properties. The indexes of indexed properties
        ///     are zero-based. This value should be null for non-indexed properties.
        ///     (From <see cref="PropertyInfo.GetValue(object, object[])"/> documentation.)
        /// </param>
        /// <param name="requiredType">If not null, the value is converted to this type. This must be a type that is assignable to <typeparamref name="T"/>.
        /// If this is null, the value is converted to the type <typeparamref name="T"/>.</param>
        /// <returns>The property value converted to the requested type.</returns>
        /// <exception cref="ArgumentException">
        ///     The index array does not contain the type of arguments needed.-or- The property's
        ///     get accessor is not found.
        ///     (From <see cref="PropertyInfo.GetValue(object, object[])"/> documentation.)
        /// </exception>
        /// <exception cref="System.Reflection.TargetException">
        ///     In the .NET for Windows Store apps or the Portable Class Library, catch System.Exception
        ///     instead.The object does not match the target type, or a property is an instance
        ///     property but obj is null.
        ///     (From <see cref="PropertyInfo.GetValue(object, object[])"/> documentation.)
        /// </exception>
        /// <exception cref="System.Reflection.TargetParameterCountException">
        ///     The number of parameters in index does not match the number of parameters the
        ///     indexed property takes.
        ///     (From <see cref="PropertyInfo.GetValue(object, object[])"/> documentation.)
        /// </exception>
        /// <exception cref="System.MethodAccessException">
        ///     In the .NET for Windows Store apps or the Portable Class Library, catch the base
        ///     class exception, System.MemberAccessException, instead.There was an illegal attempt
        ///     to access a private or protected method inside a class.
        ///     (From <see cref="PropertyInfo.GetValue(object, object[])"/> documentation.)
        /// </exception>
        /// <exception cref="System.Reflection.TargetInvocationException">
        ///     An error occurred while retrieving the property value. For example, an index
        ///     value specified for an indexed property is out of range. The System.Exception.InnerException
        ///     property indicates the reason for the error.
        ///     (From <see cref="PropertyInfo.GetValue(object, object[])"/> documentation.)
        /// </exception>
        public static T GetValueConverted<T>(this PropertyInfo propertyInfo, object target, object[] index = null, Type requiredType = null)
        {
            object value = propertyInfo.GetValue(target, index);
            return GeneralTypeConverter.Convert<T>(value, requiredType ?? typeof(T));
        }
    }
}
