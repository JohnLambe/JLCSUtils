using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Encoding
{
    /// <summary>
    /// Extension methods of <see cref="global::System.Text.Encoding"/>.
    /// </summary>
    public static class EncodingExtension
    {
        // Encoding/Decoding single character to/from single byte:

        /// <summary>
        /// Decode a single byte.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public static char GetChar(this global::System.Text.Encoding e, byte encoded)
        {
            _byteArray[0] = encoded;
            var chars = e.GetChars(_byteArray);
            // equivalent to: var chars = e.GetChars(new [] { encoded });

            if (chars.Length > 1)
                throw new ArgumentException("The given byte cannot be decoded to one character");  // an unlikely but theoretically possible case (for a theoretical encoding)
            return chars[0];
        }
        /// <summary>
        /// For use only in <see cref="GetChar"/>.
        /// </summary>
        [ThreadStatic]
        private static byte[] _byteArray = new byte[1];   // so that a new byte array is not allocated on each call.

        /// <summary>
        /// Decode a single character to a single byte.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If the given character cannot be encoded in one byte (but the encoder can encode it).</exception>
        public static byte GetByte(this global::System.Text.Encoding e, char c)
        {
            _charArray[0] = c;
            var bytes = e.GetBytes(_charArray);
            // equivalent to: var bytes = e.GetBytes(new [] { c });

            if (bytes.Length > 1)
                throw new ArgumentException("The given character cannot be encoded in one byte");
            return bytes[0];
        }
        /// <summary>
        /// For use only in <see cref="GetByte"/>.
        /// </summary>
        [ThreadStatic]
        private static char[] _charArray = new char[1];
    }

}
