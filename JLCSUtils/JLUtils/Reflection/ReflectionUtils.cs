﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;


namespace JohnLambe.Util.Reflection
{
    public static class ReflectionUtils
    {

        // For AutoConfig

        /// <summary>
        /// Returns a list of all supertypes of the given one, starting at the highest (base class)
        /// and ending with (and including) `target` itself.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static LinkedList<Type> GetTypeHeirarchy(Type target)
        {
            LinkedList<Type> heirarchy = new LinkedList<Type>();
            while (target != null)
            {
                heirarchy.AddFirst(target);
                target = target.BaseType;
            }
            return heirarchy;
        }


        // end AutoConfig

        //        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider type, )


        public static void AssignProperty(object target, string propertyName, object value)
        {
            AssignProperty(target, propertyName, value, false);
        }

        /// <summary>
        /// Assign a specified property of the given object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="ignoreInvalid">Iff true, no exception is raised if the property does not exist.
        /// (Exceptions are still raised for invalid property values).</param>
        public static void AssignProperty(object target, string propertyName, object value, bool ignoreInvalid)
        {
            var property = target.GetType().GetProperty(propertyName);
            if (property != null)
            {
                System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(property.PropertyType); // try to get a converter for the target type
                if (converter != null && converter.CanConvertFrom(value.GetType()))  // if there is a converter
                /*CanConvertTo(property.PropertyType)*/
                {
                    if (converter.IsValid(value))
                    {
                        /*value = converter.ConvertTo(value, property.PropertyType);*/
                        value = converter.ConvertFrom(value);
                        property.SetValue(target, value, null);
                    }
                    else
                    {
                        throw new ArgumentException("Value of " + propertyName + " is invalid: " + value);
                        // parameter is not a valid string representation of the target type
                    }
                }
                else
                {
                    throw new ArgumentException("Invalid parameter: " + propertyName);
                    // property cannot be assigned from a string
                }
            }
            else
            {
                if (!ignoreInvalid)
                {
                    throw new ArgumentException("Unrecognised parameter: " + propertyName);
                    // no property with the given name
                }
            }
        }

        /// <summary>
        /// Instantiate this type with the given constructor arguments.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static T Create<T>(this Type type, params object[] arguments)
            where T: class
        {
            var argumentTypes = new Type[arguments.Length];
            for (int n = 0; n < arguments.Length; n++)
            {
                if (arguments[n] == null)
                    argumentTypes[n] = typeof(object);
                else
                    argumentTypes[n] = arguments[n].GetType();
            }
            return CreateT<T>(type,argumentTypes,arguments);
        }

        public static T CreateT<T>(Type type, Type[] argumentTypes, object[] arguments)
            where T: class
        {
            return (T)type.GetConstructor(argumentTypes).Invoke(arguments);
        }

        #region GetProperty

        private enum PropertyAction { GetProperty, GetValue, SetValue };

        private static PropertyInfo GetSetProperty(object target, string propertyName, PropertyAction action, ref object value)
        { 
            PropertyInfo property = null;
            string[] levels = propertyName.Split('.');
            for (int level = 0; level < levels.Length; level++)
            {
                property = target.GetType().GetProperty(levels[level]);
                if (property == null)
                    return null;
                if (level < levels.Length - 1)
                    target = property.GetValue(target);
            }
            switch(action)
            {
                case PropertyAction.GetValue:
                    value = property.GetValue(target);
                    break;
                case PropertyAction.SetValue:
                    property.SetValue(target, value);
                    break;
            }
            return property;
        }

        /// <summary>
        /// Return a possibly nested property of the given object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName">The property name, or nested property expression (property names separated by ".").</param>
        /// <returns>The requested property, or null if it does not exist (if any property in the chain doesn't exist).</returns>
        public static PropertyInfo GetProperty(object target, string propertyName)
        {
            object dummy = null;
            return GetSetProperty(target, propertyName, PropertyAction.GetProperty, ref dummy);
        }

        /// <summary>
        /// Get the value of a given (possibly nested) property on a given object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The object from which to read the property.</param>
        /// <param name="propertyName">The property name, or nested property expression (property names separated by ".").</param>
        /// <returns>The property value.</returns>
        public static T TryGetPropertyValue<T>(object target, string propertyName)
        {
            object value = null;
            GetSetProperty(target, propertyName, PropertyAction.GetValue, ref value);
            return (T)value;
        }

        /// <summary>
        /// Set the value of a given property on a given object.
        /// Does nothing if the property does not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">The object to set the property on.</param>
        /// <param name="propertyName">The property name, or nested property expression (property names separated by ".").</param>
        /// <param name="value">The value to set.</param>
        public static void TrySetPropertyValue<T>(object target, string propertyName, T value)
        {
            object valueObject = value;
            GetSetProperty(target, propertyName, PropertyAction.SetValue, ref valueObject);
        }

        #endregion
    }

}
