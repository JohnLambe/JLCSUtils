﻿using JohnLambe.Util.MathUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Services
{
    /// <summary>
    /// Service for returning random numbers.
    /// </summary>
    public interface IRandomService
    {
        //
        // Summary:
        //     Returns a non-negative random integer.
        //
        // Returns:
        //     A 32-bit signed integer that is greater than or equal to 0 and less than System.Int32.MaxValue.
        int Next();
        //
        // Summary:
        //     Returns a non-negative random integer that is less than the specified maximum.
        //
        // Parameters:
        //   maxValue:
        //     The exclusive upper bound of the random number to be generated. maxValue must
        //     be greater than or equal to 0.
        //
        // Returns:
        //     A 32-bit signed integer that is greater than or equal to 0, and less than maxValue;
        //     that is, the range of return values ordinarily includes 0 but not maxValue. However,
        //     if maxValue equals 0, maxValue is returned.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     maxValue is less than 0.
        int Next(int maxValue);
        //
        // Summary:
        //     Returns a random integer that is within a specified range.
        //
        // Parameters:
        //   minValue:
        //     The inclusive lower bound of the random number returned.
        //
        //   maxValue:
        //     The exclusive upper bound of the random number returned. maxValue must be greater
        //     than or equal to minValue.
        //
        // Returns:
        //     A 32-bit signed integer greater than or equal to minValue and less than maxValue;
        //     that is, the range of return values includes minValue but not maxValue. If minValue
        //     equals maxValue, minValue is returned.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     minValue is greater than maxValue.
        int Next(int minValue, int maxValue);
        //
        // Summary:
        //     Fills the elements of a specified array of bytes with random numbers.
        //
        // Parameters:
        //   buffer:
        //     An array of bytes to contain random numbers.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     buffer is null.
        void NextBytes(byte[] buffer);
        //
        // Summary:
        //     Returns a random floating-point number that is greater than or equal to 0.0,
        //     and less than 1.0.
        //
        // Returns:
        //     A double-precision floating point number that is greater than or equal to 0.0,
        //     and less than 1.0.
        double NextDouble();

        /// <summary>
        /// Returns a random positive 64-bit signed value, with a uniform distribution from 0 to 2^63-1 inclusive.
        /// </summary>
        /// <returns>The random <see cref="long"/> value.</returns>
        long NextLong();

        /// <summary>
        /// Generates a new GUID.
        /// The implementation can return any type of GUID. It is not necessarily a random one.
        /// </summary>
        /// <returns>The new GUID value.</returns>
        Guid NewGuid();

        /*
        /// <summary>
        /// Generates a new random GUID.
        /// <para>The returned GUID must be a standard variant and version (including future ones), and contain random data and no information about
        /// the system or context in which it is generated (except for its type / version identification).</para>
        /// </summary>
        /// <returns>The new GUID value.</returns>
        Guid RandomGuid();
        */
    }


    /// <summary>
    /// Standard implementation of <see cref="IRandomService"/>.
    /// <para>For unit testing, this can be used with a constant seed.</para>
    /// </summary>
    public class RandomService : RandomExt, IRandomService
    {
        public RandomService()
        {
        }

        protected RandomService(int seed) : base(seed)
        {
        }

        public static RandomService CreateWithSeed(int seed)
        {
            return new RandomService(seed);
        }

        /// <inheritdoc cref="IRandomService.NewGuid()"/>
        public virtual Guid NewGuid() => new Guid();
    }
}
