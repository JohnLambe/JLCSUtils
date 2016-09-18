using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Encoding
{
    /// <summary>
    /// Converts binary data to a hexadecimal representation.
    /// <para>The output uses capital hexadecimal letters, two digits per byte,
    /// with no separators, unless otherwise specified.</para>
    /// </summary>
    public static class HexConverter
    {
        /// <summary>
        /// Converts a byte array to a hexadecimal string.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>Hexadecimal string, or null if the input was null.</returns>
        public static string ToHex(byte[] bytes)
        {
            if (bytes != null)
            {
                StringBuilder stringbuilder = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes)
                {
                    stringbuilder.Append(ToHex(b));
                }
                return stringbuilder.ToString();
            }
            else
            {
                return null;
            }
        }

        public static string ToHex(byte value)
        {
            return value.ToString("X2");
        }
    }
}
