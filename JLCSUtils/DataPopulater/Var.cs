using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnLambe.Util
{
    public class VarBase<T>
    {
        public virtual T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Cast this to the wrapped value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator T(VarBase<T> value)
        {
            return value.Value;
        }

        /// <summary>
        /// The variable wrapped by this object.
        /// All access to this must be done while a lock on _lockObject is held.
        /// </summary>
        protected T _value;
    }

    public class VarBaseExplicitCast<T> : VarBase<T>
    {
        public VarBaseExplicitCast()
        {
        }

        public VarBaseExplicitCast(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Cast a value to this.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static explicit operator VarBaseExplicitCast<T>(T value)
        {
            return new VarBaseExplicitCast<T>(value);
        }
    }

    public class Var<T> : VarBase<T>
    {
        public Var()
        {
        }

        public Var(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Cast a value to this.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Var<T>(T value)
        {
            return new Var<T>(value);
        }
    }
}
