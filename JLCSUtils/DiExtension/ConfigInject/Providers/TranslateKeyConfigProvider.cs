using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.TypeConversion;
using JohnLambe.Util;
using JohnLambe.Util.Types;

namespace DiExtension.ConfigInject.Providers
{
    /// <summary>
    /// Wraps another provider and translates the keys passed to it.
    /// </summary>
    public class TranslateKeyProvider : IConfigProvider
    {
        public TranslateKeyProvider([NotNull] IConfigProvider provider, [NotNull] Func<string,string> keyTranslation)
        {
            this.Provider = provider;
            this.KeyTranslation = keyTranslation;
        }

        /// <summary>
        /// If the key begins with <paramref name="prefix"/>, it is passed to the underlying provider without the prefix,
        /// otherwise, it is not resolved.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="prefix"></param>
        public TranslateKeyProvider([NotNull] IConfigProvider provider, [NotNull] string prefix)
        {
            this.Provider = provider;
            this.KeyTranslation = key => key.StartsWith(prefix) ? key.RemovePrefix(prefix) : null;
        }

        public virtual bool GetValue<T>(string key, Type requiredType, out T value)
        {
            key = TranslateKey(key);
            if (key != null)
            {
                return Provider.GetValue<T>(key, requiredType, out value);
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        protected virtual string TranslateKey(string key)
        {
            return KeyTranslation.Invoke(key);
        }

        /// <summary>
        /// The wrapped provider.
        /// </summary>
        protected virtual IConfigProvider Provider { get; }

        protected virtual Func<string,string> KeyTranslation { get; }
    }
}
