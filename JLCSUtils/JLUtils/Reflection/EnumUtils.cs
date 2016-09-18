using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// </summary>
    public static class EnumUtils
    {
        /// <summary>
        /// True if any of the bits in <paramref name="mask"/> are set in this enum value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        /// <seealso cref="Enum.HasFlag(Enum)"/>
        public static bool HasAnyFlag(this Enum value, int mask)
        {
            return ((int)Enum.ToObject(value.GetType(), value) & mask) != 0;
        }

        /// <summary>
        /// True if any of the bits in <paramref name="mask"/> are set in this enum value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        /// <seealso cref="Enum.HasFlag(Enum)"/>
        public static bool HasAnyFlag(this Enum value, long mask)
        {
            return ((long)Enum.ToObject(value.GetType(), value) & mask) != 0;
        }
    }
}
