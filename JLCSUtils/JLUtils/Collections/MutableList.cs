using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Collections
{
    /// <summary>
    /// The same as <see cref="System.Collections.ArrayList"/> except that it implements
    /// <see cref="IMutableList"/>.
    /// <para>Subclasses must not be read-only.</para>
    /// </summary>
    public class MutableArrayList : System.Collections.ArrayList, IMutableList
    {
    }

    /// <summary>
    /// The same as <see cref="System.Collections.Generic.List<T>"/> except that it implements
    /// <see cref="IMutableList<T>"/>.
    /// <para>Subclasses must not be read-only.</para>
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class MutableArrayList<T> : System.Collections.Generic.List<T>, IMutableList<T>
    {
    }

}
