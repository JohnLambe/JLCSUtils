using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Text
{
    public static class FormatCollectionExtension
    {
        /// <summary>
        /// Format all items in <parmref name="enumerable"/> into a single string.
        /// Null items are omitted.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="separator">String added between items.</param>
        /// <param name="format">Format of each individual item.</param>
        /// <param name="token">This string is replaced in <paramref name="format"/> with the value of the item (converted to string using its ToString() method).</param>
        /// <returns>The string containing the list of items.</returns>
        public static string FormatCollection<T>(this IEnumerable<T> enumerable, string separator = ", ", string format = "%0", string token = "%0")
        {
            var result = new StringBuilder();
            bool first = true;   // true when at the first included item
            foreach(var item in enumerable)
            {
                if (item != null)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        result.Append(separator);
                    }
                    result.Append(format.Replace(token, item.ToString()));
                }
            }
            return result.ToString();
        }
    }
}
