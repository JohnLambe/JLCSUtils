using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Types
{
    /// <summary>
    /// <see cref="EventArgs"/> subclass that adds one property.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <remarks>This is for use in common cases, where one argument is provided, but the arguments can be extended in future versions (by creating (non-generic) subclasses of this for individual cases).</remarks>
    //| There are no versions with multiple arguments, because, in such cases, the arguments should have meaningful property names.
    public class OneArgumentEventArgs<T> : EventArgs
    {
        public virtual T Argument { get; set; }
    }
}
