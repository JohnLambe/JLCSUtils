using JohnLambe.Util.Diagnostic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Collections
{
    /// <summary>
    /// Interface for a class that looks up a key to return a single value,
    /// without support for enumeration of the values.
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    public interface ISimpleLookup<K,V>
    {
        /// <summary>
        /// Get the value corresponding to the given key.
        /// </summary>
        /// <param name="key">The key to be looked up.</param>
        /// <returns>The value corresponding to the key <paramref name="key"/>.</returns>
        V this[K key] { get; }

        /// <summary>
        /// This behaves similary to <see cref="IDictionary{TKey, TValue}.TryGetValue(TKey, out TValue)"/>:
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the
        ///     key is found; otherwise, the default value for the type of the value parameter.
        ///     (Same as <see cref="IDictionary{TKey, TValue}"/>.)
        /// </param>
        /// <returns>true iff the value was found.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="key"/> is null.</exception>
        bool TryGetValue(K key, out V value);
    }

    /// <summary>
    /// Base class for <see cref="ISimpleLookup{K, V}"/> implementations
    /// that calculate and cache a value on the first time that its key is looked up.
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    public abstract class CachedSimpleLookupBase<K, V> : ISimpleLookup<K, V>
    {
        /// <summary>
        /// Get the value corresponding to the given key.
        /// </summary>
        /// <param name="key">The key to be looked up.</param>
        /// <returns>The value corresponding to the key <paramref name="key"/>.</returns>
        /// <remarks>This is not virtual. Override <see cref="TryGetValue(K, out V)"/> instead.</remarks>
        public V this[K key]
        {
            get
            {
                V value;
                if (TryGetValue(key, out value))
                    return value;
                else
                    throw new KeyNotFoundException("Key not found: " + key
                        + " (in " + ToString() + ")");
            }
        }

        /// <summary>
        /// Calculate the value for a given key.
        /// Multiple calls with the same key must yield the same result.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">The value corresponding to the given key.
        /// If false is returned, <paramref name="value"/> is ignored.
        /// (Setting it to default(<typeparamref name="V"/>) is recommended, and conventional for methods with a similar prototype to this.)
        /// </param>
        /// <returns>True if the value was successfully generated. False if there is no mapping for the given key.</returns>
        protected abstract bool Calculate(K key, out V value);

        /// <summary>
        /// Get the value corresponding to the given key.
        /// </summary>
        /// <param name="key">The key to be looked up.</param>
        /// <param name="value">The value corresponding to the key <paramref name="key"/>.
        /// default(<typeparamref name="V"/>) if not found.
        /// </param>
        /// <returns>true iff the value was found.</returns>
        /// <seealso cref="this[K]"/>
        public virtual bool TryGetValue(K key, out V value)
        {
            ObjectExtension.CheckArgNotNull(key, nameof(key));
            if (_cache.TryGetValue(key, out value))   
            {
                return true;   // found
            }
            else   // if not in cache
            {
                if (Calculate(key, out value))        // generate the value
                {
                    _cache[key] = value;              // cache it
                    return true;   // found
                }
                else
                {
                    value = default(V);               // Calculate should have set it to this anyway
                }
            }
            return false;                             // not found
        }

        /// <summary>
        /// Cache of values previously looked up.
        /// </summary>
        protected Dictionary<K, V> _cache = new Dictionary<K, V>();
    }

    /// <summary>
    /// <see cref="ISimpleLookup{K, V}"/> whose values are provided by a given
    /// function, and cached when first looked up.
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    public class CachedSimpleLookup<K, V> : CachedSimpleLookupBase<K, V>
    {
        public CachedSimpleLookup(LookupDelegate<K, V> func)
        {
            Diagnostics.PreCondition(func != null);
            _func = func;
        }

        public CachedSimpleLookup(Func<K,V> func)
        {
            Diagnostics.PreCondition(func != null);
            _func2 = func;
        }

        public CachedSimpleLookup(ISimpleLookup<K,V> underlyingLookup)
        {
            Diagnostics.PreCondition(underlyingLookup != null);
            _func2 = k => underlyingLookup[k];
        }

        protected override bool Calculate(K key, out V value)
        {
            if (_func2 != null)
            {
                value = _func2(key);
                return true;
            }
            else
            {
                return _func(key, out value);   // delegate to the function
            }
        }

        /// <summary>
        /// Underlying function, called the first time each key is looked up.
        /// </summary>
        protected readonly LookupDelegate<K, V> _func;
        protected readonly Func<K, V> _func2;
    }

    public delegate bool LookupDelegate<K, V>(K key, out V value);

    /*
        public class CachedFunction<K,V>
    {
        public Func<K,V> Create(Func<K,V> underlyingFunc)
        {
            return (k) => { return underlyingFunc(k); };
        }
    }
    */

    /*
    public abstract class LazilyPopulatedLookupBase<K, V> : ILookup<K, V>
    {
        protected abstract V Calculate(K key);

        protected ILookup<K, V> _lookup = new Lookup<K,V>();

        public IEnumerable<V> this[K key]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool Contains(K key)
        {
            return this[key] != null;
        }

        public IEnumerator<IGrouping<K, V>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }


    public class LazilyPopulatedLookup<K, V> : LazilyPopulatedLookupBase<K, V>
    {
        public LazilyPopulatedLookup(Func<K, V> populationDelegate)
        {

        }
*/

}
