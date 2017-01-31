using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Collections;

namespace JohnLambe.Util
{
    /// <summary>
    /// Represents a range of values - a minimum and maximum value.
    /// This may or may not be modifiable (depending on the subclass).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Range<T>
        where T : IComparable
    {
        // The public interface of this class must not allow changing state.
        // Subclasses may do so.

        public Range()
        {
        }

        public Range(T low, T high, bool inclusive = true)
        {
            if(high != null && low != null && high.CompareTo(Low) < 0)
            {
                throw new ArgumentException("Range: High cannot be lower than Low");
                // Disallow if High < Low
            }
            Low = low;
            High = high;
            Inclusive = inclusive;
        }

        /// <summary>
        /// Low end of the range.
        /// null if the range has no lower bound.
        /// </summary>
        public virtual T Low { get; protected set;  }

        /// <summary>
        /// High end of the range.
        /// null if the range has no upper bound.
        /// </summary>
        public virtual T High { get; protected set; }

        /// <summary>
        /// true iff the <see cref="Low"/> and <see cref="High"/> values are included in the range.
        /// </summary>
        public virtual bool Inclusive { get; protected set; } = true;

        /// <summary>
        /// true iff the given value is within the range.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool IsInRange(T value)
        {
            if (Inclusive)
                return (Low == null || value.CompareTo(Low) >= 0)
                    && (High == null || value.CompareTo(High) <= 0);
            else
                return (Low == null || value.CompareTo(Low) > 0)
                    && (High == null || value.CompareTo(High) < 0);
        }

        /// <summary>
        /// true iff there is any intersection between this range and the other (i.e. if there is any value common to the two ranges).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Intersects(Range<T> other)
        {
            if (other == null)
                return false;                                // null intersects nothing
            if (!Inclusive)
            {
                if (Low.CompareTo(other.High) >= 0            // Low of this higher than High of `other` (this whole range is higher)
                    || High.CompareTo(other.Low) <= 0)        // or High of this lower than Low of `other`
                {
                    return false;                             // no intersection
                }
            }
            else
            {
                if (Low.CompareTo(other.High) > 0            // Low of this higher than High of `other` (this whole range is higher)
                    || High.CompareTo(other.Low) < 0)        // or High of this lower than Low of `other`
                {
                    return false;                            // no intersection
                }
            }
            // otherwise, this may contain `other`, `other` may contain this, are they may partly intersect.
            return true;                                     // intersection
        }

        public override bool Equals(object obj)
        {
            if (obj is Range<T>)
            {
                return ((Range<T>)obj).Low.Equals(Low) && ((Range<T>)obj).High.Equals(High)
                    && ((Range<T>)obj).Inclusive.Equals(Inclusive);
            }
            //TODO: Types assignable to T ?
            else
            {
                return base.Equals(obj);
            }
        }

        
        /// <summary>
        /// Returns the intersion of this range and the other one (i.e. a range which contains all values common to both).
        /// Returns NoneRange is the ranges do not intersect.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>the intersection.</returns>
        public virtual Range<T> Intersection(Range<T> other)
        {
//            if (other == null)
//                return NoneRange;
            if (!Intersects(other))
                return NoneRange;
/*
            if (High.CompareTo(other.Low) < 0 
                || (High.CompareTo(other.Low) <= 0 && Inclusive)
                || (Low.CompareTo(other.High) > 0)
                || (Low.CompareTo(other.High) >= 0 && other.Inclusive)
                )
            {
                return NoneRange;
            }
            */

            return new Range<T>(CompareUtil.Max(Low,other.Low),CompareUtil.Min(High,other.High));
        }

        /// <summary>
        /// Combine this range and the given one, which must intersect this one,
        /// into one range.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>the combined range, or null if the ranges do not intersect.</returns>
        public virtual Range<T> Combine(Range<T> other)  // Union
        {
            if (!Intersects(other))
                return null;

            return new Range<T>(CompareUtil.Min(Low,other.Low),CompareUtil.Max(High,other.High));
        }

        // ==   All properties equal.
        // <    if value or whole range to compare to is less than low end.
        // +, *, /     Apply to both ends?

        /// <summary>
        /// A range which contains nothing.
        /// </summary>
        public static readonly Range<T> NoneRange = new Range<T>(default(T), default(T), false);
    }

    /// <summary>
    /// Range subclass that is immutable.
    /// <para>Subclasses must not allow changing the state.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ImmutableRange<T> : Range<T>
        where T : IComparable
    {
    }

    public class ModifiableRange<T> : Range<T>
        where T : IComparable
    {
        public new virtual T Low
        {
            get { return base.Low; }
            set { base.Low = value; }
        }

        public new virtual T High
        {
            get { return base.High; }
            set { base.High = value; }
        }
    }


    //[ComplexType]
    public class DateTimeRange : ModifiableRange<DateTime>
    {
        public DateTimeRange()
        {
            Inclusive = true;
        }

        public virtual DateTime Start
        {
            get { return Low; }
            set { High = value; }
        }
        public virtual DateTime End
        {
            get { return High; }
            set { High = value; }
        }
    }


}
