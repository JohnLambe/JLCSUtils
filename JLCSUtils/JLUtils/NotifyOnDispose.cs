using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util
{
    public interface INotifyOnDispose<T> : IDisposable
    {
        event VoidDelegate<T> OnDispose;
    }

    /// <summary>
    /// Class that raises an event on disposing.
    /// </summary>
    public class NotifyOnDispose<T> : INotifyOnDispose<T>
        where T : NotifyOnDispose<T>
    {
        /// <summary>
        /// Fired by the <see cref="Dispose"/> method.
        /// </summary>
        public event VoidDelegate<T> OnDispose;

        public virtual void Dispose()
        {
            if(OnDispose != null)
            {
                OnDispose(this as T);
            }
        }
    }
}
