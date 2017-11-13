using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
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


    public static class LazyInitialize
    {
        /// <summary>
        /// Read the value of a lazily-initialized item.
        /// </summary>
        /// <typeparam name="T">The type of the lazily-initialized item.</typeparam>
        /// <param name="field">Field that stores the value when initialized.
        /// Must be default(<typeparamref name="T"/>) when not initialised.</param>
        /// <param name="createDelegate">Delegate to initialize the item.
        /// <paramref name="field"/> is set to the result of this if it is default(<typeparamref name="T"/>) initially.
        /// If this is null, <paramref name="field"/> will always be returned, even if null.
        /// </param>
        /// <returns>The value of <paramref name="field"/> (which is initialized on exit).
        /// This can be null if and only if <paramref name="createDelegate"/> returns null or is null.
        /// </returns>
        public static T GetValue<T>([Nullable] ref T field, [Nullable] Func<T> createDelegate)
        {
            if (field == null) // || field.Equals(default(T)))
            {
                if(createDelegate != null)
                    field = createDelegate();
            }
            return field;
        }
    }

}
