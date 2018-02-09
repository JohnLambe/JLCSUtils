// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Math
{
    /// <summary>
    /// Random value utilities.
    /// </summary>
    public static class RandomUtils
    {
        private static readonly Random _random = new System.Random(Environment.TickCount ^ System.Environment.MachineName.GetHashCode() ^ System.Threading.Thread.CurrentThread.ManagedThreadId);
            // seeded with the time, machine name and thread ID.

        /// <summary>
        /// Returns a random integer in the range 0..<paramref name="range"/>-1.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static int Random(int range)
        {
            return _random.Next(range);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Random(int min, int max)
        {
            return _random.Next(min, max);
        }

        /// <summary>
        /// Positive 64-bit random number.
        /// </summary>
        /// <returns></returns>
        public static long RandomPositiveLong()
        {
            return _random.Next() | (_random.Next() << 31) | (_random.Next(2) << 62);
            // random.Next() returns 31 random bits (always positive), so we call a third time for another bit. (63 bits in total.)
        }

        /// <summary>
        /// 64-bit random number (positive or negative).
        /// </summary>
        /// <returns></returns>
        public static long RandomLong()
        {
            return _random.Next() | (_random.Next() << 31) | (_random.Next(4) << 62);
            // random.Next() returns 31 random bits (always positive), so we call a third time for another 2 bits.
        }

        /// <summary>
        /// Returns a random Enum value of the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomEnumValue<T>()
        {
            var values = typeof(T).GetEnumValues();
            return (T)values.GetValue(Random(values.Length));
        }
    }

    /// <summary>
    /// Extends <see cref="Random"/> with methods for more data types.
    /// </summary>
    public class RandomUtil : Random
    {
        /*        public void Reseed(int seed)
                {
                    Seed(Next() ^ seed);
                }
        */

        /// <summary>
        /// Returns a random capital letter character (from the set of letters supported in ASCII).
        /// </summary>
        /// <returns></returns>
        public virtual char RandomLetter()
        {
            return (char)Next((int)'A', (int)'Z');
        }

        /// <summary>
        /// Returns a random Enum value of the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T RandomEnumValue<T>()
        {
            var values = typeof(T).GetEnumValues();
            return (T)values.GetValue(Next(values.Length));
        }

        /// <summary>
        /// Returns a random byte value.
        /// </summary>
        /// <returns></returns>
        public virtual byte RandomByte()
        {
            return (byte)Next(256);
        }

        /// <summary>
        /// Returns a random boolean value (50% chance of true).
        /// </summary>
        /// <returns></returns>
        public virtual bool RandomBool()
        {
            return Next(2) == 0;
        }

        /// <summary>
        /// Returns a random positive 64-bit signed value, with a uniform distribution from 0 to 2^63-1 inclusive.
        /// </summary>
        /// <returns>The random <see cref="long"/> value.</returns>
        public virtual long NextLong() => Next() ^ (Next() << 31) ^ (Next() << 32);

        /// <summary>
        /// Returns a random GUID.
        /// </summary>
        /// <returns></returns>
        public virtual Guid RandomGuid() => Guid.NewGuid();  //TODO: Make Random
    }
}
