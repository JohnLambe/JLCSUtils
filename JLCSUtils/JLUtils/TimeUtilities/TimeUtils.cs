using System;
using System.Runtime.CompilerServices;

namespace JohnLambe.Util.TimeUtilities
{
    /// <summary>
    /// Time-related utilities.
    /// </summary>
    public static class TimeUtils
    {
        #region Constants
        // Time-related constants (to use for readability):

        public const int HoursPerDay = 24;
        public const int MinutesPerHour = 60;
        public const int SecondsPerMinute = 60;
        public const int DaysPerWeek = 7;

        // Common time durations:
        public static readonly TimeSpan Day = new TimeSpan(1, 0, 0, 0);
        public static readonly TimeSpan Hour = new TimeSpan(1, 0, 0);
        public static readonly TimeSpan Minute = new TimeSpan(0, 1, 0);
        public static readonly TimeSpan Second = new TimeSpan(0, 0, 1);
        public static readonly TimeSpan Millisecond = new TimeSpan(0, 0, 0, 1);

        // Times of day:

        //| These could have been on the Time class, but they're here since they're of type TimeSpan, rather than Time.
        /// <summary>
        /// A constant representing the time 12:00. Use this only when the TimeSpan represents a time of day.
        /// </summary>
        public static readonly TimeSpan Noon = new TimeSpan(12, 0, 0);

        /// <summary>
        /// A constant representing the time 00:00. Use this only when the TimeSpan represents a time of day.
        /// <para>This is the same as <see cref="TimeSpan.Zero"/>. It is for use for readability, to indicate that the meaning is a time of day.</para>
        /// </summary>
        public static TimeSpan Midnight
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return TimeSpan.Zero; }
        }

        #endregion

        #region TimeOfDay

        /// <summary>
        /// Returns the time-of-day part of the given timespan.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The given value witout the Days part, or null if the parameter was null.</returns>
        public static TimeSpan? TimeOfDay(this TimeSpan? value)
        {
            if (value == null)
                return null;
            else
                return TimeOfDay(value.Value);
        }

        /// <summary>
        /// Returns the time-of-day part of the given timespan.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The given value witout the Days part.</returns>
        public static TimeSpan TimeOfDay(this TimeSpan value)
        {
            return new TimeSpan(value.Ticks % TimeSpan.TicksPerDay);
        }

        #endregion

        #region IsDateOnly / IsTimeOfDay
        //| These could be extension methods, but might seem like clutter in the list in the IDE.

        //| Could add overloads for nullable types, but what should they do on null? :
        //| return true (because it is valid as a nullable date or time part) or throw an exception.

        public static bool IsDateOnly(DateTime datetime)
        {
            return datetime.TimeOfDay == Midnight;
        }

        /// <summary>
        /// True if the given TimeSpan is a valid time of day (positive and less than a day).
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static bool IsTimeOfDay(TimeSpan timeSpan)
        {
            return timeSpan >= Midnight && timeSpan.Days == 0;
        }

        /// <summary>
        /// True if the given date-time value has only a time part (the Date part is DateTime.MinValue).
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool IsTimeOfDay(DateTime time)
        {
            return time.Date == DateTime.MinValue;
        }

        #endregion

        #region ToDateTime
        // Combine an optional DateTime date part and TimeSpan time part to form a DateTime, with validation.

        /// <summary>
        /// Form a DateTime from a date and time part.
        /// </summary>
        /// <param name="datePart">The date part. This must have a time of 00:00.</param>
        /// <param name="timePart">The time part. This must be a time of day: positive and less than a day.</param>
        /// <exception cref="ArgumentException"/>
        /// <returns></returns>
        //| This could be called 'CombineDateTime' or 'FormDateTime'.
        //| This was chosen to reduce the number of method names consumers have to remember. (Since this is an overload of another method).
        public static DateTime ToDateTime(DateTime datePart, TimeSpan timePart)
        {
            if (!IsDateOnly(datePart))
                throw new ArgumentException("Date contains time part");
            if (!IsTimeOfDay(timePart))
                throw new ArgumentException("Time is not a valid time of day");
            return datePart + timePart;
        }

        /// <summary>
        /// Convert a timespan representing a time of day to a DateTime.
        /// (The Date part will be DateTime.MinValue).
        /// </summary>
        /// <param name="timePart">The time part. This must be a time of day: positive and less than a day.</param>
        /// <exception cref="ArgumentException"/>
        /// <returns></returns>
        //| This could be made an extension method of TimeSpan.
        public static DateTime ToDateTime(TimeSpan timePart)
        {
            if (!IsTimeOfDay(timePart))
                throw new ArgumentException("Time is not a valid time of day");
            return DateTime.MinValue + timePart;
        }

        #endregion
    }
}
