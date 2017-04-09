using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    /// <summary>
    /// Class that supports consumers explictly manipulating a reference count, so that the object is disposed
    /// when the reference count reaches 0.
    /// </summary>
    public class ReferenceCountedObject : IDisposable
    {
        public virtual void AddRef()
        {
            if (IsDisposed)             
                throw new ObjectDisposedException(nameof(ReferenceCountedObject) + " is disposed: " + ToString());
            // If calling this after it is disposed, the caller is likely to try to use it.
            // (Hence, it's worse than an extra call to ReleaseRef (which means that something is wrong, but the caller won't be trying to use this)
            // or disposing when there are still references (callers might no longer hold references).

            RefCount++;
        }

        public virtual void ReleaseRef()
        {
            Debug.Assert(!IsDisposed, nameof(ReferenceCountedObject) + "." + nameof(ReleaseRef) + ": Already disposed: " + ToString());

            RefCount--;
            if (RefCount == 0)           // if the last reference was released
            {
                ((IDisposable)this).Dispose();
            }
            else
            {
                Debug.Assert(RefCount > 0, nameof(ReferenceCountedObject) + " released too many times (" + RefCount + "): " + ToString());
                // This shouldn't happen since it should have been disposed in this case, and the earlier assertion should have failed.
            }
        }

        /// <summary>
        /// The number of explicitly-counted references to this object.
        /// </summary>
        protected int RefCount { get; private set; } = 1;  // initialised to 1 - whatever created the instance is assumed to have a reference.

        #region IDisposable Support

        // Override a finalizer in a subclass only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ReferenceCountedObject() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        protected virtual void Dispose(bool disposing)
        {
            //if (!IsDisposed)
            {
                // Could throw an exception if RefCount > 0 since that would suggest that
                // either a consumer of this still references it (and may expect it to not be disposed)
                // or a consumer of this did not release it before its reference went out of scope.
                Debug.Assert(RefCount == 0, "ReferenceCountedObject: Bad reference count (" + RefCount + "): " + ToString());
                // Other assertions should fail before RefCount can become negative.

                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                IsDisposed = true;

                // Do this in subclass if the finalizer is overridden:
                //   GC.SuppressFinalize(this);    // prevents the finalizer from running.
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        /// <summary>
        /// True if this object has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }  // this is also used to prevent the disposing code from running twice if Dispose is called twice.
    }

}
