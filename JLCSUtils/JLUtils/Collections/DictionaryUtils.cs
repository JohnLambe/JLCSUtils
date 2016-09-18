// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;

namespace JohnLambe.Util.Collections
{
    /// <summary>
    /// Utility and extension methods for working with Dictionary.
    /// </summary>
    public static class DictionaryUtils
    {
        /// <summary>
        /// Return the first key/value pair where the value is of exactly the given type (not just a subclass).
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="requestedType"></param>
        /// <returns></returns>
        public static KeyValuePair<K, V> GetKeyValueByExactType<K, V>(IDictionary<K, V> dictionary, Type requestedType)
        {
            foreach (var keyValue in dictionary)
            {
                if (keyValue.Value.GetType() == requestedType)
                    return keyValue;
            }
            return new KeyValuePair<K, V>(default(K), default(V));
        }

        /// <summary>
        /// Returns the value that the IDictionary.TryGetValue assigns to its 'out' parameter.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key">The key to look up. Must not be null.</param>
        /// <returns>The value corresponding to the requested key, or the default for the type if there is no corresponding value.</returns>
        public static V TryGetValue<K, V>(this IDictionary<K, V> dictionary, K key)
        {
            V value;
            dictionary.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// Delegate that takes a dictionary entry as two parameters.
        /// (Passing entries to this avoid the consumer having to cast them).
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        // It uses two separate parameters by System.Collections.DictionaryEntry has properties of type object
        // (and I didn't want the overhead of creating a new object).
        public delegate bool DictionaryEntryFilter<TKey, TValue>(TKey key, TValue value);

        /// <summary>
        /// Remove all entries for which the delegate returns true.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="filter"></param>
        /// <param name="dispose">Iff true, any <see cref="IDisposable"/> items removed are disposed.</param>
        public static void RemoveAllByKV<TKey, TValue>(this IDictionary dictionary, DictionaryEntryFilter<TKey, TValue> filter, bool dispose = false)
        {
            var toBeRemoved = new List<TKey>();     // for the list of items to be removed
            foreach (DictionaryEntry kv in dictionary)
            {
                if (filter((TKey)kv.Key, (TValue)kv.Value))
                {
                    toBeRemoved.Add((TKey)kv.Key);
                    if (dispose && kv.Value is IDisposable)
                        ((IDisposable)kv.Value).Dispose();
                }
            }
            foreach (var key in toBeRemoved)
            {
                dictionary.Remove(key);
            }
        }

        /// <summary>
        /// Add or replace a key in the dictionary.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            dictionary.Remove(key);
            dictionary.Add(key, value);
        }

    }

}