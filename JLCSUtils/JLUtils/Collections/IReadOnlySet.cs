using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Collections
{
    /// <summary>
    /// Base class for sets, implementing only the members needed for a read-only set.
    /// Subclasses may or may not be read-only.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SetBaseReadOnly<T> : ISetBase<T>
    {
        public virtual int Count
        {
            get { return EnumeratorUtil.Count(GetEnumerator()); }
            // inefficient. Subclasses should implement a more efficient method where possible.
        }

        public virtual bool Contains(T item)
        {
            foreach (var thisItem in this)
                if (thisItem.Equals(item))
                    return true;
            return false;
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            foreach(var item in this)
            {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return IsSupersetOf(other) && other.Any();
            // sub-optimal if Count is expensive
        }

        public virtual bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return IsSupersetOf(other) && other.Any();
            // sub-optimal if Count is expensive
        }

        public virtual bool IsSubsetOf(IEnumerable<T> other)
        {
            foreach (var item in other)
                if (!Contains(item))
                    return false;     // found an item in `other` that's not in this 
            return true;
        }

        public virtual bool IsSupersetOf(IEnumerable<T> other)
        {
            foreach (var item in this)
                if (other.Contains(item))
                    return false;     // found an item in this that's not in `other`
            return true;
        }

        public virtual bool Overlaps(IEnumerable<T> other)
        {
            foreach (var item in other)
                if (Contains(item))
                    return true;     // found a common item 
            return false;
        }

        public virtual bool SetEquals(IEnumerable<T> other)
        {
            return IsSubsetOf(other) && IsSupersetOf(other);
            /*
            foreach (var item in other)
                if (!Contains(item))
                    return false;     // found a difference
            foreach(var item in this)
                if (!other.Contains(item))
                    return false;     // found a difference
            return true;    // no difference found
            */
        }
    }

    public abstract class SetBase<T> : SetBaseReadOnly<T>, ISet<T>
    {
        public virtual bool IsReadOnly
            => true;

        public virtual bool Add(T item)
        {
            throw new NotSupportedException();
        }

        public virtual void Clear()
        {
            throw new NotSupportedException();
        }

        public virtual void ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public virtual void IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public virtual bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public virtual void UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        public virtual void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Set which may or may not be writeable.
    /// </summary>
    public interface ISetBase<T>
    {
        int Count { get; }

        bool Contains(T item);

        void CopyTo(T[] array, int arrayIndex);

        IEnumerator<T> GetEnumerator();

        bool IsProperSubsetOf(IEnumerable<T> other);

        bool IsProperSupersetOf(IEnumerable<T> other);

        bool IsSubsetOf(IEnumerable<T> other);

        bool IsSupersetOf(IEnumerable<T> other);

        bool Overlaps(IEnumerable<T> other);

        bool SetEquals(IEnumerable<T> other);

//        IEnumerator IEnumerable.GetEnumerator();
    }


    public interface IReadOnlySet<T> : ISetBase<T>
    {
    }
}

