using JohnLambe.Util.Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Text
{
    public class PreprocessComparer<T> : IComparer, IComparer<T>
        //IEqualityComparer, IEqualityComparer<T>
        where T : IComparable<T>
    {
        public PreprocessComparer(Func<T, T> preprocessDelegate, IComparer<T> comparer = null)
        {
            _preprocessDelegate = preprocessDelegate;
            _comparer = comparer ?? DefaultComparer<T>.Instance;
        }

        public virtual int Compare(object x, object y)
        {
            return Compare((T)x, (T)y);
        }

        public virtual int Compare(T x, T y)
        {
            x = _preprocessDelegate(x);
            y = _preprocessDelegate(y);
            return _comparer.Compare(x, y);
        }

        public virtual T Preprocess(T value)
        {
            return _preprocessDelegate(value);
        }

        /*
        bool IEqualityComparer.Equals(object x, object y)
        {
            throw new NotImplementedException();
        }

        public virtual int GetHashCode(object value)
        {
            return _preprocessDelegate(value)?.GetHashCode() ?? 0;
        }

        public virtual bool Equals(T x, T y)
        {
            return _comparer.Equals(_preprocessDelegate(x), _preprocessDelegate(y));
        }

        public int GetHashCode(T obj)
        {
            ;
        }
        */

        protected Func<T, T> _preprocessDelegate;
        protected IComparer<T> _comparer;
    }


    public class PreprocessStringComparer : PreprocessComparer<string>
    {
        public PreprocessStringComparer(Func<string, string> preprocessDelegate, IComparer<string> comparer)
            : base(preprocessDelegate,comparer)
        {
        }

        /*
        public PreprocessStringComparer(Func<string, string> preprocessDelegate, StringComparison comparison)
            : base(preprocessDelegate, ...)
        {
        }
        */

        public override int Compare(object x, object y)
        {
            return Compare(x.ToString(), y.ToString());
        }
    }
}