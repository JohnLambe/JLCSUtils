// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using JohnLambe.Util.Types;
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
    public static class CollectionUtil
    {
        /// <summary>
        /// Add everything in <paramref name="otherSequence"/> to this collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements of both collections.</typeparam>
        /// <param name="sequence"></param>
        /// <param name="otherSequence"></param>
        /// <returns><paramref name="sequence"/></returns>
        public static ICollection<T> AddAll<T>(this ICollection<T> sequence, IEnumerable<T> otherSequence)
        {
            foreach(var item in otherSequence)
            {
                sequence.Add(item);
            }
            return sequence;
        }

        /// <summary>
        /// Adds the given value to the list unless it is null (in which casem this does nothing).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="value"></param>
        public static void AddIfNotNull<T>([Nullable] this ICollection<T> collection, T value)
        {
            if (value != null)
                collection.Add(value);
        }

        /// <summary>
        /// Returns a string containing a string representation of each item in the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="separator"></param>
        /// <returns>The string representation of the contents of the collection, or null if <paramref name="collection"/> is null.</returns>
        [return: Nullable]
        public static string CollectionToString<T>([Nullable] ICollection<T> collection, [NotNull] string separator = ", ")
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

        /// <summary>
        /// Returns the only element in a sequence, or null if the sequence is empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <returns>The only element, or null.</returns>
        /// <exception cref="InvalidOperationException">If there is more than one element in the sequence.</exception>
        [return: Nullable]
        public static T SingleOrDefault<T>([NotNull] this IEnumerable<T> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            if(!enumerator.MoveNext())
            {
                return default(T);               // no elements
            }
            else
            {
                var first = enumerator.Current;  // the first element
                if(enumerator.MoveNext())
                    throw new InvalidOperationException("Sequence has multiple elements");
                else
                    return first;    // this is the only element
            }
        }
    }
}
