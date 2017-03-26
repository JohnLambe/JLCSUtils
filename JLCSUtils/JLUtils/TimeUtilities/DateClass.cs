using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.TimeUtilities
{
    //[ComplexType]
    public abstract class DateClass
    {
        //TODO: Implement (by delegation to Value) other methods and operators.

        public virtual DateTime Value
        {
            get { return _value; }
            set { _value = PreprocessDateTime(value); }
        }

        protected abstract DateTime PreprocessDateTime(DateTime value);

        public static implicit operator DateTime(DateClass value)
        {
            return value.Value;
        }

        private DateTime _value;
    }

    /// <summary>
    /// Holds a date whose time part is always 00:00.
    /// </summary>
    public class StartDate : DateClass
    {
        public StartDate(DateTime value)
        {
            this.Value = value;
        }

        protected override DateTime PreprocessDateTime(DateTime value)
        {
            return value.Date;
        }

        public static implicit operator StartDate(DateTime value)
        {
            return new StartDate(value);
        }
    }

    /// <summary>
    /// Holds a date whose time part is always the end of the day.
    /// </summary>
    public class EndDate : DateClass
    {
        public EndDate(DateTime value)
        {
            this.Value = value;
        }

        protected override DateTime PreprocessDateTime(DateTime value)
        {
            return TimeUtils.EndOfDay(value);
        }

        public static implicit operator EndDate(DateTime value)
        {
            return new EndDate(value);
        }
    }
}
