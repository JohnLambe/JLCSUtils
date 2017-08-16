using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;

namespace JohnLambe.Util.Text
{
    /// <summary>
    /// Utilities for working with captions or display names and generating captions by reflection.
    /// </summary>
    public class CaptionUtil
    {
        /// <summary>
        /// Convert a pascal-cased name (i.e. with a capital letter for the first letter of each word) 
        /// to a human-readable string (with spaces between words).
        /// <para>This currently may not work well with punctuation, since it is intended mainly for use with identifier names in code.
        /// </para>
        /// </summary>
        /// <param name="s">Pascal-cased name. This may contain underscores (converted to space) and spaces.
        /// If null, null is returned.
        /// </param>
        /// <returns>The human-readable name.</returns>
        [return: Nullable]
        public static string PascalCaseToCaption([Nullable] string s)
        {
            if (s == null)
                return null;

            bool previousWasCapital = false;
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
                        if (!previousWasCapital || nextIsLower)        // capital preceded or followed by non-capital
                            result.Append(' ');         // insert SPACE before
                    }
                    result.Append(c);
                    previousWasCapital = true;
                    previousWasSpace = false;
                }
                else
                {
                    previousWasCapital = false;
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
        /// Get a caption (for display to a user) for the given type member ().
        /// This uses an attribute (simlarly to <see cref="GetDisplayNameFromAttribute(ICustomAttributeProvider)"/>) if there is one, otherwise the member/property name.
        /// </summary>
        /// <param name="member">An item (usually a property or method) that may have a name or attribute that can
        /// be converted to a display name.</param>
        /// <param name="prefix">Prefix of the member name which should not be part of the display name (it is removed, if present) (case-sensitive).</param>
        /// <param name="suffix">Suffix of the member name which should not be part of the display name (it is removed, if present) (case-sensitive).</param>
        /// <returns>The caption for the property. null if <paramref name="member"/> is null.</returns>
        [return: Nullable]
        public static string GetDisplayName([Nullable] MemberInfo member, [Nullable] string prefix = null, [Nullable] string suffix = null)
        {
            if (member == null)
                return null;
            else
                return GetDisplayNameFromAttribute(member)
                    ?? PascalCaseToCaption(member.Name.RemovePrefix(prefix).RemoveSuffix(suffix));
        }

        /// <summary>
        /// Returns a display name for the type of <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        [return: Nullable]
        public static string GetTypeDisplayName([Nullable] object instance, [Nullable] string prefix = null, [Nullable] string suffix = null)
        {
            return GetDisplayName(instance?.GetType());
        }

        /// <summary>
        /// Returns the plural of the display name for <paramref name="member"/>.
        /// </summary>
        /// <param name="member"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        // Doesn't support languages with multiple plurals?
        [return: Nullable]
        public static string GetPluralDisplayName([Nullable] MemberInfo member, [Nullable] string prefix = null, [Nullable] string suffix = null)
        {
            if (member == null)
                return null;
            else
                return /*TODO: member?.GetCustomAttribute<DisplayNameAnyAttribute>()?.PluralDisplayName
                    ?? */
                    GetDisplayNameFromAttribute(member)
                    ?? PascalCaseToCaption(member.Name.RemovePrefix(prefix).RemoveSuffix(suffix));
        }

        /// <summary>
        /// Returns a human-readable name to describe the given object.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        [return: Nullable]
        public static string GetDisplayNameForObject([Nullable] object instance)
        {
            if (instance == null)
                return null;
            if (instance is Enum)
                return EnumUtil.GetDisplayName((Enum)instance);
            return GetDisplayNameForObjectWithoutToString(instance)
                ?? instance.ToString();
        }

        /// <summary>
        /// Same as <see cref="GetDisplayNameForObject"/> except that this does not use <see cref="Object.ToString"/>.
        /// This may be useful in a <see cref="Object.ToString"/> method.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string GetDisplayNameForObjectWithoutToString([Nullable] object instance)
        {
            if (instance == null)
                return null;
            if (instance is Enum)
                return EnumUtil.GetDisplayName((Enum)instance);
            return ReflectionUtil.TryGetPropertyValue<string>(instance, "Name")
                ?? ReflectionUtil.TryGetPropertyValue<string>(instance, "Description");
        }

        [Obsolete("Use GetDisplayName")]
        public static string PropertyToCaption(PropertyInfo property)
        {
            return GetDisplayName(property);
        }

        /// <summary>
        /// Returns a display name (for display to a user) for the attributed item
        /// if there is an attribute that provides one.
        /// </summary>
        /// <param name="provider">The item to get a name for.</param>
        /// <returns>The display name, or null if no supported attribute is present.</returns>
        [return: Nullable]
        public static string GetDisplayNameFromAttribute([Nullable] ICustomAttributeProvider provider)
        {
            return provider?.GetCustomAttribute<DisplayAttribute>()?.Name     // Try DataAnnotations first
                ?? provider?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;   // ComponentModel
            // we don't fall back on DescriptionAttribute since it is for longer descriptions.
        }

        /// <summary>
        /// Returns a description (for display to a user) for the attributed item
        /// if there is an attribute that provides one.
        /// </summary>
        /// <param name="provider">The item to get a description for.</param>
        /// <returns>The description name, or null if no supported attribute is present.</returns>
        [return: Nullable]
        public static string GetDescriptionFromAttribute([Nullable] ICustomAttributeProvider provider)
        {
            return provider?.GetCustomAttribute<DisplayAttribute>()?.Description        // Try DataAnnotations first
                ?? provider?.GetCustomAttribute<DescriptionAttribute>()?.Description;   // ComponentModel
        }
    }
}
