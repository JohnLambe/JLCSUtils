using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Encoding
{
    public static class BinaryConverter
    {
        public static byte[] FromLong(long value)
        {
            return new byte[]
            {
                (byte)(value & 0xFF),
                (byte)(value >> 8),
                (byte)(value >> 16),
                (byte)(value >> 24),
                (byte)(value >> 32),
                (byte)(value >> 40),
                (byte)(value >> 48),
                (byte)(value >> 56)
            };
        }

        public static long ToLong(byte[] value)
        {
            return value[0]
                | value[1] << 8
                | value[2] << 16
                | value[3] << 24
                | value[4] << 32
                | value[5] << 40
                | value[6] << 48
                | value[7] << 56;
        }
    }
}
