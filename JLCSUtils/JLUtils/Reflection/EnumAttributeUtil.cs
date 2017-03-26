using JohnLambe.Util.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    public static class EnumAttributeUtil
    {
        /// <summary>
        /// Convert from an enum value to a mapped value.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TAttrib"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TValue FromEnum<TValue, TAttrib>(Enum value, TValue defaultValue = default(TValue))
            where TAttrib : EnumMappedValueAttribute
        {
            var attrib = value.GetField().GetCustomAttribute<TAttrib>();
            //TODO: If multiple attributes are found, use the one highest in the class hierarchy (so that introducing a subclass doesn't break an existing mapping).
            return GeneralTypeConverter.Convert<TValue>(attrib?.Value);
        }

        /*
        public static Enum ToEnum<TEnum, TAttrib>(object value)
        {
            Enum x;
            x.
            typeof(TEnum)
        }
        */
    }


    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumMappedValueAttribute : Attribute
    {
        public EnumMappedValueAttribute(object value)
        {
            this.Value = value;
        }

        public virtual object Value { get; protected set; }
    }

}
