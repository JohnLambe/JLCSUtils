using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util
{
    /// <summary>
    /// Wrapper for a value and a delegate, to lazy-initialise it.
    /// The delegate is fired on the first attempt to read the value.
    /// This class is implicitly castable to the type of the value, so can be used in place of the value.
    /// </summary>
    /// <typeparam name="T">The type of the lazily initialised value.</typeparam>
    public class LazyInitialized<T>
    {
        public LazyInitialized(Func<T> createDelegate)
        {
            Contract.Requires(createDelegate != null);
            _createDelegate = createDelegate;
        }

        public static implicit operator T(LazyInitialized<T> value)
        {
            return value.GetValue();
        }

        /// <summary>
        /// Get the value, initialising it if it is not already initialised.
        /// </summary>
        /// <returns></returns>
        public virtual T GetValue()
        {
            if (_value == null)
                _value = _createDelegate();
            return _value;
        }

        /// <summary>
        /// Delegate to return the value when first needed.
        /// </summary>
        protected Func<T> _createDelegate;

        /// <summary>
        /// Value, assigned when first needed.
        /// null if not initialised.
        /// </summary>
        protected T _value;
    }

    public class LazyInitialize
    {
        public static T GetValue<T>(ref T field, Func<T> createDelegate)
        {
            if (field == null)
                field = createDelegate();
            return field;
        }
    }

}
