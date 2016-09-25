using System;

namespace JohnLambe.Util.Types
{
    /// <summary>
    /// Generic type for an ID (a value used to identify something).
    /// <para>Since it is an ID, most operations of the underlying type are not supported:
    /// One generally shouldn't manipulate it; it is just assigned and compared.</para>
    /// <para>It can be cast explicitly to its underlying type in order to assign a value
    /// to a new instance and read the value for external storage or transmission.
    /// It can't be cast implicitly, since that would allow things that don't make sense for an ID.
    /// (These can still be done with an explicit cast, but that makes it clear that this is an ID).</para>
    /// </summary>
    /// <typeparam name="T">The underlying type of the ID value.</typeparam>
    public struct ID<T> : IEquatable<ID<T>>
    {
        private T _value;

        public ID(T value)
        {
            _value = value;
        }

        public bool Equals(ID<T> other)
        {
            return other._value.Equals(this._value);
        }

        // Explicit casting is allowed, since this type has to get its value from somehwere,
        // and storing it persistently or transmitting it to other systems will probably
        // require converting back to the original type.
        // Implicit casting is not supported, since most operations supported by the underlying type
        // are probably not relevant for an ID.

        public static explicit operator T(ID<T> value)
            => value._value;

        public static explicit operator ID<T>(T value)
            => new ID<T>(value);
    }


    // Convenience type equivalent to ID<long>:

    /// <summary>
    /// Type for an integer ID.
    /// <para>Since it is an ID, most numeric operations on it are invalid:
    /// It can be assigned and compared for equality, but can't be added, etc.,
    /// and whether an ID is higher or lower than another has no relevance (The &lt; and &gt; operators are not supported).</para>
    /// <para>Converting to integer types requires an explicit cast.</para>
    /// </summary>
    public struct IntID : IEquatable<IntID>
    {
        private long _value;

        public IntID(long value)
        {
            _value = value;
        }

        public bool Equals(IntID other)
        {
            return other._value == this._value;
        }

        public static explicit operator long(IntID value)
            => value._value;

        public static explicit operator IntID(long value)
            => new IntID(value);
    }

}
