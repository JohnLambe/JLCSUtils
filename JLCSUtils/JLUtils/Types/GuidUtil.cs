using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Types
{
    /// <summary>
    /// GUID-related utilities.
    /// <para>
    /// Note: The GUID variant and version are interpreted using Microsoft byte order.
    /// The nil GUID (all bits zero) is treated as a Variant 0 GUID.
    /// </para>
    /// </summary>
    public static class GuidUtil
    {
        /// <summary>
        /// Formats the GUID with no punctuation, with all letters in lowercase.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>
        /// the string representation.
        /// If passed null, null is returned.
        /// </returns>
        public static string CompactForm(this Guid? value)
            => value?.ToString()?.Replace("-","")?.ToLowerInvariant();

        /// <summary>
        /// Returns the Variant 1 or 2 GUID version.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>the GUID version (4 bits), or -1 if this is not a Variant 1 or 2 GUID (based on the variant bits only) (including if it is a nil GUID).</returns>
        public static int GetVersion(this Guid value)
        {
            int version;
            GetVariant(value, out version);
            return version;
        }

        /// <summary>
        /// Get the variant type of the GUID.
        /// </summary>
        /// <param name="value">the GUID version (4 bits), or -1 if this is not a Variant 1 or 2 GUID (based on the variant bits only).</param>
        /// <returns>the GUID variant (0 to 2), or -1 if this is not a recognised variant.</returns>
        public static int GetVariant(this Guid value)
        {
            int dummy;
            return GetVariant(value, out dummy);
        }

        /// <summary>
        /// Get the variant and version of the GUID.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="version"></param>
        /// <returns>the GUID variant (0 to 2), or -1 if this is not a recognised variant.</returns>
        public static int GetVariant(this Guid value, out int version)
        {
            var bytes = value.ToByteArray();
            int variant;

            int variantByte = bytes[8];    // the byte that contains the variant bits
            if ((variantByte & 0x80) == 0)    // 0xxx xxxx
                variant = 0;           // Apollo Network Computer System
            else if ((variantByte & 0xC0) == 0x80)  // 10xx xxxx
                variant = 1;           // RFC 4122 / DCE 1.1 / Leach-Salz
            else if ((variantByte & 0xE0) == 0xC0)  // 110x xxxx
                variant = 2;           // "reserved, Microsoft backward compatibility"
            else                       // otherwise, it's 11x xxxx
                variant = -1;                 // reserved for future variants

            if (variant == 1 || variant == 2)
                version = bytes[7] >> 4;
            else
                version = -1;   // a variant that doesn't have a Version.

            return variant;
        }
    }
}
