// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using JohnLambe.Util.TypeConversion;
using JohnLambe.Util.Types;
using System;
using System.Collections;
using System.Collections.Generic;

namespace JohnLambe.Util.Collections
{
    /// <summary>
    /// Utility and extension methods for working with Dictionary.
    /// </summary>
    public static class DictionaryUtil
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
        /// Returns the value that <see cref="IDictionary{K,V}.TryGetValue"/> assigns to its 'out' parameter.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key">The key to look up. Must not be null.</param>
        /// <returns>The value corresponding to the requested key, or the default for the type if there is no corresponding value.</returns>
        public static V TryGetValue<K, V>([NotNull] this IDictionary<K, V> dictionary, [NotNull] K key)
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

        /// <summary>
        /// Import an array of string in '&lt;key&gt; "=" &lt;value&gt;' format to the dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to add the parsed items into.</param>
        /// <param name="data">The input data.</param>
        /// <param name="prefix">Only keys beginning with this value are imported.</param>
        /// <param name="separator">The character that separates the key from the value.</param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ImportText<TKey,TValue>(IDictionary<TKey, TValue> dictionary, string[] data, string prefix = null, char separator = '=')
        {
            if (prefix == null)
                prefix = "";

            foreach (var record in data)
            {
                string key, value;
                record.SplitWholeToVars(separator, out key, out value);  // parse as '<key> "=" <value>'
                if (key.StartsWith(prefix))
                {
                    dictionary.Add(GeneralTypeConverter.Convert<TKey>(key), GeneralTypeConverter.Convert<TValue>(value));
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Get the value for a given key, and create a value for that key if it does not exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key">The key to try to get.</param>
        /// <param name="create">Delegate to return the value for <paramref name="key"/> if it does not exist.</param>
        /// <returns></returns>
        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey,TValue> dictionary, TKey key, Func<TValue> create)
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                value = create.Invoke();
                dictionary.Add(key, value);
            }
            return value;
        }

    }

}