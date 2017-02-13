using JohnLambe.Util.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DiExtension.ConfigInject.Providers
{
    public abstract class FileConfigProviderBase : IConfigProvider
    {
        public abstract bool GetValue<T>(string key, Type requiredType, out T value);
    }

    /*
    public class XmlConfigProvider<TValue> : FileConfigProviderBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">Filename of the XML document.</param>
        /// <param name="xPath">The xpath of the node to use. null to use the whole XML document.</param>
        public XmlConfigProvider(string filename, string xPath = null)
        {
            var doc = new XmlDocument();
            doc.LoadXml(filename);
            if (xPath == null)
            {
                _xml = doc;
            }
            else
            {
                _xml = doc.SelectSingleNode(xPath);
            }
        }

        /// <summary>
        /// Create as a wrapper for an existing XML node.
        /// </summary>
        /// <param name="values"></param>
        public XmlConfigProvider(XmlNode xml)
        {
            this._xml = xml;
        }

        public override bool GetValue<T>(string key, Type requiredType, out T value)
        {
            key = key.Replace('.', '/');              // Convert key (with "." between levels) to xPath.
            TValue valueOurType;   // the value in the Type we have
            bool resolved = _xml.TryGetSubNodeValue(key, requiredType, out valueOurType);
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
        /// The underlying XML data.
        /// </summary>
        protected readonly XmlNode _xml;
    }
    */

}
