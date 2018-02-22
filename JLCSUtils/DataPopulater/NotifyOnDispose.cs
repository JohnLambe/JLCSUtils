using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util
{
    /// <summary>
    /// Interface for a class that fires an event when it is disposed.
    /// </summary>
    /// <typeparam name="T">The type which implements the interface or a type that it is assignable to (it could be an interface or superclass of that type).</typeparam>
    [Obsolete]
    public interface INotifyOnDispose<T> : IDisposable
    {
        /// <summary>
        /// Fired when the object is disposed.
        /// </summary>
        event VoidDelegate<T> OnDispose;
    }

    // Non-generic version:

    /// <summary>
    /// Interface for a class that fires an event when it is disposed.
    /// </summary>
    public interface INotifyOnDispose : IDisposable
    {
        /// <summary>
        /// Fired when the object is disposed.
        /// </summary>
        event EventHandler Disposed;
    }


    /// <summary>
    /// Class that raises an event on disposing.
    /// </summary>
    /// <typeparam name="T">The type provided on the event. This must be a type that this class (or the subclass of it being used) is assignable to.</typeparam>
    [Obsolete]
    public class NotifyOnDispose<T> : INotifyOnDispose<T>
        where T : class
        //| if T should be this, the constraint `T: NotifyOnDispose<T>` could be used (and would be checked at compile time), but that would not allow using an interface implemented by a subclass.
    {
        public NotifyOnDispose()
        {
            Contract.Requires(typeof(T).IsAssignableFrom(GetType()), "The type parameter of NotifyOnDispose<T> must be assignable from the this type");
        }

        /// <summary>
        /// Fired by the <see cref="Dispose"/> method.
        /// </summary>
        public event VoidDelegate<T> OnDispose;

        public virtual void Dispose()
        {
            OnDispose?.Invoke(this as T);
        }
    }

    /// <summary>
    /// Class that raises an event on disposing.
    /// </summary>
    public class NotifyOnDispose : INotifyOnDispose
    {
        /// <summary>
        /// Fired by the <see cref="Dispose"/> method.
        /// </summary>
        public event EventHandler Disposed;

        public virtual void Dispose()
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }

}
