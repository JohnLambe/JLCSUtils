using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util
{

    public class LazyInitialzed<T>
    {
        public LazyInitialzed(Func<T> createDelegate)
        {
            _createDelegate = createDelegate;
        }

        public static implicit operator T(LazyInitialzed<T> value)
        {
            return value.GetValue();
        }

        public T GetValue()
        {
            if (_value == null)
                _value = _createDelegate();
            return _value;
        }

        protected Func<T> _createDelegate;
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
