using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Misc
{
    public class DefaultComparer<T> : IComparer, IComparer<T>
        where T : IComparable<T>
    {
        public virtual int Compare(T x, T y)
        {
            if (x == null && y == null)
                return 0;
            else if (x == null)
                return -1;
            else if (y == null)
                return 1;
            else
                return x.CompareTo(y);
        }

        public virtual int Compare(object x, object y)
        {
            if (x == y)     // both null or same instance, even if they don't implement IComparable
                return 0;
            //if (x == null && y == null)
            //    return 0;
            else if (x == null)   // x == null && y != null (because x != y)
                return -1;
            else if (y == null)   // y == null && x != null (because x != y)
                return 1;
            else if (x is IComparable)
                return ((IComparable)x).CompareTo(y);
            else if (y is IComparable)
                return -((IComparable)y).CompareTo(x);
            else
                throw new InvalidOperationException("Can't compare " + x.GetType() + " and " + y.GetType()); // x and y are not null (already checked)
        }

        public static DefaultComparer<T> Instance { get; } = new DefaultComparer<T>();
    }
}
