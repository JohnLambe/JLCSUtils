using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Collections
{
    public static class SortedListExtension
    {
        /// <summary>
        /// Remove the first instance of the given value from the list.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="list"></param>
        /// <param name="value">Value to remove.</param>
        /// <returns>true iff removed. False if not found.</returns>
        public static bool RemoveByValue<K, V>(this SortedList<K, V> list, V value)
        {
            int index = list.IndexOfValue(value);
            if (index == -1)
            {
                return false;
            }
            else
            {
                list.RemoveAt(index);
                return true;
            }
        }

        /// <summary>
        /// Returns the index of the given key if it exists,
        /// otherwise, the index of the first key higher than <paramref name="findKey"/>.
        /// If there is no key higher than it, the Count of the list is returned.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="list"></param>
        /// <param name="findKey"></param>
        /// <returns></returns>
        public static int NearestIndexToKey<K,V>(this SortedList<K,V> list, K findKey)
            where K: IComparable
        {
            return IndexOfFirstHigher(list.Keys, findKey);
        }
        /*
        public static int NearestIndexToKey<K, V>(this ISortedList<K, V> list, K findKey)
            where K : IComparable
        {
            return IndexOfFirstHigher(list.Keys, findKey);
        }
        */

        /// <summary>
        /// Returns the index of the item before the first item in the list with a value
        /// greater than or equal to the given value.
        /// otherwise, the index of the first item with a higher value than <paramref name="findKey"/>.
        /// If there is no item higher than it, the Count of the list is returned.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <param name="list"></param>
        /// <param name="findKey"></param>
        /// <returns></returns>
        public static int IndexOfFirstHigher<K>(IList<K> list, K findKey)
            where K : IComparable
        {
            int index = 0;
            foreach (var key in list)
            {
                int comparison = key.CompareTo(findKey);
                if (comparison <= 0)
                {
                    return index;
                }
                index++;
            }
            return index;
        }
    }
}
