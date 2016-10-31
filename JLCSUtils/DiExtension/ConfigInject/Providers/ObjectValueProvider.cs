using JohnLambe.Util.Reflection;
using JohnLambe.Util.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.ConfigInject.Providers
{
    /// <summary>
    /// Implements <see cref="IConfigProvider"/> to return property values of a given object.
    /// </summary>
    public class ObjectValueProvider : IConfigProvider
    {
        /// <summary>
        /// </summary>
        /// <param name="source">Object whose properties are to be returned.
        /// If null is passed, <see cref="GetValue{T}(string, Type, out T)"/> will always return false.</param>
        public ObjectValueProvider(object source)
        {
            this._source = source;
        }

        public virtual bool GetValue<T>(string key, Type requiredType, out T value)
        {
            return StaticGetValue<T>(_source, key ,requiredType, out value);
/*            var property = _source?.GetType()?.GetProperty(key);
            if(property != null)
            {
                value = GeneralTypeConverter.Convert<T>(property.GetValue(_source, Collections.EmptyCollection<object>.EmptyArray), requiredType);
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
*/
        }

        public static bool StaticGetValue<T>(object source, string key, Type requiredType, out T value)
        {
            //            var property = ReflectionUtils.GetProperty(source, key);
            ////            var property = source?.GetType()?.GetProperty(key);
            object objectValue = ReflectionUtils.TryGetPropertyValue<object>(source, key);
            if (objectValue != null)
            {
                value = GeneralTypeConverter.Convert<T>(objectValue
                    //property.GetValue(source, JohnLambe.Util.Collections.EmptyCollection<object>.EmptyArray)
                    , requiredType
                    );
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        protected readonly object _source;
    }
}
