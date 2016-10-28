using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JohnLambe.Util.Text;

namespace JohnLambe.Util
{
    public static class StrUtils
    {

        #region Null/Blank

        /// <summary>
        /// Returns the first string parameter that is not null and not "".
        /// If all are null or "", it returns "".
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string FirstNonBlank(params string[] values)
        {
            foreach (string value in values)
            {
                if (value != null && !value.Equals(""))
                    return value;
            }
            return "";
        }
        /* Extension method version of above:
         * Rejected - clutters extension method list.
        public static string FirstNonBlank(this string s, params string[] values)
        {
            if (s != null && !s.Equals(""))
                return s;
            foreach (string value in values)
            {
                if (value != null && !value.Equals(""))
                    return value;
            }
            return "";
        }
        */

        /// <summary>
        /// If the string is null, blank is returned, otherwise the string is returned unmodified.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string NullToBlank(this string s)
        {
            if (s == null)
                return "";
            else
                return s;
        }

        /// <summary>
        /// If any of the parameters is null or there are no parameters, null is returned,
        /// otherwise the concatentation of the strings is returned.
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static string NullPropagate(params string[] parts)
        {
            int totalLength = 0;
            if (parts.Length == 0)
                return null;
            foreach (string s in parts)
            {
                if (s == null)
                    return null;
                totalLength += s.Length;
            }
            StringBuilder sb = new StringBuilder(totalLength);
            sb.AppendArray(parts);
            return sb.ToString();
        }

        #endregion

        #region Concatenation

        public static string ConcatWithSeparatorIncludeBlanks(string separator, params string[] parts)
        {
            return ConcatWithSeparatorIncludeBlanks(separator, parts);
        }

        public static string ConcatWithSeparatorIncludeBlanks(string separator, IEnumerable<object> parts)
        {
            StringBuilder sb = new StringBuilder(TotalLength(separator.Length, parts));

            foreach (var part in parts)
            {
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }
                sb.Append(part);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Concatenate all non-null, non-blank strings in the parameter list,
        /// with a separator between them.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static string ConcatWithSeparator(string separator, params string[] parts)
        {
            return ConcatWithSeparatorIncludeBlanks(separator, parts.Where(p => !string.IsNullOrEmpty(p)));
        }

        /// <summary>
        /// Concatenate the parameters after trimming leading and trailing space in each one,
        /// inserting the separator between non-blank parts.
        /// null parts are skipped.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="parts">Strings to be concatenated.</param>
        /// <returns></returns>
        public static string ConcatWithSeparatorTrim(string separator, params string[] parts)
        {
            StringBuilder sb = new StringBuilder(TotalLength(separator.Length, parts));

            foreach (var part in parts)
            {
                if (part != null)
                {
                    string currentPart = part.Trim();
                    if (currentPart.Length > 0)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(separator);
                        }
                        sb.Append(currentPart);
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Concatenates the result of a delegate executed on each item in an enumerable.
        /// 
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="del">Delegate to be executed on each item in `enumerable`.</param>
        /// <param name="separator">Separator added between non-null items.</param>
        /// <returns></returns>
        public static string ConcatForEach<TItem, TReturn>(IEnumerable<TItem> enumerable, Func<TItem, TReturn> del, string separator = null)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var x in enumerable)
            {
                TReturn value = del(x);
                if (separator != null && value != null)
                {
                    if (!first)
                        sb.Append(separator);
                    else
                        first = false;
                }
                sb.Append(value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Total length of a string consisting of all non-null items in <see cref="parts"/>
        /// with a separator of length <see cref="separatorLength"/>.
        /// </summary>
        /// <param name="separatorLength"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static int TotalLength(int separatorLength, IEnumerable<object> parts)
        {
            int totalLength = 0;
            if (parts != null)
            {
                foreach (var part in parts)
                {
                    if (part != null)
                        totalLength += part.ToString().Length + separatorLength;  // counts 1 separator too many
                }
            }
            return totalLength == 0 ? 0 : totalLength - separatorLength;
        }

        #endregion

        #region Prefix/Suffix

        /// <summary>
        /// If s starts with <see cref="prefix"/>, it is removed.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="prefix"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static string RemovePrefix(this string s, string prefix, StringComparison comparison = StringComparison.InvariantCulture)
        {
            if (s == null)
                return null;
            if (s.StartsWith(prefix, comparison))
                return s.Substring(prefix.Length);
            else
                return s;
        }

        /// <summary>
        /// If s ends with <see cref="suffix"/>, it is removed.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="suffix"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static string RemoveSuffix(this string s, string suffix, StringComparison comparison = StringComparison.InvariantCulture)
        {
            if (s == null)
                return null;
            if (s.EndsWith(suffix, comparison))
                return s.Substring(0, s.Length - suffix.Length);
            else
                return s;
        }

        #endregion

        #region SplitToVars

        //TODO: Replace with one method with a params parameter.
        // Option to put rest of string in last parameter.

        /// <summary>
        /// Split a string into parts, separated by a given separator.
        /// If the separator does not occur, the whole string is the first part.
        /// If the given string is null, all parts are null.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator">the separator</param>
        /// <param name="p1">the first part. null if `s` is null.</param>
        /// <param name="p2">the second part, or null if there is only one or no parts.</param>
        public static void SplitToVars(this string s, char separator, out string p1, out string p2)
        {
            if (s == null)
            {
                p1 = null;
                p2 = null;
            }
            else
            {
                string[] parts = s.Split(separator);
                p1 = parts[0];
                p2 = parts.Length > 1 ? parts[1] : null;
            }
        }

        /// <summary>
        /// Split a string into parts, separated by a given separator.
        /// If the separator does not occur, the whole string is the first part.
        /// If the given string is null, all parts are null.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator">the separator</param>
        /// <param name="p1">the first part. null if `s` is null.</param>
        /// <param name="p2">the second part, or null if there is only one part.</param>
        /// <param name="p3">the third part, or null if there is fewer than three parts.</param>
        public static void SplitToVars(this string s, char separator, out string p1, out string p2, out string p3)
        {
            if (s == null)
            {
                p1 = null;
                p2 = null;
                p3 = null;
            }
            else
            {
                string[] parts = s.Split(separator);
                p1 = parts[0];
                p2 = parts.Length > 1 ? parts[1] : null;
                p3 = parts.Length > 2 ? parts[2] : null;
            }
        }

        #endregion

        /// <summary>
        /// Split a string on a separator, returning the first part and setting the string the rest.
        /// Repeated calls can extract a series of parts, one at a time.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="value">On entry: The string to be split.
        /// On exit: The remaining part of the string (after the separator). If nothing remains (i.e. there was no separator), this is null on exit.</param>
        /// <returns>The part before the separator.</returns>
        public static string SplitFirst(char separator, ref string value)
        {
            string[] parts = value.Split(new char[] { separator }, 2);
            if (parts.Length > 1)
                value = parts[1];
            else
                value = null;
            return parts[0];
        }

        #region Quoting

        /*
        public static string UnDoubleQuote(this string s, char quote = '"')
        {
            if (s == null)
                return null;
            if (s.Length > 0 && s[0] == quote && s.CharFromEnd() == quote)
            {
                StringBuilder sb = new StringBuilder(s.Length);
                for (int index = 1; index < s.Length - 1; index++)  // loop, skipping the first and last characters, which we know are quotes
                {
                    if(s[index] != quote || s)
                    sb.Append(s[index]);
                }
            }
            else
            {
                return s;
            }
        }
         * */

        /// <summary>
        /// Enclose string in quotes and escape quotes within it by replacing with two quote characters.
        /// If <paramref name="s"/> is null, null is returned.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public static string DoubleQuote(this string s, char quote = '"')
        {
            if (s == null)
                return null;
            else
                return "" + quote + DoubleEscapeQuote(s) + quote;
        }

        /// <summary>
        /// Escape quotes by replacing with two quote characters.
        /// If <paramref name="s"/> is null, null is returned.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public static string DoubleEscapeQuote(this string s, char quote = '"')
        {
            if (s == null)
                return null;
            return s.Replace("" + quote, "" + quote + quote);
            /*  Alternative implementation:
                        if(!s.Contains(quote))
                            return s;
                        StringBuilder sb = new StringBuilder(s.Length * 2);
                        foreach(char c in s)
                        {
                            sb.Append(quote);
                            if (c == quote)         // if quote
                                sb.Append(quote);   // 
                        }
            */
        }

        #endregion


        /// <summary>
        /// Returns a substring consisting of the last <paramref name="length"/> characters of <paramref name="s"/>.
        /// If <paramref name="s"/> is null, null is returned.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length">Number of characters. Must be 0 or positive.
        /// If longer than the string, the whole string is returned.</param>
        /// <returns></returns>
        public static string EndSubstring(this string s, int length)
        {
            if (s == null)
                return null;
            return s.Substring(System.Math.Max(0, s.Length - length));
        }

        #region Get character

        /// <summary>
        /// Returns the character at index <see cref="index"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="index">0-based index of a character.</param>
        /// <param name="defaultValue">Value to return if the string is null or <see cref="index"/> is out of range.</param>
        /// <returns></returns>
        public static char CharAt(this string s, int index, char defaultValue = '\0')
        {
            if (s == null || index >= s.Length || index < 0)
                return defaultValue;
            return s[index];
        }

        /// <summary>
        /// Returns the character <see cref="index"/> characters from the end.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="index">Position from the end of the string. 0 returns the last character.</param>
        /// <param name="defaultValue">Value to return if the string is null or <see cref="index"/> is out of range.</param>
        /// <returns></returns>
        public static char CharFromEnd(this string s, int index = 0, char defaultValue = '\0')
        {
            if (s == null || index < 0)
                return defaultValue;
            index = s.Length - index - 1;
            if (index < 0)
                return defaultValue;
            return s[index];
        }

        #endregion

        /// <summary>
        /// Remove specified characters from a string.
        /// If <paramref name="s"/> is null, null is returned.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="remove">Characters to be removed. null is treated the same as "".</param>
        /// <param name="removeExcept">If true, all characters except those specified are removed. Otherwise, those specified are removed.</param>
        /// <returns>A copy of the string with characters removed.</returns>
        public static string RemoveCharacters(this string s, string remove, bool removeExcept = false)
        {
            if (s == null)
                return null;
            if (remove == null)
                remove = "";
            StringBuilder stringbuilder = new StringBuilder(s.Length);
            foreach (char currentCharacter in s)
            {
                if (!remove.Contains(currentCharacter) ^ removeExcept)   // if it contains it or doesn't, depending on removeExcept
                    stringbuilder.Append(currentCharacter);
            }
            return stringbuilder.ToString();
        }

        /// <summary>
        /// Repeat this string <paramref name="count"/> times.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count">The number of times to repeat.
        /// 0 is valid (return "").</param>
        /// <returns><paramref name="s"/> repeated <paramref name="count"/> times.</returns>
        /// <exception cref="ArgumentException">If count is negative.</exception>
        public static string Repeat(this string s, int count)
        {
            if (count < 0)
                throw new ArgumentException(nameof(count), "StrUtils.Repeat: 'count' cannot be negative (was " + count + ")");
            if (s == null)
                return null;
            StringBuilder stringbuilder = new StringBuilder(s.Length * count);
            for (int n = 0; n < count; n++)
                stringbuilder.Append(s);
            return stringbuilder.ToString();
        }

        #region Enclosed

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="prefix">Prefix, must be non-null.</param>
        /// <param name="suffix">Suffix, or null if the same as <paramref name="Prefix"/>.</param>
        /// <returns>true iff this string starts with <paramref name="prefix"/> and ends with <paramref name="suffix"/>, not overlapping.</returns>
        public static bool IsEnclosedIn(this string s, string prefix, string suffix = null)
        {
            if (suffix == null)
                suffix = prefix;
            return s != null                                      // returns false for null.
                && s.Length >= prefix.Length + suffix.Length      // prefix and suffix not overlapping
                && s.StartsWith(prefix) && s.EndsWith(suffix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="encloser"></param>
        /// <returns>true iff this string starts with <paramref name="encloser"/>, not overlapping.</returns>
        public static bool IsEnclosedIn(this string s, char encloser)
        {
            return IsEnclosedIn(s, encloser, encloser);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns>true iff this string starts with <paramref name="prefix"/> and ends with <paramref name="suffix"/>, not overlapping.</returns>
        public static bool IsEnclosedIn(this string s, char prefix, char suffix)
        {
            return s != null                                      // returns false for null.
                && s.Length >= 2                                  // prefix and suffix not overlapping
                && s[0] == prefix && s[s.Length - 1] == suffix;
        }

        /// <summary>
        /// Iff the string is enclosed within <paramref name="prefix"/> and <paramref name="suffix"/>,
        /// the text between these is returned, otherwise null is returned.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="prefix">Prefix, must be non-null.</param>
        /// <param name="suffix">Suffix, or null if the same as <paramref name="Prefix"/>.</param>
        /// <returns></returns>
        public static string ExtractEnclosed(this string s, string prefix, string suffix = null)
        {
            if (suffix == null)
                suffix = prefix;
            if (s.IsEnclosedIn(prefix,suffix))
                return s.Substring(prefix.Length, s.Length - suffix.Length - prefix.Length);
            else
                return null;
        }

        public static string ExtractEnclosed(this string s, char prefix, char suffix)
        {
            if (s.IsEnclosedIn(prefix, suffix))
                return s.Substring(1, s.Length - 2);
            else
                return null;
        }

        #endregion
    }
}
