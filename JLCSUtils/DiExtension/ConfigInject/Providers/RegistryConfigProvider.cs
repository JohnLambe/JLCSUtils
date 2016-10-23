using JohnLambe.Util.TypeConversion;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.ConfigInject.Providers
{
    /// <summary>
    /// Reads Registry entries under a given key.
    /// </summary>
    public class RegistryConfigProvider : IConfigProvider
    {
        public RegistryConfigProvider(string baseKey)
        {
            this.BaseKey = baseKey;
        }

        public virtual string BaseKey { get; set; }

        public virtual bool GetValue<T>(string key, Type requiredType, out T value)
        {
            //TODO: Support multiple levels, with `key` being a path
            object valueObject = Registry.GetValue(BaseKey, key, null);
            value = GeneralTypeConverter.Convert<T>(valueObject, requiredType);
            return valueObject != null;
        }
    }
}
