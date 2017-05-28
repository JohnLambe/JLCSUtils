using JohnLambe.Util;
using JohnLambe.Util.Misc;
using JohnLambe.Util.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.ConfigInject.Providers
{
    public abstract class IconRepositoryBase<TKeyType, TImageType> : IIconRepository<TKeyType, TImageType>
    {
        public virtual TImageType GetIcon(TKeyType iconId, IconState state = IconState.Normal)
        {
            return LoadIcon(KeyString(iconId, state));
        }

        protected abstract TImageType LoadIcon(string iconKey);

        protected virtual string KeyString(TKeyType iconId, IconState state = IconState.Normal)
        {
            if (iconId == null)
                return null;
            return iconId.ToString() + StrUtil.BlankPropagate("-", EnumAttributeUtil.FromEnum<string,EnumMappedValueAttribute>(state));
        }
    }


    /// <summary>
    /// Adapts <see cref="IConfigProvider"/> to <see cref="IIconRepository{string, TImageType}"/>.
    /// </summary>
    /// <typeparam name="TImageType"></typeparam>
    public abstract class IconRepositoryConfigProvider<TImageType> : IconRepositoryBase<string, TImageType>, IConfigProvider
    {
        public IconRepositoryConfigProvider(IConfigProvider provider)
        {
            this.Provider = provider;
        }

        protected override TImageType LoadIcon(string iconKey)
        {
            return Provider.GetValue<TImageType>(iconKey);
        }

        public virtual bool GetValue<T>(string key, Type requiredType, out T value)
        {
            return Provider.GetValue<T>(key, requiredType, out value);
        }

        protected virtual IConfigProvider Provider { get; set; }
    }
}