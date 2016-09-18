using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// The contract of IList and IList<T> indicates that they may or may not be modifiable (certain methods are
// specified as possibly throwing an exception on an attempt to change the data).
// The interfaces below can be used to indicate that a list is modifiable, or that a modifiable list is required.

namespace JohnLambe.Util.Collections
{
    /// <summary>
    /// A list which can be modified.
    /// <para>This is the same as <see cref="System.Collections.IList"/> except that it must not
    /// throw <see cref="System.NotSupportedException"/> on attempts to modify it,
    /// unless there is a reason other than it not being modifiable, e.g. trying to make
    /// the list longer than a maximum supported size.
    /// </para>
    /// </summary>
    public interface IMutableList : System.Collections.IList
    {
    }

    /// <summary>
    /// A list which can be modified.
    /// <para>This is the same as <see cref="System.Collections.Generic.IList<T>"/> except that it must not
    /// throw <see cref="System.NotSupportedException"/> on attempts to modify it,
    /// unless there is a reason other than it not being modifiable, e.g. trying to make
    /// the list longer than a maximum supported size.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public interface IMutableList<T> : System.Collections.Generic.IList<T>, System.Collections.Generic.IReadOnlyList<T>
    {
    }
}
