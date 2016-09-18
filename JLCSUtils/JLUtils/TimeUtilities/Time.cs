using System;

namespace JohnLambe.Util.TimeUtilities
{
    /// <summary>
    /// An immutable time-of-day value - effectively a timespan in the range 0 (inclusive) to 1 day (exclusive).
    /// <para>This has members equivalent to (most of) the members of <see cref="TimeSpan"/>, and some members of <see cref="DateTime"/>, relevant to a time of day.</para>
    /// <para>For consistency with <see cref="TimeSpan"/>, this uses <see cref="int"/>
    /// for parts of the time, and handles negative or out-of-range values similarly.
    /// </para>
    /// <para>Note: The serialized formt of this may change in a future version.</para>
    /// </summary>
    //TODO: [ComVisible] - Check that this works with COM.
    [Serializable]
    public struct Time : IEquatable<object>,
        IComparable<TimeSpan>, IComparable<Time>,
        ICloneable,
        IFormattable
    //        ISerializable  //TODO: Make serialisable
    {
        #region Constructors
        //TODO: Add Kind to first 3 ctors.

        public Time(TimeSpan value)
        {
            if (!TimeUtils.IsTimeOfDay(value))
                throw new OverflowException("Invalid Time value (" + value + ")");
            _value = value;
        }

        /// <summary>
        /// Construct from a number of ticks.
        /// </summary>
        /// <param name="ticks">Number of ticks, must not be negative, nor more than 24 hours.</param>
        /// <exception cref="OverflowException">If the value is outside the valid range of a time of day.</exception>
        public Time(long ticks)
        {
            if (ticks >= TimeSpan.TicksPerDay || ticks < 0)
                throw new OverflowException("Invalid `ticks` value for Time (" + ticks + ")");
            _value = new TimeSpan(ticks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <param name="milliseconds"></param>
        /// <exception cref="OverflowException">If the value is outside the valid range of a time of day.</exception>
        public Time(int hours, int minutes, int seconds = 0, int milliseconds = 0)
        {
//            if (hours >= TimeUtils.HoursPerDay || hours < 0)
//                throw new ArgumentException("Invalid value for Time (" + hours + " hours)");
            TimeSpan value = new TimeSpan(0, hours, minutes, seconds, milliseconds);
            if(!TimeUtils.IsTimeOfDay(value))
                throw new OverflowException("Invalid value for time of day (" + value + " )");
            _value = value;
        }

        /// <summary>
        /// Creates a <see cref="Time"/> from the time-of-day part of the given <see cref="DateTime"/>.
        /// </summary>
        /// <param name="time"></param>
        public Time(DateTime time)
        {
            _value = Truncate(time);
        }

        #endregion

        private TimeSpan _value;

        public TimeSpan AsTimeSpan
        {
            get { return _value; }
        }

        /// <summary>
        /// Converts this to a <see cref="DateTime"/> with a Date part of DateTime.MinValue.
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
            => DateTime.MinValue + _value;

        #region 12-Hour clock

        /// <summary>
        /// The 12-hour clock 'AM' or 'PM' part of the time.
        /// </summary>
        public TimeAmPm AmPm
            => (_value < TimeUtils.Noon) ? TimeAmPm.Am : TimeAmPm.Pm;

        /// <summary>
        /// The hour on a 12-hour clock.
        /// </summary>
        public int Hour12 => Hours % 12;

        #endregion

        /// <summary>
        /// Create a <see cref="Time"/> object populated with only the Time Of Day part of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Time Truncate(TimeSpan value) => new Time(value.TimeOfDay());

        /// <summary>
        /// Create a <see cref="Time"/> object populated with only the Time Of Day part of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Time Truncate(DateTime value) => new Time(value.TimeOfDay);

        public override string ToString()
            => ToDateTime().ToShortTimeString();

        public string ToLongTimeString()
            => ToDateTime().ToLongTimeString();

        public long ToBinary()
            => ToDateTime().ToBinary();

        public static Time FromBinary(long binary)
            => new Time(DateTime.FromBinary(binary));

        #region BinaryTime
        // Bits 2-31: the the ticks value divided by 1000.
        // Bits 0-1:  reserved for a Kind.

        /// <summary>
        /// Represents the time as a 32-bit integer.
        /// This involves loss of accuracy - its resolution is 1000 ticks (0.1 ms).
        /// <para>This value can be converted back to a <see cref="Time"/> using <see cref="FromBinaryTime"/>.</para>
        /// </summary>
        /// <returns></returns>
        public int ToBinaryTime()
            => unchecked((int)(_value.Ticks / BinaryTimeGranularity) << 2);

        /// <summary>
        /// Converts a value output by <see cref="ToBinaryTime"/> back to a <see cref="Time"/>.
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static Time FromBinaryTime(int binary)
            => new Time((long)(binary >> 2) * BinaryTimeGranularity);

        /// <summary>
        /// The resolution of the 'BinaryTime' format. (<see cref="ToBinaryTime"/>).
        /// <para>This value is chosen because it allows the highest resolution possible in 32-bits
        /// that gives midnight a value of 0, never involves a rounding error in conversion to seconds in decimal
        /// (it is a factor of a millisecond), and leaves space (2 bits) for a Kind.</para>
        /// </summary>
        private const long BinaryTimeGranularity = 1000;
        /* If no Kind is to be added:
        /// <summary>
        /// The resolution of the 'BinaryTime' format. (<see cref="ToBinaryTime"/>).
        /// <para>This value is chosen because it allows the highest resolution possible in 32-bits
        /// that gives midnight a value of 0 and never involves a rounding error in conversion to seconds in decimal
        /// (it is a factor of a millisecond).</para>
        /// </summary>
        private const long BinaryTimeGranularity = 500;
        */

        #endregion

        #region Properties
        // Properties for parts of the time.

        /// <summary>
        /// The hours component of the time.
        /// </summary>
        public int Hours
        {
            get { return _value.Hours; }
        }
        /// <summary>
        /// The minutes component of the time.
        /// </summary>
        public int Minutes
        {
            get { return _value.Minutes; }
        }
        /// <summary>
        /// The seconds component of the time.
        /// </summary>
        public int Seconds
        {
            get { return _value.Seconds; }
        }
        /// <summary>
        /// The milliseconds component of the time.
        /// </summary>
        public int Milliseconds
        {
            get { return _value.Milliseconds; }
        }

        /// <summary>
        /// The value as a fraction of a day (in the range 0 (inclusive) to 1 (exclusive)).
        /// </summary>
        public double TotalDays
        {
            get { return _value.TotalDays; }
        }
        /// <summary>
        /// The number of hours since midnight (a floating-point value in the range 0 (inclusive) to 24 (exclusive)).
        /// </summary>
        public double TotalHours
        {
            get { return _value.TotalHours; }
        }
        /// <summary>
        /// The total number of minutes since midnight.
        /// </summary>
        public double TotalMinutes
        {
            get { return _value.TotalMinutes; }
        }
        /// <summary>
        /// The total number of seconds since midnight.
        /// </summary>
        public double TotalSeconds
        {
            get { return _value.TotalSeconds; }
        }
        /// <summary>
        /// The total number of milliseconds since midnight.
        /// </summary>
        public double TotalMilliseconds
        {
            get { return _value.TotalMilliseconds; }
        }

        /// <summary>
        /// The total number of ticks since midnight.
        /// </summary>
        public long Ticks
        {
            get { return _value.Ticks; }
        }

        #endregion

        #region DateTime methods

        public Time AddHours(int hours)
            => new Time(_value + new TimeSpan(hours, 0, 0));

        public Time AddMinutes(int minutes)
            => new Time(_value + new TimeSpan(0, minutes, 0));

        public Time AddSeconds(int seconds)
            => new Time(_value + new TimeSpan(0, 0, seconds));

        public Time AddMilliseconds(int ms)
            => new Time(_value + new TimeSpan(0, 0, 0, 0, ms));

        #endregion

        #region Casting

        /// <summary>
        /// Cast this <see cref="Time"/> to a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator TimeSpan(Time value)
        {
            return value.AsTimeSpan;
        }

        /// <summary>
        /// Cast a <see cref="TimeSpan"/> to a <see cref="Time"/>.
        /// </summary>
        /// <exception cref="OverflowException">If the value is not a in the valid range for a time of day.</exception>
        /// <param name="value"></param>
        public static explicit operator Time(TimeSpan value)
        {
            return new Time(value);
        }

        /// <summary>
        /// Converts a <see cref="Time"/> to a <see cref="DateTime"/> with a date part of <see cref="DateTime.MinValue"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator DateTime(Time value)
        {
            return value.ToDateTime();
        }

        /// <summary>
        /// Casts a <see cref="DateTime"/> to a <see cref="Time"/>, discarding the date part.
        /// <para>Requires an explicit cast since information is lost.</para>
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator Time(DateTime value)
        {
            return new Time(value);
        }

        #endregion

        #region + and - operators

        /// <summary>
        /// Adds a <see cref="TimeSpan"/> to this time.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="delta"></param>
        /// <exception cref="OverflowException">If the resultant value is not a valid time of day.</exception>
        /// <returns></returns>
        public static Time operator + (Time time, TimeSpan delta)
        {
            return new Time(time.AsTimeSpan + delta);
        }

        /// <summary>
        /// Subtracts a <see cref="TimeSpan"/> from this time.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="delta"></param>
        /// <exception cref="OverflowException">If the resultant value is not a valid time of day.</exception>
        /// <returns></returns>
        public static Time operator - (Time time, TimeSpan delta)
        {
            return new Time(time.AsTimeSpan - delta);
        }

        /// <summary>
        /// Calculate the difference between to times as a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        public static TimeSpan operator -(Time time1, Time time2)
            => time1.AsTimeSpan - time2.AsTimeSpan;

        #endregion

        #region Add / Subtract

        /// <summary>
        /// Adds a <see cref="TimeSpan"/> to this time.
        /// </summary>
        /// <param name="delta"></param>
        /// <exception cref="OverflowException">If the resultant value is not a valid time of day.</exception>
        /// <returns></returns>
        public Time Add(TimeSpan delta)
        {
            return this + delta;
        }

        /// <summary>
        /// Subtracts a <see cref="TimeSpan"/> from this time.
        /// </summary>
        /// <param name="delta"></param>
        /// <exception cref="OverflowException">If the resultant value is not a valid time of day.</exception>
        /// <returns></returns>
        public Time Subtract(TimeSpan delta)
        {
            return this - delta;
        }

        #endregion

        #region Comparison operators

        //TODO: What if only Kind is different?
        /// <summary>
        /// True iff the two <see cref="Time"/> objects represent the same time.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Time a, Time b)
        {
            return a._value == b._value;
        }
        public static bool operator !=(Time a, Time b)
        {
            return a._value != b._value;
        }

        /// <summary>
        /// Compares two times.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>True iff <paramref name="a"/> is earlier than <paramref name="b"/>.</returns>
        public static bool operator <(Time a, Time b)
        {
            return a._value < b._value;
        }
        /// <summary>
        /// Compares two times.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>True iff <paramref name="a"/> is later than <paramref name="b"/>.</returns>
        public static bool operator >(Time a, Time b)
        {
            return a._value > b._value;
        }
        public static bool operator >=(Time a, Time b)
        {
            return a._value >= b._value;
        }
        public static bool operator <=(Time a, Time b)
        {
            return a._value >= b._value;
        }

        #endregion

        #region Equals and GetHashCode

        public override bool Equals(object other)
        {
//            if (other == null)
//                return _value == null;         // equal if both are null

            if (other is Time)
                return _value == ((Time)other)._value;

//            else if (other is TimeSpan?)
//                return _value == (TimeSpan?)other;
            else if (other is TimeSpan)
                return _value == (TimeSpan)other;

            else if (other is DateTime)
                return _value == ((DateTime)other).TimeOfDay
                    && TimeUtils.IsDateOnly((DateTime)other);
//            else if (other is DateTime?)
//                return (!_value.HasValue && !((DateTime?)other).HasValue)
//                    || (_value.HasValue && ((DateTime?)other).HasValue
//                    && _value == ((DateTime?)other).Value.TimeOfDay
//                    && ((DateTime?)other).Value.Date == DateTime.MinValue);

            else
                return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        #endregion

        #region IComparable

        public int CompareTo(TimeSpan other)
        {
            if (this > other)
                return 1;
            else if (this < other)
                return -1;
            else
                return 0;
        }

        public int CompareTo(Time other)
        {
            if (this > other)
                return 1;
            else if (this < other)
                return -1;
            else
                return 0;
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Creates a new <see cref="Time"/> instance with the same value as this one.
        /// </summary>
        /// <returns></returns>
        public object Clone() => new Time(AsTimeSpan);

        #endregion

        #region IFormattable

        /// <summary>
        /// Formats this in the same way as a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ((IFormattable)ToDateTime()).ToString(format, formatProvider);
        }

        #endregion

        public string ToTimeSpanString(string format, IFormatProvider formatProvider)
        {
            return ((IFormattable)_value).ToString(format, formatProvider);
        }

        /*  //TODO
        #region ISerializable

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ((ISerializable)_value).GetObjectData(info, context);
        }

        #endregion
        */

        /// <summary>
        /// Indicates whether a 12-hour time is 'AM' (before noon) or 'PM'.
        /// </summary>
        public enum TimeAmPm { Am, Pm };
    }
}
