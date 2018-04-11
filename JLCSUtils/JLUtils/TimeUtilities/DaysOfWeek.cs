using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.TimeUtilities
{
    //
    // Summary:
    //     Specifies the day of the week.
    [ComVisible(true)]
    [Flags]
    public enum DaysOfWeek
    {
        //
        // Summary:
        //     Indicates Sunday.
        Sunday = 1 << 0,
        //
        // Summary:
        //     Indicates Monday.
        Monday = 1 << 1,
        //
        // Summary:
        //     Indicates Tuesday.
        Tuesday = 1 << 2,
        //
        // Summary:
        //     Indicates Wednesday.
        Wednesday = 1 << 3,
        //
        // Summary:
        //     Indicates Thursday.
        Thursday = 1 << 4,
        //
        // Summary:
        //     Indicates Friday.
        Friday = 1 << 5,
        //
        // Summary:
        //     Indicates Saturday.
        Saturday = 1 << 6
    }

    public static class DaysOfWeekUtil
    {
        /*
        public static DayOfWeek ToDayOfWeek(this DaysOfWeek days)
        {
            return 
        }
        */

        public static DaysOfWeek FromDayOfWeek(DayOfWeek day)
        {
            return (DaysOfWeek)(1 << (int)day);
        }

        /// <summary>
        /// true iff the date is on of the days in <paramref name="days"/>.
        /// </summary>
        /// <param name="days"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Compare(this DaysOfWeek days, DateTime value)
        {
            return days.HasFlag(FromDayOfWeek(value.DayOfWeek));
        }

    }

}
