using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.TypeConversion;

namespace JohnLambe.Util.DependencyInjection.ConfigInject.Providers
{
    /// <summary>
    /// Resolves keys in the form {object}.{property} on an underlying provider.
    /// </summary>
    public class CompositeProvider : IConfigProvider
    {
        public CompositeProvider(IConfigProvider provider)
        {
            Provider = provider;
        }

        public bool GetValue<T>(string key, Type requiredType, out T value)
        {
            string parentKey, childKey;
            key.SplitToVars(Separator, out parentKey, out childKey);
            //TODO: Support mulitple levels
            if (childKey == null)
            {
                return Provider.GetValue<T>(key, requiredType, out value);
            }
            else
            {
                object parentObject;
                if(Provider.GetValue(parentKey, typeof(object), out parentObject))
                {
                    if(parentObject is IConfigProvider)
                    {
                        return ((IConfigProvider)parentObject).GetValue(childKey, requiredType, out value);
                    }   //TODO: try evaluating property if IConfigProvider lookup fails?
                    else
                    {
                        return ObjectValueProvider.StaticGetValue<T>(parentObject, childKey, requiredType, out value);
                    }
                }
                else
                {
                    value = default(T);
                    return false;
                }
            }
        }

        /// <summary>
        /// The wrapped provider.
        /// </summary>
        protected IConfigProvider Provider { get; private set; }

        /// <summary>
        /// Separator for levels of hierarchical keys.
        /// </summary>
        protected const char Separator = '.';
    }
}
