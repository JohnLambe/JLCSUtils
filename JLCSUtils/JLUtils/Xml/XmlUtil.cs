using JohnLambe.Util.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace JohnLambe.Util.Xml
{
    public static class XmlUtil
    {
        /// <summary>
        /// Load an XML Document from a file, or extract a single node from an XML file.
        /// </summary>
        /// <param name="filename">The file to load.</param>
        /// <param name="xpath">X-path of the node to return. null for the whole document.</param>
        /// <param name="nameTable"></param>
        /// <returns>the requested XML node. Returns an <see cref="XmlDocument"/> if <paramref name="xpath"/> is null or "".</returns>
        public static XmlNode LoadFromFile(string filename, string xpath = null, XmlNameTable nameTable = null)
        {
            var doc = new XmlDocument(nameTable);
            doc.Load(filename);
            if (!string.IsNullOrEmpty(xpath))
                return doc.SelectSingleNode(xpath);
            else
                return doc;
        }

        /// <summary>
        /// Get the value of the requested sub-node, or return a default value if the node doesn't exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="xpath">X-path to match a single node.</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetSubNodeValue<T>(this XmlNode node, string xpath, T defaultValue = default(T))
        {
            node = node?.SelectSingleNode(xpath);
            if (node == null)
                return defaultValue;
            else
                return node.GetValue<T>();
        }

        public static T GetValue<T>(this XmlNode node)
        {
            return GeneralTypeConverter.Convert<T>(node.Value);
        }

        public static void SetSubNodeValue<TValue>(this XmlNode node, string xpath, TValue value)
        {
            SetSubNodeValue(node, xpath, value, null);
        }

        private static void SetSubNodeValue<TValue>(this XmlNode node, string xpath, TValue value, string createPath)
        {
            if (node != null)
            {
                var targetNode = node.SelectSingleNode(xpath);
                if (targetNode == null)
                {
                    string restOfPath;
                    xpath = GetXpathParent(xpath,out restOfPath);
                    createPath = restOfPath + createPath;
                    if(xpath != null)
                    {
                        SetSubNodeValue<TValue>(node, xpath, value, createPath);

                    }
                }
                else  // found
                {
                    if (!string.IsNullOrEmpty(createPath))
                        node = CreatePath(node, createPath);
                    node.Value = GeneralTypeConverter.Convert<string>(value);
                }
            }
        }

        public static XmlNode CreatePath(XmlNode node, string xpath)
        {
            throw new NotImplementedException();
            //TODO
        }

        /// <summary>
        /// Return the x-path of the parent of the node specified by the given x-path, provided that it can be determined just from the path (without reference to a document).
        /// This removes the last '/' and everything after it.
        /// <para>
        /// If there is no '/' (just one level), this returns "".
        /// If <paramref name="xpath"/> is null or "", this return null (the parent is not within the given x-path).
        /// </para>
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="leaf"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If there is a "//" before the last node in the xpath, or a <paramref name="xpath"/> is "". (The parent cannot be determined just from the path in this case.)</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="xpath"/> is null.</exception>
        public static string GetXpathParent(string xpath, out string leaf)
        {
            xpath.ArgNotNull(nameof(xpath));
            if (xpath.Equals(string.Empty))   // no parent
                throw new ArgumentException("Path has no parent");
            var index = xpath.LastIndexOf('/');         //TODO: Handle '/' other than path separator
            if (index >= 0)
            {
                leaf = xpath.Substring(index + 1);               // everything after '/'
                string result = xpath.Substring(index);          // remove last '/' and everything after it
                if (result.EndsWith('/'))
                    throw new ArgumentException("Parent cannot be determined");
                return result;
            }
            else
            {
                leaf = xpath;
                return "";                              // the node that the original path was relative to is the parent
            }
        }
    }
}
