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
        DateTime Time { get; }

        long TimeTicks { get; }

        TimeZone CurrentTimeZone { get; }

        TimeZoneInfo LocalTimeZoneInfo { get; }
    }

    /// <summary>
    /// Standard implementation of <see cref="ITimeService"/>.
    /// </summary>
    public class TimeService : ITimeService
    {
        public virtual DateTime Time => DateTime.Now;

        public virtual long TimeTicks => Time.Ticks;  // same as Time.Ticks;
                                                      // Environment.TickCount is a 32-bit version of this.

        public virtual TimeZone CurrentTimeZone => TimeZone.CurrentTimeZone;

        public virtual TimeZoneInfo LocalTimeZoneInfo => TimeZoneInfo.Local;
    }

    /// <summary>
    /// A mock implementation of <see cref="ITimeService"/> (for use in unit testing).
    /// </summary>
    public class MockTimeService : ITimeService
    {
        /// <summary>
        /// Uses a default of 1/1/2000.
        /// </summary>
        public MockTimeService()
        {
            Time = new DateTime(1, 1, 2000);
        }

        public MockTimeService(DateTime value)
        {
            Time = value;
        }

        public virtual DateTime Time { get; set; } = DateTime.Now;

        public virtual long TimeTicks => Time.Ticks;

        public virtual TimeZone CurrentTimeZone { get; set; } = TimeZone.CurrentTimeZone;

        public virtual TimeZoneInfo LocalTimeZoneInfo { get; set; } = TimeZoneInfo.Local;
    }

}
