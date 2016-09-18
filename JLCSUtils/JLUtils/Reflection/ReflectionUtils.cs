using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;


namespace JohnLambe.Util.Reflection
{
    public static class AttributeUtils
    {
        public static T GetCustomAttribute<T>(this ICustomAttributeProvider type)
        {
            foreach (var attribute in type.GetCustomAttributes(true))
            {
                if (attribute is T)
                    return (T)attribute;
            }
            return default(T);
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider type, bool inherit = true)
        {
            /*
            // Allocate a temporary array for the filtered collection of attributes, then copy to an array of the correct type.
            // Three arrays allocated.
            object[] attribs = type.GetCustomAttributes(inherit).Where(a => a is T).ToArray();
            T[] result = new T[attribs.Length];
            attribs.CopyTo(result, 0);
            return result;
            */

            // The extra call to Count may cause Type.GetCustomAttributes(bool) to be called twice.
            var attribs = type.GetCustomAttributes(inherit).Where(a => a is T);
            T[] result = new T[attribs.Count()];
            int index = 0;
            foreach(object a in attribs)
            {
                result[index++] = (T)a;
            }
            return result;

            /* Alternative implementation:
            // Evaluates only once, but uses a list.
            var list = new List<T>();
            foreach (var attribute in type.GetCustomAttributes(inherit).Where(a => a is T))
            {
                list.Add((T)attribute);
            }
            return list;
            */
            // Can't do     return (IEnumerable<T>)type.GetCustomAttributes(inherit).Where( a => a is T );
            // because type.GetCustomAttributes(bool) returns object[] .
            // The above could only return IEnumerable<object> .
        }
        /*
                public static IEnumerable<T> GetCustomAttributes<T>(this ICustomAttributeProvider type)
                {
                    return GetCustomAttributes<T>(type, true);
                }
        */
        public static bool HasCustomAttribute<T>(this ICustomAttributeProvider type)
        {
            return type.GetCustomAttribute<T>() != null;
        }
    }

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
    }
}
