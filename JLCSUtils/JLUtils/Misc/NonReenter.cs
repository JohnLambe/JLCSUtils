using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    /// <summary>
    /// Encapsulates the logic to prevent re-entry of code within the same thread.
    /// Not thread-safe.
    /// </summary>
    public class NonReenter
    {
        public virtual TReturn Execute<TReturn>(Func<TReturn> del, TReturn defaultValue = default(TReturn))
        {
            TReturn value = defaultValue;
            if (Enter())
            {
                try
                {
                    value = del.Invoke();
                }
                finally
                {
                    Exit();
                }
            }
            return value;
        }

        public virtual void Execute(VoidDelegate del)
        {
            if (Enter())
            {
                try
                {
                    del.Invoke();
                }
                finally
                {
                    Exit();
                }
            }
        }

        protected virtual bool Enter()
        {
            if (_entered)
            {
                if (ExceptionOnReenter)
                    throw new InvalidOperationException("Invalid recursion");
                return false;
            }
            else
            {
                _entered = true;
                return true;
            }
        }

        public virtual bool Exit()
        {
            bool value = _entered;
            _entered = false;
            return value;
        }

        /// <summary>
        /// true to cause an <see cref="InvalidOperationException"/> to be thrown on an attempt to re-enter.
        /// </summary>
        public virtual bool ExceptionOnReenter { get; set; }

        protected bool _entered = false;

        public static TReturn DoExecute<TReturn>(Func<TReturn> del, TReturn defaultValue)
        {
            return new NonReenter()
                .Execute<TReturn>(del, defaultValue);
        }

        public static TReturn DoExecute<TReturn>(Func<TReturn> del)
        {
            return new NonReenter() {  ExceptionOnReenter = true }
                .Execute<TReturn>(del);
        }

        public static void DoExecute(VoidDelegate del, bool exceptionOnReenter = false)
        {
            new NonReenter() { ExceptionOnReenter = exceptionOnReenter }
                .Execute(del);
        }
    }
}
