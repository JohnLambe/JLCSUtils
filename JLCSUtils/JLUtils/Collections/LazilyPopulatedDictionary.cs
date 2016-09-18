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
        public virtual V this[K key]
        {
            get
            {
                V value;
                if (!_cache.TryGetValue(key, out value))   // if not in cache
                {
                    value = Calculate(key);                // generate the value
                    _cache[key] = value;                   // cache it
                }
                return value;                              // return value (either from cache or calculated on this call)
            }
        }

        /// <summary>
        /// Calculate the value for a given key.
        /// Multiple calls with the same key must yield the same result.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The value corresponding to the given key.</returns>
        protected abstract V Calculate(K key);

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
        public CachedSimpleLookup(Func<K,V> func)
        {
            Contract.Requires(func != null);
            _func = func;
        }

        protected override V Calculate(K key)
        {
            return _func(key);   // delegate to the function
        }

        /// <summary>
        /// Underlying function, called the first time each key is looked up.
        /// </summary>
        protected readonly Func<K, V> _func;
    }

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
