using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;

namespace JohnLambe.Util.Text
{
    public class FormatUtil
    {
        /// <summary>
        /// Format a value with the given format and provider (if supported).
        /// If <paramref name="arg"/> is of a primitive type, it is formatted with the given arguments,
        /// otherwise, this just returns the argument converted to a string.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg">
        /// The value to be formatted.
        /// If null, null is returned.
        /// </param>
        /// <param name="formatProvider"></param>
        /// <returns>the formatted string.</returns>
        [return: Nullable]
        public static string FormatObject(string format, [Nullable] object arg, [Nullable] IFormatProvider formatProvider)
        {
            return
                (arg as long?)?.ToString(format, formatProvider)
                ?? (arg as int?)?.ToString(format, formatProvider)
                ?? (arg as short?)?.ToString(format, formatProvider)
                ?? (arg as sbyte?)?.ToString(format, formatProvider)

                ?? (arg as ulong?)?.ToString(format, formatProvider)
                ?? (arg as uint?)?.ToString(format, formatProvider)
                ?? (arg as ushort?)?.ToString(format, formatProvider)
                ?? (arg as byte?)?.ToString(format, formatProvider)

                ?? (arg as double?)?.ToString(format, formatProvider)
                ?? (arg as float?)?.ToString(format, formatProvider)
                ?? (arg as decimal?)?.ToString(format, formatProvider)

                ?? (arg as Guid?)?.ToString(format, formatProvider)

                ?? arg?.ToString();
        }
    }
}
