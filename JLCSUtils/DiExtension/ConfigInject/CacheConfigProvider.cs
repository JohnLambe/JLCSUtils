using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.ConfigInject
{
    /// <summary>
    /// Proxy that adds a cache in front of an existing provider.
    /// Values are cached the first time each key is looked up.
    /// </summary>
    public class CacheConfigProvider : IConfigProvider
    {
        public CacheConfigProvider(IConfigProvider internalProvider)
        {
            this.InternalProvider = internalProvider;
        }

        public virtual bool GetValue<T>(string key, Type requiredType, out T value)
        {
            object result;
            if (!Cache.TryGetValue(key, out result))
            {   // if not in cache
                if(InternalProvider.GetValue(key, requiredType, out result))
                {                                   // if successfully got a value
                    Cache[key] = result;            // add to cache, even if null (or default(T))
                }
                else
                {
                    Cache[key] = NoMapping;            // otherwise, add an entry to indicate that there is no value for this key
                }
            }
            if(result == NoMapping)
            {
                value = default(T);
                return false;
            }
            value = (T)result;
            return result != null;
        }

        protected readonly IConfigProvider InternalProvider;
        protected readonly IDictionary<string, object> Cache = new Dictionary<string, object>();

        protected static object NoMapping = new object();
    }
}
