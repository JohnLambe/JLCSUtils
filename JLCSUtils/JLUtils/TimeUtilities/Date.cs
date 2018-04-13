using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace JohnLambe.Util.TimeUtilities
{
    /// <summary>
    /// An immutable date with no time part.
    /// <para>This has members equivalent to (most of) the members of <see cref="DateTime"/> relevant to the Date part.</para>
    /// <para>For consistency with <see cref="DateTime"/>, this uses <see cref="int"/>
    /// for parts of the date, and handles negative or out-of-range values similarly.
    /// </para>
    /// </summary>
    //| 'int' is used for parts of the date (rather than 'byte' and 'word') for consistency with DateTime.
    //TODO: [ComVisible] - Check that this works with COM.
    [Serializable]
    public struct Date : IEquatable<Object>,
        IComparable<Date>, IComparable<DateTime>,
        ICloneable,
        IFormattable,
        ISerializable,
        IConvertible
    {
        #region Constructors

        /// <summary>
        /// Initializes to a specified number of ticks.
        /// </summary>
        /// <param name="ticks">
        /// A date and time expressed in the number of 100-nanosecond intervals that have
        ///     elapsed since January 1, 0001 at 00:00:00.000 in the Gregorian calendar (as
        ///     for the DateTime constructor).
        /// Must be a multiple of <see cref="TimeSpan.TicksPerDay"/> - no time-of-day part.
        /// </param>
        /// <param name="kind"><inheritdoc cref="DateTime.Kind"></inheritdoc></param>
        public Date(long ticks, DateTimeKind kind = DateTimeKind.Unspecified)
        {
            if (ticks % TimeSpan.TicksPerDay != 0)            // must not have a time part
                throw new OverflowException("Invalid `ticks` value for Date (" + ticks + ")");
            _value = new DateTime(ticks,kind);
        }

        public Date(int year, int month, int day, DateTimeKind kind = DateTimeKind.Unspecified)
        {
            _value = new DateTime(year, month, day, 0, 0, 0, kind);
        }

        public Date(int year, int month, int day, Calendar calendar)
        {
            _value = new DateTime(year, month, day, calendar);
        }

        /// <summary>
        /// Initializes from a DateTime value. There must be no time part in this value.
        /// </summary>
        /// <param name="value"></param>
        public Date(DateTime value)
        {
            if (!TimeUtil.IsDateOnly(value))
                throw new ArgumentException("Can't cast DateTime to Date because it has a time part");
            _value = value;
        }

        /*  // Not needed because Date can be implicitly cast to DateTime:
        /// <summary>
        /// Copy an existing Date.
        /// </summary>
        /// <param name="value"></param>
        public Date(Date value)
        {
            _value = value._value;
        }
        */

        #endregion

        /// <summary>
        /// The underlying value.
        /// </summary>
        private DateTime _value;

        /// <summary>
        /// The Date value as a <see cref="DateTime"/>.
        /// </summary>
        public DateTime AsDateTime => _value;

        /// <summary>
        /// Creates a Date from the date part of the given date and time.
        /// </summary>
        public static Date Truncate(DateTime value) => new Date(value.Date);

        public override string ToString()
            => _value.ToShortDateString();

        /// <summary>
        /// Combine this <see cref="Date"/> with a <see cref="Time"/> to form a DateTime.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public DateTime CombineTime(Time time)
            => _value + (TimeSpan)time;

        #region BinaryDate
        // Bits 2-31: Number of days since 01/01/0001.
        // Bits 0-1:  Kind.
        //| This actually uses 25 bits (23 for the date value) to support dates up to DateTime.Max (31/12/9999).
        //| It could be stored in 24 bits (3 bytes - useful for more efficient serialization) if we multiplied by 3 instead of using
        //| two full bits for the Kind.

        /// <summary>
        /// Converts this date to an integer.
        /// </summary>
        /// <returns></returns>
        public int ToBinaryDate()
        {
            int kind = (int)Kind;
            if (kind >= 4)          // if a later version of .NET adds more, we don't support them
            {
                kind = (int)DateTimeKind.Unspecified;
                Debug.Assert(false, "Unsupported DateTimeKind value: " + Kind);
            }
            return (int)((_value - BaseDate).TotalDays) << 2 | kind;
            // Put Kind in the low 2 bits.
            // This enables sorting/comparing the binary values, as much as possible in order of the date value.
        }

        public static Date FromBinaryDate(int binary)
            => new Date(BaseDate.AddDays(binary >> 2).Ticks, (DateTimeKind)(binary % 3));

        public static Date FromBinary(long binary)
            => new Date(DateTime.FromBinary(binary));

        public static readonly DateTime BaseDate
            = DateTime.MinValue;  // 01/01/0001
//            = DateTime.MinValue.AddDays( (int)((DateTime.MaxValue - DateTime.MinValue).TotalDays) / 2 ); // mean of DateTime.MinValue and DateTime.MaxValue.

        #endregion

        #region Methods

        public Date AddDays(int delta) => new Date(_value.AddDays(delta));

        public Date AddMonths(int delta) => new Date(_value.AddMonths(delta));

        public Date AddYears(int delta) => new Date(_value.AddYears(delta));

        public string ToLongDateString()
            => _value.ToLongDateString();

        public string ToShortDateString()
            => _value.ToShortDateString();

        public bool IsDaylightSavingTime()
            => _value.IsDaylightSavingTime();

        public string[] GetDateTimeFormats(char format, IFormatProvider provider)
            => _value.GetDateTimeFormats(format, provider);

        public string[] GetDateTimeFormats(IFormatProvider provider)
            => _value.GetDateTimeFormats(provider);

        public string[] GetDateTimeFormats(char format)
            => _value.GetDateTimeFormats(format);

        public string[] GetDateTimeFormats()
            => _value.GetDateTimeFormats();

        public long ToBinary()
            => _value.ToBinary();

        public long ToFileTimeUtc()
            => _value.ToFileTimeUtc();

        public long ToFileTime()
            => _value.ToFileTime();

        public double ToOADate()
            => _value.ToOADate();

        public DateTime ToUniversalTime()
            => _value.ToUniversalTime();

        public DateTime ToLocalTime()
            => _value.ToLocalTime();

        #endregion

        #region Properties
        // Properties for parts of the date:

        public int Year => _value.Year;

        public int Month => _value.Month;

        public int Day => _value.Day;

        public DayOfWeek DayOfWeek => _value.DayOfWeek;

        public int DayOfYear => _value.DayOfYear;

        public long Ticks => _value.Ticks;

        /// <summary>
        /// Indicates whether this date value is local, UTC or unspecified.
        /// </summary>
        /// <remarks>This is relevant to just a date, since it determines when a day starts and ends.</remarks>
        public DateTimeKind Kind => _value.Kind;

        #endregion

        #region Casting

        public static implicit operator DateTime(Date value)
            => value.AsDateTime;

        /// <summary>
        /// 
        /// Requires an explicit cast because the conversion may fail (if there is a time part).
        /// </summary>
        /// <param name="value"></param>
        public static explicit operator Date(DateTime value)
            => new Date(value);

        #endregion

        #region + and - operators

        /// <summary>
        /// Adds a TimeSpan to the date. Only the Days part is added.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Date operator +(Date date, TimeSpan delta)
            => date.AddDays(delta.Days);

        /// <summary>
        /// Subtracts a TimeSpan to from date. Only the Days part is subtracted.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public static Date operator -(Date date, TimeSpan delta)
            => date.AddDays(-delta.Days);

        public static TimeSpan operator -(Date date1, Date date2)
            => date1.AsDateTime - date2.AsDateTime;

        #endregion

        #region Add / Subtract

        public Date Add(TimeSpan ts)
            => this + ts;

        public Date Subtract(TimeSpan ts)
            => this - ts;

        #endregion

        #region Comparison operators

        public static bool operator == (Date a, Date b)
            => a._value == b._value;
        public static bool operator != (Date a, Date b)
            => a._value != b._value;

        public static bool operator < (Date a, Date b)
            => a._value < b._value;
        public static bool operator > (Date a, Date b)
            => a._value > b._value;
        public static bool operator >= (Date a, Date b)
            => a._value >= b._value;
        public static bool operator <= (Date a, Date b)
            => a._value >= b._value;

        #endregion

        #region Equals and GetHashCode

        public override bool Equals(object other)
        {
//            if (other == null)
//                return _value == null;         // equal if both are null

            if (other is Date)
                return _value == ((Date)other)._value;

            else if (other is DateTime)
                return _value == (DateTime)other;
//            else if (other is DateTime?)
//                return _value == (DateTime?)other;

            else
                return base.Equals(other);
        }

        public override int GetHashCode()
            => _value.GetHashCode();

        #endregion

        #region IComparable

        /*
        public int CompareTo(object obj)
        {
            if(obj is Date)
            {
                if (this > (Date)obj)
                    return 1;
                else if (this < (Date)obj)
                    return -1;
                else
                    return 0;
            }
            else if(obj is DateTime)
            {
                if(this > (DateTime)obj)
                    return 1;
                else if (this < (DateTime)obj)
                    return -1;
                else
                    return 0;
            }
            throw new NotImplementedException();
        }
        */

        public int CompareTo(Date other)
        {
            if (this > other)
                return 1;
            else if (this < other)
                return -1;
            else
                return 0;
        }

        public int CompareTo(DateTime other)
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
        /// Creates a new <see cref="Date"/> instance with the same value as this one.
        /// </summary>
        /// <returns></returns>
        public object Clone()
            => new Date(this);

        #endregion

        #region IFormattable

        /// <summary>
        /// Formats this in the same way as a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
            => _value.ToString(format, formatProvider);

        #endregion

        #region ISerializable

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ((ISerializable)_value).GetObjectData(info, context);
        }

        #endregion

        #region IConvertible

        public TypeCode GetTypeCode()
        {
            return _value.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToBoolean(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToChar(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToSByte(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToByte(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToInt16(provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToUInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToInt32(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToUInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToInt64(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToUInt64(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToSingle(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToDouble(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToDecimal(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)_value).ToDateTime(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return _value.ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)_value).ToType(conversionType, provider);
        }

        #endregion

    }

}
