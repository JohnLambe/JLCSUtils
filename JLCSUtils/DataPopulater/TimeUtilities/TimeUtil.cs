using System;
using System.Runtime.CompilerServices;

namespace JohnLambe.Util.TimeUtilities
{
    /// <summary>
    /// Time-related utilities.
    /// </summary>
    public static class TimeUtil
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

        #region Formats

        /// <summary>
        /// Full date and time in metric format.
        /// </summary>
        public const string MetricFormat = "yyyy-MM-dd hh:mm:ss";

        /// <summary>
        /// ISO-8601 Basic date/time format, including the full time (with seconds).
        /// </summary>
        public const string Iso8601Format = "yyyyMMdd'T'hhmmss";

        #endregion

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

        /// <summary>
        /// Returns a new DateTime equal to this one with the time part changed to <paramref name="time"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ChangeTime(this DateTime value, TimeSpan time)
        {
            return value.Date + time;
        }

        #endregion

        #region IsDateOnly / IsTimeOfDay
        //| These could be extension methods, but might seem like clutter in the list in the IDE.

        //| Could add overloads for nullable types, but what should they do on null? :
        //| return true (because it is valid as a nullable date or time part) or throw an exception.

        /// <summary>
        /// True if this DateTime has no (midnight) time of day part.
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
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

        #region Start/End of Day/Month

        /// <summary>
        /// Returns a DateTime with the time part set to the end of the day (last millisecond of the day).
        /// </summary>
        /// <param name="time"></param>
        /// <returns>The end of the day of the date represented by the given value, or null if passed null.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime? EndOfDay(this DateTime? time)
            => time == null ? (DateTime?)null
                : EndOfDay(time.Value);

        /// <summary>
        /// Returns a DateTime with the time part set to the end of the day (last millisecond of the day).
        /// </summary>
        /// <param name="time">The end of the day of the date represented by the given value.</param>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime time)
            => time.Date + EndOfDayTime;

        /// <summary>
        /// Returns midnight on the first day of the month of the given date (or null if passed null).
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime? StartOfMonth(this DateTime? datetime)
            => datetime == null ? (DateTime?)null
                : StartOfMonth(datetime);

        /// <summary>
        /// Returns midnight on the first day of the month of the given date.
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime StartOfMonth(this DateTime datetime)
            => new DateTime(datetime.Year, datetime.Month, 1);

        /// <summary>
        /// Returns the end of the last day of month of the given date (or null if passed null).
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime? EndOfMonth(DateTime? datetime)
            => datetime == null ? (DateTime?)null
                : EndOfMonth(datetime.Value);

        /// <summary>
        /// Returns the end of the last day of month of the given date.
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime EndOfMonth(DateTime datetime)
            => new DateTime(datetime.Day,datetime.Month,1).AddMonths(1).AddDays(-1) + EndOfDayTime;

        /// <summary>
        /// A time just less than one day (currently one millisecond less than a day).
        /// </summary>
        public static readonly TimeSpan EndOfDayTime = new TimeSpan(0,23,59,59,999);

        #endregion

        #region SQL Server End Of Day/Month

        /// <summary>
        /// Returns a DateTime with the time part set to the end of the day (last millisecond of the day).
        /// </summary>
        /// <param name="time"></param>
        /// <returns>The end of the day of the date represented by the given value, or null if passed null.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime? SqlEndOfDay(this DateTime? time)
            => time == null ? (DateTime?)null
                : SqlEndOfDay(time.Value);

        /// <summary>
        /// Returns a DateTime with the time part set to the end of the day (last millisecond of the day).
        /// </summary>
        /// <param name="time">The end of the day of the date represented by the given value.</param>
        /// <returns></returns>
        public static DateTime SqlEndOfDay(this DateTime time)
            => time.Date + SqlEndOfDayTime;

        /// <summary>
        /// Returns the end of the last day of month of the given date (or null if passed null).
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime? SqlEndOfMonth(DateTime? datetime)
            => datetime == null ? (DateTime?)null
                : SqlEndOfMonth(datetime.Value);

        /// <summary>
        /// Returns the end of the last day of month of the given date.
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime SqlEndOfMonth(DateTime datetime)
            => new DateTime(datetime.Year, datetime.Month, 1).AddMonths(1).AddDays(-1) + SqlEndOfDayTime;

        /// <summary>
        /// A time just less than one day: The highest value less than a day that can be represented in a Microsoft SQL Server datetime type.
        /// </summary>
        public static readonly TimeSpan SqlEndOfDayTime = new TimeSpan(0, 23, 59, 59, 997);

        #endregion

        /// <summary>
        /// The current time in ISO 8601 Basic format.
        /// </summary>
        /// <returns></returns>
        public static string NowIso8601()
            => DateTime.Now.ToString(Iso8601Format);
    }
}
