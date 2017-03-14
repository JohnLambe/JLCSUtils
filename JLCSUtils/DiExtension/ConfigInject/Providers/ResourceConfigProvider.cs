using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Globalization;
using JohnLambe.Util.TypeConversion;
using System.Reflection;

namespace DiExtension.ConfigInject.Providers
{
    public class ResourceConfigProvider : IConfigProvider
    {
        public ResourceConfigProvider(ResourceManager resourceManager, CultureInfo culture)
        {
            ResourceMgr = resourceManager;
            Culture = culture;
        }

        /*
        public ResourceConfigProvider(Assembly assm)
        {
            var resourceManager = new global::System.Resources.ResourceManager(assm. + ".Properties.Resources", assm);
            if (resourceManager == null)
                throw new ArgumentException("No ResourceManager found in assembly " + assm);
            ResourceMgr = resourceManager;
        }
        */

        public virtual bool GetValue<T>(string key, Type requiredType, out T value)
        {
            if(key==null)
            {
                value = default(T);
                return false;
            }

            value = GeneralTypeConverter.Convert<T>(ResourceMgr.GetObject(key),requiredType);

            return value != null;
        }

        protected readonly ResourceManager ResourceMgr;
        protected readonly CultureInfo Culture;
    }
}
