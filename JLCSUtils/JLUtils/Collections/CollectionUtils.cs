// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Collections
{
    /// <summary>
    /// Collection-related utilities and extension methods.
    /// </summary>
    public static class CollectionUtils
    {
        /// <summary>
        /// Add everything in <paramref name="otherCollection"/> to this collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="otherCollection"></param>
        /// <returns><paramref name="collection"/></returns>
        public static ICollection<T> AddAll<T>(this ICollection<T> collection, IEnumerable<T> otherCollection)
        {
            foreach(var item in otherCollection)
            {
                collection.Add(item);
            }
            return collection;
        }

        /// <summary>
        /// Returns a string containing a string representation of each item in the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="separator"></param>
        /// <returns>The string representation of the contents of the collection, or null if <paramref name="collection"/> is null.</returns>
        public static string CollectionToString<T>(ICollection<T> collection, string separator = ", ")
        {
            if (collection == null)
                return null;
            StringBuilder result = new StringBuilder();
            int index = 0;
            foreach(var item in collection)
            {
                if (index > 0)   // if not the first item
                    result.Append(separator);
                if(item != null)
                    result.Append(item.ToString());
                index++;
            }
            return result.ToString();
        }
    }
}
