using JohnLambe.Util.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Collections
{
    /// <summary>
    /// Holds mappings where the domain of the key forms a heirarchy and mappings can be defined
    /// at different levels of the heirarchy,
    /// and when looking up a key, the match at the lowest level (most specific) is returned.
    /// <para>One only mapping per key can be defined. A value can have multiple keys mapped to it.</para>
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public abstract class HMap<K, V> : ISimpleLookup<K, V>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="key2"></param>
        /// <returns>&gt;0 if a <paramref name="key2"/> </returns>
        protected abstract int IsMoreSpecific(K key1, K key2);

        protected abstract bool Matches(K key1, K key2);

        #region ISimpleLookup

        public virtual V this[K key]
        {
            get
            {
                V value;
                if (TryGetValue(key, out value))
                    return value;
                else
                    throw new KeyNotFoundException();
            }
        }

        public virtual bool TryGetValue(K key, out V value)
        {
            KeyValuePair<K, V> bestMatch = new KeyValuePair<K, V>();
            bool match = false;
            var ambiguousMatches = new LinkedList<KeyValuePair<K, V>>();

            foreach (var mapping in _map)
            {
                if (mapping.Key.Equals(key))            // exact match
                {
                    value = mapping.Value;                    // stop searching
                    return true;
                }
                
                if (Matches(mapping.Key, key))
                    // mapping.Key.IsAssignableFrom(key)
                {
                    match = true;       // match found, searching for a better one
                    int moreSpecific = IsMoreSpecific(mapping.Key, bestMatch.Key);
                    if (moreSpecific > 0)
                    {
                        bestMatch = mapping;
                        ambiguousMatches.Clear();   // no ambiguous match yet
                    }
                    else if (moreSpecific == 0)
                    {
                        ambiguousMatches.AddLast(mapping);    // match is currently ambiguous, but there might be a better one.
                    }
                }
            }

            if (ambiguousMatches.Any())
            {
                throw new AmbiguousMatchException("Ambiguous match for handler for " + key /*.Name*/
                    + ": " + CollectionUtil.CollectionToString(ambiguousMatches)  //TODO: format in string?
                    );
            }

            if (match)
            {
                value = bestMatch.Value;
            }
            else
            {
                value = default(V);
            }
            return match;
        }

        #endregion

        public virtual void Add(K key, V value)
        {
            _map[key] = value;
        }

        public virtual void Remove(K key)
        {
            _map.Remove(key);
        }

        /// <summary>
        /// Remove all mappings to the given value.
        /// </summary>
        /// <param name="value"></param>
        public virtual void RemoveByValue(V value)
        {
            var mappings = _map.Where(m => m.Value.Equals(value));
            foreach (var mapping in _map)
            {
                _map.Remove(mapping);
            }
        }

        protected IDictionary<K, V> _map = new Dictionary<K,V>();
    }


    public class TypeMap : HMap<Type, Type>
    {
        protected override int IsMoreSpecific(Type key1, Type key2)
        {
            return TypeUtil.IsMoreSpecific(key1,key2);
        }

        protected override bool Matches(Type key1, Type key2)
        {
            return key1.IsAssignableFrom(key2);
        }
    }
}
