using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Reflection;

namespace JohnLambe.Util.Text
{
    public class CaptionUtils
    {
        /// <summary>
        /// Convert a pascal-cased name (i.e. with a capital letter for the first letter of each word) 
        /// to a human-readable string (with spaces between words).
        /// </summary>
        /// <param name="s">Pascal-cased name. This may contain underscores (converted to space) and spaces.</param>
        /// <returns></returns>
        public static string PascalCaseToCaption(string s)
        {
            if (s == null)
                return null;

            bool previousWasCaptial = false;
            bool previousWasSpace = true;      // treat start of string the same as following a SPACE
            int charIndex = 0;

            StringBuilder result = new StringBuilder(s.Length * 2);

            foreach (char c in s)
            {
                if (char.IsUpper(c) || char.IsDigit(c))
                {
                    if (!previousWasSpace)
                    {
                        bool nextIsLower = char.IsLower(s.CharAt(charIndex + 1));
                        if (!previousWasCaptial || nextIsLower)        // capital preceded or followed by non-capital
                            result.Append(' ');         // insert SPACE before
                    }
                    result.Append(c);
                    previousWasCaptial = true;
                    previousWasSpace = false;
                }
                else
                {
                    previousWasCaptial = false;
                    if (c == '_' || c == ' ')
                    {
                        result.Append(' ');
                        previousWasSpace = true;
                    }
                    else
                    {
                        result.Append(c);
                        previousWasSpace = false;
                    }
                }

                charIndex++;
            }
            return result.ToString();
        }

        /// <summary>
        /// Get a caption (for display to a user) for the given property.
        /// This uses the <see cref="DescriptionAttribute"/> if there is one, otherwise the property name.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>The caption for the property. null if <paramref name="property"/> is null.</returns>
        public static string GetDisplayName(PropertyInfo property)
        {
            if (property == null)
                return null;
            else
                return GetDisplayNameFromAttribute(property)
                    ?? PascalCaseToCaption(property.Name);
        }

        [Obsolete("Use GetDisplayName")]
        public static string PropertyToCaption(PropertyInfo property)
        {
            return GetDisplayName(property);
        }

        /// <summary>
        /// Return a display name for a given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDisplayName(Type type)
        {
            if (type == null)
                return null;
            else
                return GetDisplayNameFromAttribute(type)
                    ?? PascalCaseToCaption(type.Name);
        }

        /// <summary>
        /// Returns a display name (for display to a user) for the attributed item
        /// if there is an attribute that provides one.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>The display name, or null if no supported attribute is present.</returns>
        public static string GetDisplayNameFromAttribute(ICustomAttributeProvider provider)
        {
            return provider.GetCustomAttribute<DisplayAttribute>()?.Name     // Try DataAnnotations first
                ?? provider.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName   // ComponentModel
                //?? provider.GetCustomAttribute<DescriptionAttribute>()?.Description
                ;
        }

        public static string GetDescriptionFromAttribute(ICustomAttributeProvider provider)
        {
            return provider.GetCustomAttribute<DisplayAttribute>()?.Description        // Try DataAnnotations first
                ?? provider.GetCustomAttribute<DescriptionAttribute>()?.Description;   // ComponentModel
        }
    }
}
