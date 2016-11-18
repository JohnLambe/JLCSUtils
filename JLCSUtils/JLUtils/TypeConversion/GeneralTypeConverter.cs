using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.TypeConversion
{
    /// <summary>
    /// Static class for use instead of System.Config.ChangeType(),
    /// to enable changing the implementation in future (to support more conversions).
    /// </summary>
    public static class GeneralTypeConverter
    {
        public static T Convert<T>(object source)
        {
            return (T)System.Convert.ChangeType(source, typeof(T));
        }

        public static T Convert<T>(object source, Type requiredType)
        {
            /*
            if ((source is int && requiredType == typeof(int))
                || (source is long && requiredType == typeof(long))
                )
            {
                return (T)source;
            }
            */

            return (T)System.Convert.ChangeType(source, requiredType);
        }

        /// <summary>
        /// Same as Convert&lt;object&gt;.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="requiredType"></param>
        /// <returns></returns>
        public static object Convert(object source, Type requiredType)
        {
            return Convert<object>(source, requiredType);
        }
    }
}