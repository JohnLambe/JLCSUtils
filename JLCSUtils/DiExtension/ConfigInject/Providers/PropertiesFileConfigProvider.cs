using JohnLambe.Util.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.ConfigInject.Providers
{
    /// <summary>
    /// Provider that reads a file in the Java Properties format.
    /// <para>
    /// Each line is in the format: &lt;Name&gt; "=" &lt;Value&gt;<br/>
    /// Where &lt;Name&gt; can be a heirarchical key with levels separated by '.'.
    /// </para>
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class PropertiesFileConfigProvider<TValue> : DictionaryConfigProviderBase<TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">Filename of the properties file.</param>
        /// <param name="root">The root property to use. null for the root.</param>
        public PropertiesFileConfigProvider(string filename, string root = null)
            : this(File.ReadAllLines(filename), root)
        {
        }

        /// <summary>
        /// Provide the contents as a string array.
        /// </summary>
        /// <param name="values"></param>
        public PropertiesFileConfigProvider(string[] data, string root = null)
        {
            if (root != null)
                root = root + ".";
            DictionaryUtil.ImportText(_values, data, root);
        }
    }

        /*
        public class PropertiesFileConfigProvider<TValue> : FileConfigProviderBase
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="filename">Filename of the XML document.</param>
            /// <param name="xPath">The root property to use. null for the root.</param>
            public PropertiesFileConfigProvider(string filename, string root = null)
            {
                _data = File.ReadAllLines(filename);
                _root = root;
            }

            /// <summary>
            /// Provide the contents as a string array.
            /// </summary>
            /// <param name="values"></param>
            public PropertiesFileConfigProvider(string[] data)
            {
                this._data = data;
            }

            public override bool GetValue<T>(string key, Type requiredType, out T value)
            {
                key = _root + "." + key;
                TValue valueOurType;   // the value in the Type we have
                bool resolved = _data.TryGetSubNodeValue(key, requiredType, out valueOurType);
                // _xml.SelectSingleNode(key).InnerText
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

            /// <summary>
            /// The underlying text data.
            /// </summary>
            protected readonly string[] _data;
            protected readonly string _root;
        }
        */
}
