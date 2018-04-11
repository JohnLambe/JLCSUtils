using JohnLambe.Util.TimeUtilities;
using JohnLambe.Util.TypeConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Validation
{
    // Validation/metadata of time-related types:

    /// <summary>
    /// Validates a date/time or timespan value.
    /// </summary>
    public abstract class TimeOrTimeSpanValidationAttribute : ValidationAttributeBase
    {
        /// <summary>
        /// The components of the date/time that are present.
        /// </summary>
        public virtual TimePrecision TimeParts { get; set; }

        /// <summary>
        /// Number of decimal places in seconds (for display and entry).
        /// (This is ignored if <see cref="TimeParts"/> does not include <see cref="TimePrecision.SecondFraction"/>.)
        /// </summary>
        public virtual int SecondsDecimalPlaces { get; set; }

        /// <summary>
        /// True to round values (on validation) to the precision specified by <see cref="TimeParts"/>
        /// and <see cref="SecondsDecimalPlaces"/>.
        /// </summary>
        public virtual bool Round { get; set; }

        /// <summary>
        /// true iff null is allowed.
        /// </summary>
        public virtual bool Nullable { get; set; } = true;


        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);

            if (!Nullable && value == null)
            {
                results.AddBlankError(validationContext);
            }
        }
    }

    /// <summary>
    /// Specifies that a property is a date and/or time value, and provides metadata relating to it.
    /// It may be a time of day, but not a time interval.
    /// </summary>
    public class DateTimeValidationAttribute : TimeOrTimeSpanValidationAttribute
    {
        public override string GeneralDescription => "A date and/or time.";

        /// <summary>
        /// What the time part should be populated with when the value represents only a date.
        /// </summary>
        public virtual TimePartOption TimePartOption { get; set; }
        //TODO: Set the time part on validation.

        public virtual TimeValidationOptions Options { get; set; }

        /// <summary>
        /// Minimum (earliest) allowed value.
        /// </summary>
        public virtual DateTime Minimum { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Maximum (latest) allowed value.
        /// </summary>
        public virtual DateTime Maximum { get; set; } = DateTime.MaxValue;

        // Options to add:
        //   Restrict days of week?
        //   Min. and Max. time of day?
        //   Min. and Max. difference to current time (TimeSpans).
        //   Default to current time?

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);

            if (value != null)
            {
                DateTime timeValue = GeneralTypeConverter.Convert<DateTime>(value);

                if (timeValue < Minimum || timeValue > Maximum)
                    results.Add(validationContext?.DisplayName + " is outside the allowed range");  //TODO Show range

                switch (TimePartOption)
                {
                    case TimePartOption.EndOfDay:
                        timeValue = timeValue.EndOfDay();
                        value = timeValue;
                        break;
                    case TimePartOption.StartOfDay:
                        timeValue = timeValue.Date;
                        value = timeValue;
                        break;
                }

                if (validationContext.GetState().HasFlag(ValidationState.LiveInput) && Options != TimeValidationOptions.Any)
                {
                    var now = DateTime.Now;  //TODO use ITimeService
                    if (timeValue < now && !(Options.HasFlag(TimeValidationOptions.AllowPast)))
                    {
                        results.Add(validationContext?.DisplayName + " must not be in the past");
                    }
                    else if (timeValue > now && !(Options.HasFlag(TimeValidationOptions.AllowFuture)))
                    {
                        results.Add(validationContext?.DisplayName + " must not be in the future");
                    }
                }
            }

            //TODO other properties
        }
    }

    /// <summary>
    /// Indicates that the data item is a time interval (duration).
    /// <para>
    /// This can be used on <see cref="TimeSpan"/> and numeric types.
    /// </para>
    /// </summary>
    public class TimeSpanValidationAttribute : TimeOrTimeSpanValidationAttribute
    {
        public override string GeneralDescription => "A time interval.";

        /// <summary>
        /// Where the attributed item is of a numeric type (not a TimeSpan or DateTime),
        /// it is multiplied by this value to convert it to a <seealso cref="TimeSpan"/>.
        /// </summary>
        public virtual TimeSpan Multiplier { get; set; }

        /// <summary>
        /// Minimum allowed value.
        /// </summary>
        public virtual TimeSpan Minimum { get; set; } = TimeSpan.MinValue;

        /// <summary>
        /// Maximum allowed value.
        /// </summary>
        public virtual TimeSpan Maximum { get; set; } = TimeSpan.MaxValue;

        protected override void IsValid(ref object value, ValidationContext validationContext, ValidationResults results)
        {
            base.IsValid(ref value, validationContext, results);
            //TODO: Multiplier
            TimeSpan timeValue = GeneralTypeConverter.Convert<TimeSpan>(value);

            if (timeValue < Minimum || timeValue > Maximum)
                results.Add(validationContext?.DisplayName + " is outside the allowed range");  //TODO Show range

            //TODO other properties
        }

    }

    /// <summary>
    /// Specifies which parts of a date/time or timespan value are present or relevant.
    /// </summary>
    /// <seealso cref="TimePartOption"/>
    [Flags]
    public enum TimePrecision
    {
        Year = 0x80,
        Month = 0x40,
        Day = 0x20,
        Hour = 0x10,
        Minute = 0x08,
        Second = 0x04,
        SecondFraction = 0x02,
        [Obsolete]
        SecondsFraction = SecondFraction,

        Date = Year | Month | Day,
        TimeOfDay = Hour | Minute | Second | SecondFraction,
        Full = Date | TimeOfDay
    }

    /// <summary>
    /// Specifies what the time part of a datetime value holds.
    /// </summary>
    public enum TimePartOption
    {
        Unspecified = 0,

        /// <summary>
        /// The value represents a date and time.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// The value represents a date only.
        /// The time part should be midnight (00:00:00).
        /// </summary>
        StartOfDay,

        /// <summary>
        /// The value represents a date only.
        /// The time part should be latest value representable (~23:59:59.999).
        /// It may be the highest value representable in a system (e.g. a database) that the value is stored in rather than <see cref="DateTime"/>.
        /// <para>
        /// This may be used for dates at the end of a range (or expiry dates or deadlines that include the day itself),
        /// so that comparing a full date/time (less than or greater than) to this value yields the expected result.
        /// </para>
        /// </summary>
        EndOfDay,
    }

    [Flags]
    public enum TimeValidationOptions
    {
        /// <summary>
        /// Time values in the past are allowed.
        /// </summary>
        AllowPast = 1,

        /// <summary>
        /// Time values in the future are allowed.
        /// </summary>
        AllowFuture = 2,

        Any = AllowPast | AllowFuture
    }
}
