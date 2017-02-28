using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.TypeConversion;
using JohnLambe.Util;

namespace DiExtension.ConfigInject.Providers
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

        public virtual bool GetValue<T>(string key, Type requiredType, out T value)
        {
            var parts = key.Split(Separator);
            var level = 0;                      // level in the heirarchical key (0 for top)
            object currentValue = Provider;     // value at this level
            foreach (var part in parts)
            {
                bool isLeaf = level == parts.Length - 1;      // true if we've reached the leaf node in the expression (key)
                Type currentRequiredType = isLeaf ? requiredType : typeof(object);   // if this is the leaf, then we expect the requested type, otherwise we don't know the type

                if (!isLeaf && currentValue == null)   // if this is not the leaf node and the value so far is null
                {                                      // fail - we can't evaluate the next level
                    value = default(T);
                    return false;
                }

                // Evaluate the key at this level, replacing currentValue:
                if (currentValue is IConfigProvider)
                {
                    if(! ((IConfigProvider)currentValue).GetValue(parts[level], currentRequiredType, out currentValue) )
                    {
                        value = default(T);
                        return false;
                    }
                }   //TODO: try evaluating property if IConfigProvider lookup fails??
                else
                {
                    if (!ObjectValueProvider.StaticGetValue<object>(currentValue, parts[level], currentRequiredType, out currentValue))
                    {
                        value = default(T);     
                        return false;
                    }
                }

                level++;
            }

            value = (T)currentValue;
            return true;

            /*
            string parentKey, childKey;
            key.SplitToVars(Separator, out parentKey, out childKey);
            //TODO: Support multiple levels
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
            */
        }

        /// <summary>
        /// The wrapped provider.
        /// </summary>
        protected virtual IConfigProvider Provider { get; private set; }

        /// <summary>
        /// Separator for levels of hierarchical keys.
        /// </summary>
        protected const char Separator = '.';
    }
}
