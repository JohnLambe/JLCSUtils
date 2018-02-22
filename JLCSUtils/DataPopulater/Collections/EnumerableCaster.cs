using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Collections
{
    /*
    public class EnumerableCaster<TOut, TIn> : IEnumerable<TOut>
    {
        public EnumerableCaster(IEnumerable<TIn> inner)
        {
            _inner = inner;
        }

        protected IEnumerable<TIn> _inner;

        public virtual IEnumerator<TOut> GetEnumerator()
        {
            return new EnumeratorCaster(_inner.GetEnumerator<TIn>());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new EnumeratorCaster(_inner.GetEnumerator());
        }

        public class EnumeratorCaster<TOut,TIn> : IEnumerator<TOut>
        {
            public EnumeratorCaster(IEnumerator<TIn> inner)
            {

            }

            public virtual TOut Current
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public virtual void Dispose()
            {
                _inner.Dispose();
            }

            public virtual bool MoveNext()
            {
                return _inner.MoveNext();
            }

            public virtual void Reset()
            {
                _inner.Reset();
            }

            protected IEnumerator<TIn> _inner;
        }
    }
    */
}
