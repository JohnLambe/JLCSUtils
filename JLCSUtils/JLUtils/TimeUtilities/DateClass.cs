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
        //TODO: Implement (be delegation to Value) other methods and operators.

        public virtual DateTime Value
        {
            get { return _value; }
            set { _value = PreprocessDateTime(value); }
        }

        protected abstract DateTime PreprocessDateTime(DateTime value);

        protected DateTime _value;
    }

    /// <summary>
    /// Holds a date whose time part is always 00:00.
    /// </summary>
    public class StartDate : DateClass
    {
        protected override DateTime PreprocessDateTime(DateTime value)
        {
            return Value.Date;
        }
    }

    /// <summary>
    /// Holds a date whose time part is always the end of the day.
    /// </summary>
    public class EndDate : DateClass
    {
        protected override DateTime PreprocessDateTime(DateTime value)
        {
            return TimeUtils.EndOfDay(Value);
        }
    }
}
