using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Types
{
    /// <summary>
    /// <see cref="EventArgs"/> subclass that adds one property. (Subclasses of this may have more.)
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <remarks>This is for use in common cases, where one argument is provided, but the arguments can be extended in future versions (by creating (non-generic) subclasses of this for individual cases).</remarks>
    //| There are no versions with multiple arguments, because, in such cases, the arguments should have meaningful property names.
    public class OneArgumentEventArgs<T> : EventArgs
    {
        public virtual T Argument { get; set; }
    }

    /// <summary>
    /// Event arguments that can be cast to and from the type of the <see cref="OneArgumentEventArgs{T}.Argument"/> property.
    /// Can be used for events that currently have only one argument, so that this can be used as if it was the one argument,
    /// but can be extended in future versions.
    /// </summary>
    /// <typeparam name="T">The type of the property of the event (and the type this can be cast to/from).</typeparam>
    public class CastableEventArgs<T> : OneArgumentEventArgs<T>
    {
        public static implicit operator T(CastableEventArgs<T> value)
        {
            return value.Argument;
        }

        public static implicit operator CastableEventArgs<T>(T value)
        {
            return new CastableEventArgs<T>() { Argument = value };
        }
    }
}
