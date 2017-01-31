using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Collections
{
    public static class CompareUtil
    {
        /// <summary>
        /// Returns the lowest of the parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T Min<T>(params T[] values)
        {
            return values.Min();
        }

        /// <summary>
        /// Returns the highest of the parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T Max<T>(params T[] values)
        {
            return values.Max();
        }
    }
}
