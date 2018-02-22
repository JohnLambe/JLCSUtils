using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Encoding
{
    /// <summary>
    /// Converts types to fixed-length strings of visible characters that have the same
    /// sorting order as the original type.
    /// </summary>
    public static class SortStringCalculator
    {
        public static string IntToSortString(int value)
        {
            return unchecked(value + 0x80000000).ToString("X8");
            // maps -2^31 .. 2^31-1 to 0 .. 0xFFFFFFFF - everything to an unsigned value in the same sorting order.
        }

        public static string UIntToSortString(uint value)
        {
            return value.ToString("X8");
        }

        public static string Int16ToSortString(short value)
        {
            return unchecked(value + 0x8000).ToString("X4");
        }

        public static string UInt16ToSortString(ushort value)
        {
            return value.ToString("X4");
        }
    }
}
