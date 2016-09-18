using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Collections;
using JohnLambe.Util.TypeConversion;

namespace JohnLambe.Util.DependencyInjection.ConfigInject.Providers
{
    /// <summary>
    /// Base class for providers based on an <see cref="IDictionary"/>.
    /// </summary>
    public class DictionaryConfigProviderBase<TValue> : IConfigProvider
    {
        public virtual bool GetValue<T>(string key, Type requiredType, out T value)
        {
            TValue valueOurType;   // the value in the Type we have
            bool resolved = _values.TryGetValue(key, out valueOurType);
            if (resolved)
            {
                value = (T)GeneralTypeConverter.Convert<T>(valueOurType, requiredType);
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        protected IDictionary<string, TValue> _values = new Dictionary<string, TValue>();
    }

    /// <summary>
    /// Provider based on a dictionary, which is publicly writable.
    /// </summary>
    public class DictionaryConfigProvider<TValue> : DictionaryConfigProviderBase<TValue>
    {
        /// <summary>
        /// Accesses the live data of this provider as a dictionary.
        /// </summary>
        public virtual IDictionary<string, TValue> AsDictionary { get { return _values; } }
    }
}
