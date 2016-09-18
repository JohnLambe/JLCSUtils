using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.DependencyInjection.ConfigInject
{
    public interface IConfigProvider
    {
        /// <summary>
        /// Get a configuration value.
        /// </summary>
        /// <typeparam name="T">The returned value is cast to this.</typeparam>
        /// <param name="key"></param>
        /// <param name="requiredType">The type that the returned value should be.
        /// This must be assignable to <see cref="T"/>.
        /// A subtype of this may be returned.
        /// </param>
        /// <returns>true iff the key is found. If false, <paramref name="value"/> will be default(T).</returns>
        bool GetValue<T>(string key, Type requiredType, out T value);
    }

    /// <summary>
    /// Extension method(s) for <see cref="IConfigProvider"/>.
    /// </summary>
    public static class ConfigProviderExtension
    {
        /// <summary>
        /// Try to get a value, and return a provided default if it fails.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue">The value to be returned if the key does not exist or cannot be converted to the required type.</param>
        /// <returns></returns>
        public static T GetValue<T>(this IConfigProvider provider, string key, T defaultValue = default(T))
        {
            T value;
            if(provider.GetValue<T>(key, typeof(T), out value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
