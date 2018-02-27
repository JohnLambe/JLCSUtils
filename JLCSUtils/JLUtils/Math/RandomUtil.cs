// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using JohnLambe.Util.Services;
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
    public static class RandomUtil
    {
        public static readonly IRandomService RandomService = JohnLambe.Util.Services.RandomService.CreateWithSeed(
            Environment.TickCount ^ System.Environment.MachineName.GetHashCode() ^ System.Threading.Thread.CurrentThread.ManagedThreadId);
        // seeded with the time, machine name and thread ID.

        /*
                /// <summary>
                /// Returns a random integer in the range 0..<paramref name="range"/>-1.
                /// </summary>
                /// <param name="range"></param>
                /// <returns></returns>
                public static int Random(int range)
                {
                    return RandomService.Next(range);
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="min"></param>
                /// <param name="max"></param>
                /// <returns></returns>
                public static int Random(int min, int max)
                {
                    return RandomService.Next(min, max);
                }

                /// <summary>
                /// Positive 64-bit random number.
                /// </summary>
                /// <returns></returns>
                public static long RandomPositiveLong()
                {
                    return RandomService.RandomPositiveLong();
                }

                /// <summary>
                /// 64-bit random number (positive or negative).
                /// </summary>
                /// <returns></returns>
                public static long RandomLong()
                {
                    return RandomService.RandomLong();
                }

                /// <summary>
                /// Returns a random Enum value of the type.
                /// </summary>
                /// <typeparam name="T"></typeparam>
                /// <returns></returns>
                public static T RandomEnumValue<T>()
                {
                    //TODO Use IRandomService
                    var values = typeof(T).GetEnumValues();
                    return (T)values.GetValue(Random(values.Length));
                }
                */
    }

    /// <summary>
    /// Extends <see cref="Random"/> with methods for more data types.
    /// <para>
    /// This is NOT a secure random number generator.
    /// </para>
    /// </summary>
    public class RandomExt : Random
    {
        public RandomExt()
        {
        }

        public RandomExt(int seed) : base(seed)
        {
        }

        /*        public void Reseed(int seed)
                {
                    Seed(Next() ^ seed);
                }
        */

            /*
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
*/

        /// <summary>
        /// Returns a random positive 64-bit signed value, with a uniform distribution from 0 to 2^63-1 inclusive.
        /// </summary>
        /// <returns>The random <see cref="long"/> value.</returns>
        public virtual long NextLong() => Next() ^ (Next() << 31) ^ (Next() << 32);

        /*
        /// <summary>
        /// Returns a random GUID.
        /// </summary>
        /// <returns></returns>
        public virtual Guid RandomGuid() => Guid.NewGuid();

        /// <summary>
        /// Returns a <see cref="Guid"/> in which all bits are random (NOT a standard random GUID).
        /// The returned value is not valid for interpreting parts of the GUID value.
        /// </summary>
        /// <returns></returns>
        public virtual Guid Random128Bits() => new Guid(RandomBytes(16));

        /// <summary>
        /// Returns an array of random bytes of the specified size.
        /// </summary>
        /// <param name="size">Size of the array.</param>
        /// <returns>The new array.</returns>
        public virtual byte[] RandomBytes(int size)
        {
            byte[] randomValue = new byte[size];
            return randomValue;
        }
        */
    }

    public static class RandomServiceExtension
    {
        /// <summary>
        /// Returns a random capital letter character (from the set of letters supported in ASCII).
        /// </summary>
        /// <param name="r">The random source.</param>
        /// <returns></returns>
        public static char RandomLetter(this IRandomService r)
        {
            return (char)r.Next((int)'A', (int)'Z');
        }

        /// <summary>
        /// Returns a random character in the given range.
        /// </summary>
        /// <param name="r">The random source.</param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static char RandomChar(this IRandomService r, char minimum, char maximum)
        {
            return (char)(minimum + r.Next((int)maximum - (int)minimum));
        }

        /// <summary>
        /// Returns a random Enum value of the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="r">The random source.</param>
        /// <returns></returns>
        public static T RandomEnumValue<T>(this IRandomService r)
        {
            var values = typeof(T).GetEnumValues();
            return (T)values.GetValue(r.Next(values.Length));
        }

        /// <summary>
        /// Returns a random byte value.
        /// </summary>
        /// <param name="r">The random source.</param>
        /// <returns></returns>
        public static byte RandomByte(this IRandomService r)
        {
            return (byte)r.Next(256);
        }

        /// <summary>
        /// Returns a random boolean value (50% chance of true).
        /// </summary>
        /// <param name="r">The random source.</param>
        /// <returns></returns>
        public static bool RandomBool(this IRandomService r)
        {
            return r.Next(2) == 0;
        }

        /// <summary>
        /// Returns a random positive 64-bit signed value, with a uniform distribution from 0 to 2^63-1 inclusive.
        /// </summary>
        /// <param name="r">The random source.</param>
        /// <returns>The random <see cref="long"/> value.</returns>
        public static long NextLong(this IRandomService r) => r.Next() ^ (r.Next() << 31) ^ (r.Next() << 32);

        /// <summary>
        /// Returns a random GUID.
        /// </summary>
        /// <param name="r">The random source.</param>
        /// <returns></returns>
        public static Guid RandomGuid(this IRandomService r) => Guid.NewGuid();

        /// <summary>
        /// Returns a <see cref="Guid"/> in which all bits are random (NOT a standard random GUID).
        /// The returned value is not valid for interpreting parts of the GUID value.
        /// </summary>
        /// <param name="r">The random source.</param>
        /// <returns></returns>
        public static Guid Random128Bits(this IRandomService r) => new Guid(r.RandomBytes(16));

        /// <summary>
        /// Returns an array of random bytes of the specified size.
        /// </summary>
        /// <param name="r">The random source.</param>
        /// <param name="size">Size of the array.</param>
        /// <returns>The new array.</returns>
        public static byte[] RandomBytes(this IRandomService r, int size)
        {
            byte[] randomValue = new byte[size];
            r.NextBytes(randomValue);
            return randomValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="r">The random source.</param>
        /// <returns>64-bit random number.</returns>
        public static long RandomLong(this IRandomService r)
            => r.Next() | (r.Next() << 31) | (r.Next(4) << 62);
        // random.Next() returns 31 random bits (always positive), so we call a third time for another 2 bits.

        /// <summary>
        /// </summary>
        /// <param name="r">The random source.</param>
        /// <returns>Positive 64-bit random number.</returns>
        public static long RandomPositiveLong(this IRandomService r)
            => r.Next() | (r.Next() << 31) | (r.Next(2) << 62);
            // random.Next() returns 31 random bits (always positive), so we call a third time for another bit. (63 bits in total.)
    }
}
