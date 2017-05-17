// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////


namespace JohnLambe.Util.Math
{
    public static class BitUtil
    {
        // http://stackoverflow.com/questions/757059/position-of-least-significant-bit-that-is-set :
        // x & (x-1) will clear the lowest set bit
        // lowest set bit: ( x & ~(x-1) )

        /*
unsigned int v;  // find the number of trailing zeros in 32-bit v 
int r;           // result goes here
static const int MultiplyDeBruijnBitPosition[32] = 
{
  0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8, 
  31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
};
r = MultiplyDeBruijnBitPosition[((uint32_t)((v & -v) * 0x077CB531U)) >> 27];
        */

        // http://graphics.stanford.edu/~seander/bithacks.html#IntegerMinOrMax

        #region LowSetBit

        /// <summary>
        /// Returns the bit index of the least significant bit in <paramref name="value"/> that is set.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>bit index: 0-31, or -1 if <paramref name="value"/> is 0.</returns>
        public static int LowSetBit(uint value)
        {
            // Rather than a loop with up to 32 iterations,
            // we test half of the bits at a time, for a total of 5 tests.
            // We identify the byte, then call ByteLowSetBit.

            if (value == 0)
                return -1;

            if ((value & 0xFFFF) != 0)   // in bits 0-15
            {
                if ((value & 0x00FF) != 0)   // in bits 0-7
                {
                    return ByteLowSetBit((byte)(value & 0xFF));
                }
                else  // bits 8-15
                {
                    return ByteLowSetBit((byte)(value >> 8)) + 8;
                }
            }
            else   // bits 16-31
            {
                if ((value & 0x00FF0000) != 0)   // in bits 16-23
                {
                    return ByteLowSetBit((byte)(value >> 16)) + 16;
                }
                else    // bits 24-31
                {
                    return ByteLowSetBit((byte)(value >> 24)) + 24;
                }
            }
        }

        /// <summary>
        /// Returns the bit index of the least significant bit in <paramref name="value"/> that is set.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>bit index: 0-7, or -1 if <paramref name="value"/> is 0 (no bit set).</returns>
        public static int ByteLowSetBit(byte value)
        {
            if (value == 0)
                return -1;

            if ((value & 0x0F) != 0)   // in bits 0-3
            {
                if ((value & 0x3) != 0)    // bits 0-1
                {
                    if ((value & 0x1) != 0)
                        return 0;
                    else
                        return 1;
                }
                else  // bits 2-3
                {
                    if ((value & 0x4) != 0)
                        return 2;
                    else
                        return 3;
                }
            }
            else   // bits 4-7
            {
                if ((value & 0x30) != 0)    // bits 4-5
                {
                    if ((value & 0x10) != 0)   // bit 4
                        return 4;
                    else
                        return 5;
                }
                else  // bits 6-7
                {
                    if ((value & 0x40) != 0)  // bit 6
                        return 6;
                    else
                        return 7;
                }
            }
        }

        #endregion

        #region AllSet

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bits"></param>
        /// <returns>true iff all bits that are set in <paramref name="bits"/> are set in <paramref name="value"/>.</returns>
        public static bool AllSet(int value, int bits)
        {
            return (value & bits) == bits;
        }

        /// <returns>true iff all bits that are set in <paramref name="bits"/> are set in <paramref name="value"/>.</returns>
        public static bool AllSet(uint value, uint bits)
        {
            return (value & bits) == bits;
        }

        /// <returns>true iff all bits that are set in <paramref name="bits"/> are set in <paramref name="value"/>.</returns>
        public static bool AllSet(long value, long bits)
        {
            return (value & bits) == bits;
        }

        /// <returns>true iff all bits that are set in <paramref name="bits"/> are set in <paramref name="value"/>.</returns>
        public static bool AllSet(ulong value, ulong bits)
        {
            return (value & bits) == bits;
        }

        #endregion
    }

}
