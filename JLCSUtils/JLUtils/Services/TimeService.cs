using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Services
{
    /// <summary>
    /// A service for accessing the system time and time zone.
    /// </summary>
    public interface ITimeService
    {
        /// <summary>
        /// The current time (equivalent to <see cref="DateTime.Now"/>).
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// The current time as UTC.
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// The current time in ticks.
        /// </summary>
        long NowTicks { get; }

        /// <summary>
        /// The number of ticks since the system started.
        /// </summary>
        int TickCount { get; }

        /// <summary>
        /// The current time zone.
        /// </summary>
        TimeZone CurrentTimeZone { get; }

        /// <summary>
        /// The current time zone.
        /// </summary>
        TimeZoneInfo LocalTimeZoneInfo { get; }

        /// <summary>
        /// Sleep the calling thread for a specified time or delay for this time before returning.
        /// <para>Implementations may do something else in the same thread during this time.</para>
        /// </summary>
        /// <param name="duration">Time interval in milliseconds.
        /// Does nothing if negative. The thread yields if this is 0.
        /// </param>
        /// <returns>How it ended. See <see cref="SleepEndReason"/>.</returns>
        SleepEndReason Sleep(int duration);

        /// <summary>
        /// Same as <see cref="Sleep(int)"/> except that the duration is a <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="duration">Time interval.</param>
        /// <returns>How it ended. See <see cref="SleepEndReason"/>.</returns>
        SleepEndReason Sleep(TimeSpan duration);

        /// <summary>
        /// Same as <see cref="Sleep(int)"/> except that it waits until a specified time.
        /// </summary>
        /// <param name="endTime">Time at which to return. Does nothing if this is in the past.</param>
        /// <returns>How it ended. See <see cref="SleepEndReason"/>.</returns>
        SleepEndReason SleepUntil(DateTime endTime);
    }

    /// <summary>
    /// How a call to <see cref="ITimeService.Sleep(int)"/> or similar ended.
    /// </summary>
    public enum SleepEndReason
    {
        /// <summary>
        /// The requested time interval expired.
        /// </summary>
        Expired,
        /// <summary>
        /// Something caused it to return early.
        /// </summary>
        Interrupted
    }


    /// <summary>
    /// Base class for <see cref="ITimeService"/> implementations, with some default method implementations.
    /// </summary>
    public abstract class TimeServiceBase
    {
        /// <summary>
        /// <see cref="ITimeService.Sleep(int)"/>.
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public virtual SleepEndReason Sleep(int duration)
        {
            if (duration >= 0)
                System.Threading.Thread.Sleep(duration);
            return SleepEndReason.Expired;
        }

        /// <summary>
        /// <see cref="ITimeService.Sleep(TimeSpan)"/>.
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public virtual SleepEndReason Sleep(TimeSpan duration)
        {
            return Sleep(duration.Milliseconds);
        }

        /// <summary>
        /// <see cref="ITimeService.SleepUntil(DateTime)"/>.
        /// </summary>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public abstract SleepEndReason SleepUntil(DateTime endTime);
    }

    /// <summary>
    /// Standard implementation of <see cref="ITimeService"/>.
    /// </summary>
    public class TimeService : TimeServiceBase, ITimeService
    {
        public virtual DateTime Now => DateTime.Now;

        public virtual DateTime UtcNow => DateTime.UtcNow;

        public virtual long NowTicks => Now.Ticks;  // same as Time.Ticks;

        public virtual int TickCount => Environment.TickCount;

        public virtual TimeZone CurrentTimeZone => TimeZone.CurrentTimeZone;

        public virtual TimeZoneInfo LocalTimeZoneInfo => TimeZoneInfo.Local;

        public override SleepEndReason SleepUntil(DateTime endTime)
        {
            return Sleep(endTime - Now);
        }
    }

    /// <summary>
    /// A mock implementation of <see cref="ITimeService"/> (for use in unit testing),
    /// providing the ability to simulate a time.
    /// </summary>
    public class MockTimeService : TimeServiceBase, ITimeService
    {
        /// <summary>
        /// Uses a default of 1/1/2000.
        /// </summary>
        public MockTimeService()
        {
            Now = new DateTime(1, 1, 2000);
        }

        public MockTimeService(DateTime value)
        {
            Now = value;
        }

        public virtual DateTime Now { get; set; } = DateTime.Now;

        public virtual DateTime UtcNow => Now.ToUniversalTime();

        public virtual long NowTicks => Now.Ticks;

        public virtual int TickCount { get; set; } = 1000;

        public virtual TimeZone CurrentTimeZone { get; set; } = TimeZone.CurrentTimeZone;

        public virtual TimeZoneInfo LocalTimeZoneInfo { get; set; } = TimeZoneInfo.Local;

        public override SleepEndReason Sleep(int duration)
        {
            base.Sleep(Math.Min(duration, MaximumSleepTime));
            if(AdvanceTimeOnSleep)
                AdvanceTime((int)(duration / TimeSpan.TicksPerMillisecond));
            return SleepEndReason.Expired;
        }

        public override SleepEndReason SleepUntil(DateTime endTime)
        {
            return Sleep(endTime - Now);
        }

        #region Mock features
        // Additional features for use in mocking.
        // Note: The properties can also be set directly.

        public virtual void AdvanceTime(int ticks)
        {
            // simulate time advancing:
            TickCount += ticks;
            Now = Now.AddTicks(ticks);
        }

        /// <summary>
        /// If <see cref="Sleep(int)"/> is called with a duration longer than this, it will sleep for this duration only.
        /// This can be used to make tests run without delays that 
        /// </summary>
        public virtual int MaximumSleepTime { get; set; } = int.MaxValue;

        /// <summary>
        /// Iff true, the simulated time advances when <see cref="Sleep(int)"/> is called.
        /// </summary>
        public virtual bool AdvanceTimeOnSleep { get; set; } = true;

        #endregion
    }

}
