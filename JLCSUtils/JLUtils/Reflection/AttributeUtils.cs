using System;
using System.Collections.Generic;
using System.Linq;

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
            foreach (object a in attribs)
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
}
