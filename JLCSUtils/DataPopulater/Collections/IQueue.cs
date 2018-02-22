// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// An interface for a generic queue, and a class that implements it by subclassing System.Collections.Generic.Queue<T>.

namespace JohnLambe.Util.Collections
{
    /// <summary>
    /// An interface for a generic queue.
    /// </summary>
    /// <typeparam name="T">The type of item that may be contained in the queue.</typeparam>
    public interface IQueue<T>
    {
        //
        // Summary:
        //     Gets the number of elements contained in the System.Collections.Generic.Queue`1.
        //
        // Returns:
        //     The number of elements contained in the System.Collections.Generic.Queue`1.
        //public int Count { get; }

        /// <summary>
        /// Removes all objects from the queue.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether an element is in the queue.
        /// </summary>
        /// <param name="item">The object to locate in the queue. The value can
        ///     be null for reference types.</param>
        /// <returns>true if item is found in the queue; otherwise, false.</returns>
        bool Contains(T item);

        //
        // Summary:
        //     Copies the System.Collections.Generic.Queue`1 elements to an existing one-dimensional
        //     System.Array, starting at the specified array index.
        //
        // Parameters:
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements copied
        //     from System.Collections.Generic.Queue`1. The System.Array must have zero-based
        //     indexing.
        //
        //   arrayIndex:
        //     The zero-based index in array at which copying begins.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     array is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     arrayIndex is less than zero.
        //
        //   T:System.ArgumentException:
        //     The number of elements in the source System.Collections.Generic.Queue`1 is greater
        //     than the available space from arrayIndex to the end of the destination array.
        void CopyTo(T[] array, int arrayIndex);

        //
        // Summary:
        //     Removes and returns the object at the beginning of the System.Collections.Generic.Queue`1.
        //
        // Returns:
        //     The object that is removed from the beginning of the System.Collections.Generic.Queue`1.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The System.Collections.Generic.Queue`1 is empty.
        T Dequeue();

        //
        // Summary:
        //     Adds an object to the end of the System.Collections.Generic.Queue`1.
        //
        // Parameters:
        //   item:
        //     The object to add to the System.Collections.Generic.Queue`1. The value can be
        //     null for reference types.
        void Enqueue(T item);

        //
        // Summary:
        //     Returns the object at the beginning of the System.Collections.Generic.Queue`1
        //     without removing it.
        //
        // Returns:
        //     The object at the beginning of the System.Collections.Generic.Queue`1.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The System.Collections.Generic.Queue`1 is empty.
        T Peek();
    }

    /// <summary>
    /// Queue that implements <see cref="IQueue{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueImpl<T> : Queue<T>, IQueue<T>
    {
    }
}
