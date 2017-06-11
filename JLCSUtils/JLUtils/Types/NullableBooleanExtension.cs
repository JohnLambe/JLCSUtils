using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Types
{
    /// <summary>
    /// Extension methods of <see cref="Nullable{bool}"/>, NOT related to <see cref="NullableBool"/>.
    /// </summary>
    public static class NullableBooleanExtension
    {
        /// <summary>
        /// Compare two nullable booleans, but return true if either is null.
        /// </summary>
        /// <param name="required"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static bool CompareNonNull(this bool? required, bool? actual)
        {
            return !required.HasValue
                || !actual.HasValue
                || required == actual;
        }
    }
}
