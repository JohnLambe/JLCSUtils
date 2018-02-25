using JohnLambe.Util;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.TypeConversion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JohnLambe.DataPopulater
{
    public class XmlObjectLoader
    {
        /// <summary>
        /// Convert XML to an object.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual object Parse(XmlNode source, Type type = null)
        {
            string className = source.Name;
            if(source is XmlDocument)
            {
                return Parse(((XmlDocument)source).DocumentElement);
            }

            if (className == "Value")
            {   // simple value
                if (type == null)
                    return source.InnerText;
                else
                    return GeneralTypeConverter.Convert<object>(source.InnerText, type);
            }
            else
            {
                var instance = CreateInstance(className);
                //Later version: Populate contructor arguments from XML nodes

                Populate(instance, source);

                return instance;
            }
        }

        /// <summary>
        /// Populate properties defined in XML, on a given instance.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public virtual void Populate(object target, XmlNode source)
        {
            foreach(XmlNode propertyNode in source.ChildNodes)
            {
                if (propertyNode.Name?.StartsWith(PropertyPrefix) ?? false)
                {
                    if (!(propertyNode is XmlComment))
                    {
                        string propertyName;
                        if (propertyNode.Name == "Property")
                        {
                            propertyName = propertyNode.Attributes["name"].Value;
                        }
                        else
                        {
                            propertyName = propertyNode.Name?.RemovePrefix(PropertyPrefix);
                        }
                        SetProperty(target, propertyName, propertyNode);
                    }
                }
                //Later version: Method calls
            }
        }

        public virtual void SetProperty(object target, string propertyName, XmlNode source)
        {
            var property = target.GetType().GetProperty(propertyName);

            object value;
            var typeAttrib = source.Attributes["kind"];
            if (typeAttrib?.Value == "collection" || typeAttrib?.Value == "list")   // node contains a collection
            {
                var elementType = property.PropertyType.GetGenericArguments().First();
                var collectionType = typeof(LinkedList<>).MakeGenericType(property.PropertyType.GetGenericArguments());
                var items = ReflectionUtil.Create<object>(collectionType);
                var addMethod = items.GetType().GetMethod(nameof(LinkedList<object>.AddLast), new Type[] { elementType });
                if (typeAttrib?.Value == "list")
                {
                    bool trim = source.Attributes["trim"]?.Value.Trim().Equals("1") ?? false;
                    foreach(var line in source.InnerText.Split(new string[] { "|", "\r\n", "\n" }, StringSplitOptions.None))
                    {
                        string lineValue = line;
                        if (trim)
                        {
                            lineValue = lineValue.Trim();
                            if (string.IsNullOrEmpty(lineValue))
                                continue;
                        }
                        addMethod.Invoke(items, new object[] { GeneralTypeConverter.Convert<object>(lineValue, elementType) });
                    }
                }
                else
                {
                    foreach (XmlNode node in source.ChildNodes)
                    {
                        if(!(node is XmlComment))
                            addMethod.Invoke(items, new object[] { Parse(node, elementType) });
                    }
                }
                if (source.Attributes["includeBlank"]?.Value.Trim().Equals("1") ?? false)
                {
                    addMethod.Invoke(items, new object[] { "" });
                }
                if (source.Attributes["includeNull"]?.Value.Trim().Equals("1") ?? false)
                {
                    addMethod.Invoke(items, new object[] { null });
                }
                value = items;
            }
            else if (typeAttrib?.Value == "object")    // node contains a single objects instance as its first (only) child node
            {
                value = Parse(source.FirstChild, property.PropertyType);
            }
            else  // simple value
            {
                value = source.InnerText;
                bool isNull = source.Attributes["isNull"]?.Value.Equals("1") ?? false;
                if(isNull)
                {
                    if (string.IsNullOrWhiteSpace(value.ToString()))
                        value = null;
                    else
                        throw new InvalidDataException("Both Value and null specified for proeprty " + target?.GetType()?.Name + "." + propertyName);
                }

                string targetTypeName = source.Attributes["type"]?.Value;
                Type targetType = targetTypeName == null ? null : Type.GetType(targetTypeName);
                if (targetTypeName != null && targetType == null)
                    targetType = ReflectionUtil.FindType(targetTypeName);
                if(targetType != null)
                    value = GeneralTypeConverter.Convert(value, targetType);
            }
            ReflectionUtil.TrySetPropertyValue<object>(target, propertyName, value);
        }

        public virtual object CreateInstance(string className)
        {
            var type = ReflectionUtil.FindType(Namespace + "." + className);
            return ReflectionUtil.Create<object>(type);
        }

        protected const string PropertyPrefix = "p-";
        protected const string ClassPrefix = "c-";
        protected const string MethodPrefix = "m-";
        protected const string ArgumentPrefix = "a-";

        public virtual string Namespace { get; set; }
    }
}
