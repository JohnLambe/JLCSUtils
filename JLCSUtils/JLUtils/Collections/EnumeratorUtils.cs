using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Collections
{
    public static class EnumeratorUtils
    {
        /// <summary>
        /// Returns the number of items remaining in the enumerator
        /// and leaves the enumerator scrolled to the end.
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns>The number of items from that were remaining in the enumerator,
        /// or 0 if null was passed.</returns>
        public static int Count<T>(IEnumerator<T> enumerator)
        {
            if (enumerator == null)
            {
                return 0;
            }
            else
            {
                int count = 0;
                while (enumerator.MoveNext())
                    count++;
                return count;
            }
        }
    }
}
