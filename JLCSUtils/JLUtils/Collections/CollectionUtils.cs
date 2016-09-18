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
    }
}
