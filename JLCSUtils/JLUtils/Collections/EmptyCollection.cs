using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Collections
{
    /// <summary>
    /// An empty read-only (immutable) list / collection.
    /// (Can be used as a null object for any of the implemented interfaces).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EmptyReadOnlyCollection<T> : System.Collections.IEnumerable, IEnumerable<T>, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        #region IEnumerable members

        public IEnumerator<T> GetEnumerator()
        {
            return _emptyEnumerator;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _emptyEnumerator;
        }

        #endregion

        public T this[int index]
        {
            get { throw new IndexOutOfRangeException(); }    // it's always out of range since the collection is empty
        }

        /// <summary>
        /// The size of the collection - always 0.
        /// </summary>
        public int Count => 0;

        /// <summary>
        /// Enumerator over no items.
        /// </summary>
        protected static readonly IEnumerator<T> _emptyEnumerator = new EmptyEnumerator<T>();

        /// <summary>
        /// A singleton empty collection of <typeparamref nanme="T"/>.
        /// Any instance created will always behave the same as this.
        /// </summary>
        public static EmptyCollection<T> Instance = new EmptyCollection<T>();

        /// <summary>
        /// An empty array of <typeparamref name="T"/>.
        /// </summary>
        public static readonly T[] EmptyArray = new T[] { };
    }

    /// <summary>
    /// An empty read-only collection that implements interfaces
    /// with methods for modifying a collection (calling them fails).
    /// <para>These interfaces allow read-only collections, and have an IsReadOnly property.</para>
    /// <para>Methods of the interfaces that can potentially modify a collection (could modify it if it was not read-only)
    /// are implemented explicitly (so that consumers that don't use that interface don't see them - they're there only
    /// to make it compatible with something that expects that interface).
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EmptyCollection<T> : EmptyReadOnlyCollection<T>,
            System.Collections.IList, IList<T>, ICollection<T>
    {
        #region IList

        object IList.this[int index]
        {
            get
            {
                throw new IndexOutOfRangeException();
            }
            set
            {
                throw new IndexOutOfRangeException();
            }
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        bool IList.Contains(object value) => false;

        void IList.Clear()
        {   // it's already empty
        }

        int IList.IndexOf(object value) => -1;

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
        }

        void IList.RemoveAt(int index)
        {
            throw new IndexOutOfRangeException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
        }

        #endregion

        #region IList<T>

        public bool IsReadOnly => true;

        public bool IsFixedSize => true;

        /// <summary>
        /// Returns a new object on every call, so that synchronization does
        /// nothing (and therefore cannot cause a deadlock),
        /// since this is thread-safe anyway.
        /// <para>This is so that the collection can be shared without restriction,
        /// with different consumers (in potentially different threads) seeing it for a
        /// different purpose.
        /// </para>
        /// </summary>
        object ICollection.SyncRoot => new object();
        // Alternatively, we could return this.

        /// <summary>
        /// Always returns true - this is thread-safe because it is immutable.
        /// </summary>
        bool ICollection.IsSynchronized => true;

        T IList<T>.this[int index]
        {
            get
            {
                throw new IndexOutOfRangeException();
            }
            set
            {
                throw new IndexOutOfRangeException();
            }
        }

        int IList<T>.IndexOf(T item)
        {
            return -1;  // whatever it is, it can't be in the list
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new IndexOutOfRangeException();  // no index is valid
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Contains(T item) => false;    // it's not in the list (because it's empty)

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {   // do nothing - there's nothing to copy.
        }

        bool ICollection<T>.Remove(T item)
        {
            return false;    // it's not in the list (because it's empty)
        }

        void ICollection<T>.Clear()
        {   // it's already empty
        }

        #endregion
    }

    /// <summary>
    /// Non-generic empty collection.
    /// </summary>
    public static class EmptyCollection
    {
        /// <summary>
        /// Non-generic enumerator over no items.
        /// </summary>
        public static System.Collections.IEnumerator EmptyEnumerator { get; }
            = EmptyCollection<object>.EmptyArray.GetEnumerator();
    }

    /// <summary>
    /// Empty Lookup.
    /// (Can be used as a null object for ILookup&lt;K,V&gt;, IEnumerable&lt;V&gt; and IReadOnlyList&lt;V&gt;).
    /// </summary>
    /// <typeparam name="K">Key type.</typeparam>
    /// <typeparam name="V">Value type.</typeparam>
    public class EmptyLookup<K, V> : EmptyReadOnlyCollection<V>, ILookup<K, V>
    {
        public bool Contains(K key)
        {
            return false;
        }

        IEnumerable<V> ILookup<K, V>.this[K key]
        {
            get { return this; }
        }

        IEnumerator<IGrouping<K, V>> IEnumerable<IGrouping<K, V>>.GetEnumerator()
        {
            return _emptyLookupEnumerator;
        }

        static protected IEnumerator<IGrouping<K, V>> _emptyLookupEnumerator = new EmptyEnumerator<IGrouping<K, V>>();
    }

    public class EmptyDictionary<K, V> : EmptyReadOnlyCollection<V>, IReadOnlyDictionary<K, V>
    {
        #region IReadOnlyDictionary<K, V>

        public bool ContainsKey(K key)
        {
            return false;
        }

        public IEnumerable<K> Keys
        {
            get { return _emptyKeyEnumerator; }
        }

        public bool TryGetValue(K key, out V value)
        {
            if (key == null)
                throw new ArgumentNullException();
            value = default(V);
            return false;
        }

        IEnumerable<V> IReadOnlyDictionary<K, V>.Values
        {
            get { return this; }
            // EmptyCollection implements this
        }

        V IReadOnlyDictionary<K, V>.this[K key]
        {
            get
            {
                return SimulateLookup(key);
            }
        }

        protected virtual V SimulateLookup(K key)
        {
            if (key == null)
                throw new ArgumentNullException();
            else
                throw new KeyNotFoundException();
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
        {
            return _emptyKeyValuePairEnumerator;
        }

        #endregion

        static private IEnumerable<K> _emptyKeyEnumerator = new EmptyCollection<K>();
        static private IEnumerator<KeyValuePair<K, V>> _emptyKeyValuePairEnumerator = new EmptyEnumerator<KeyValuePair<K, V>>();
    }

    /// <summary>
    /// Enumerator over an empty collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EmptyEnumerator<T> : IEnumerator<T>
    {
        public T Current
        {
            get { return default(T); }
            //TODO: What do other implementations do when empty
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }
    }
}
