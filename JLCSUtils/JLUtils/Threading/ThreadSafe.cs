using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace JohnLambe.Util.Threading
{

    /// <summary>
    /// Wraps a variable and acquires a lock on all accesses to it, to make it thread-safe.
    /// If T is a class type, only the reading and assignment of the object reference are thread-safe
    /// (not changing the state of the object itself).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThreadSafe<T> : VarBase<T>
    {
        /// <summary>
        /// Initialises to the default value for T.
        /// </summary>
        public ThreadSafe()
        {
        }

        /// <summary>
        /// Assigns an initial value.
        /// </summary>
        /// <param name="value">the initial value</param>
        public ThreadSafe(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Accesses the wrapped value thread-safely.
        /// </summary>
        // Casting can be used instead of this.
        public override T Value
        {
            get
            {
                T result;
                lock (this)
                {
                    result = _value;
                }
                return result;
            }
            set
            {
                lock (this)
                {
                    _value = value;
                }
            }
        }

        /// <summary>
        /// Iff the value is currentValue, set it to newValue, otherwise leave it unchanged.
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="newValue"></param>
        /// <returns>true iff the value was changed.</returns>
        public virtual bool ConditionalUpdate(T currentValue, T newValue)
        {
            lock (this)
            {
                if (_value.Equals(currentValue))
                {
                    _value = newValue;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Iff the value is currentValue, set it to newValue, otherwise leave it unchanged.
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="newValue"></param>
        /// <param name="readValue">The current value.</param>
        /// <returns>true iff the value was changed.</returns>
        public virtual bool ConditionalUpdate(T currentValue, T newValue, out T readValue)
        {
            lock (this)
            {
                readValue = _value;
                if (_value.Equals(currentValue))
                {
                    _value = newValue;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Set the value and return what it was before updating it.
        /// </summary>
        /// <param name="newValue">The value to assign this to.</param>
        /// <returns>The old value.</returns>
        public virtual T ReadAndUpdate(T newValue)
        {
            lock (this)
            {
                T result = _value;
                _value = newValue;
                return result;
            }
        }

        /// <summary>
        /// Wait until this has a specified value.
        /// </summary>
        /// <param name="waitValue">The value to wait for.</param>
        /// <param name="maxWait">Maximum time to wait.</param>
        /// <param name="checkInterval">How frequently to check, in milliseconds.</param>
        /// <returns>Value (the value of the Value property) on exit.</returns>
        public virtual T WaitForValue(T waitValue, int maxWait =-1, int checkInterval = 10)
        {
            const long TicksToMilliseconds = 10000;
                // 1 tick = 100 ns = 0.1 us = 1e-4 ms

            T value;   // last value read

            long endTimeTicks = DateTime.Now.Ticks + maxWait * TicksToMilliseconds;
            do
            {
                Thread.Sleep(checkInterval);
                value = Value;  // thread-safe read
            } while (!value.Equals(waitValue) || ((maxWait >= 0) && (DateTime.Now.Ticks > endTimeTicks)));

            return value;
        }

        /// <summary>
        /// Wait until this has a specified value.
        /// </summary>
        /// <param name="waitValue">The value to wait for.</param>
        /// <param name="readValue">Value (the value of the Value property) on exit.</param>
        /// <param name="maxWait">Maximum time to wait.</param>
        /// <param name="checkInterval">How frequently to check, in milliseconds.</param>
        /// <returns>true iff Value is the specified value on exit.</returns>
        public virtual bool WaitForValue(T waitValue, out T readValue, int maxWait = -1, int checkInterval = 10)
        {
            readValue = WaitForValue(waitValue,maxWait,checkInterval);
            return readValue.Equals(waitValue);
        }

/*
        /// <summary>
        /// Used for synchronisation of access to _value
        /// (because T may be (and probably is) a non-class type).
        /// </summary>
        protected object _lockObject = new object();
*/

        /* Allowing casting to this type might be error-prone:
         * Assigning an instance of this is NOT thread safe (only assigning its Value property is).
         * Having to explicitly create a new instance makes it clearer to the reader.
         */
    }

    public class ThreadSafeExt<T> : ThreadSafe<T>
    {
        #region Constructors
        // same constructors as base class:

        /// <summary>
        /// Initialises to the default value for T.
        /// </summary>
        public ThreadSafeExt()
        {
        }

        /// <summary>
        /// Assigns an initial value.
        /// </summary>
        /// <param name="value">the initial value</param>
        public ThreadSafeExt(T value)
            : base(value)
        {
        }

        #endregion

        /// <summary>
        /// Read/Write the value without requiring a lock.
        /// This can be used while the caller has a lock on this object (not _value),
        /// thus allowing more complex logic while locked than the methods of this class provide.
        /// </summary>
        /// <returns></returns>
        public virtual T UnsafeValue
        {
            get { return _value; }
            set { _value = value; }
        }
    }

}
