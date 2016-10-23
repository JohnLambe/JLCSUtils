using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.TypeConversion;

namespace DiExtension.ConfigInject.Providers
{
    /// <summary>
    /// Implements <see cref="IConfigProvider"/> to return environment variable values.
    /// </summary>
    public class EnvironmentVarConfigProvider : IConfigProvider
    {
        public virtual bool GetValue<T>(string key, Type requiredType, out T value)
        {
            string stringValue = Environment.GetEnvironmentVariable(key, Target);
            if(stringValue == null)
            {
                value = default(T);
                return false;
            }
            else
            {
                value = GeneralTypeConverter.Convert<T>(stringValue, requiredType);
                return true;
            }
        }

        public virtual EnvironmentVariableTarget Target { get; set; } = EnvironmentVariableTarget.User;
    }
}
