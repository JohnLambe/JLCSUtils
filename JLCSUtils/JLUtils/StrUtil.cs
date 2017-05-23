using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JohnLambe.Util.Text;
using System.Diagnostics.Contracts;
using System.Windows;
using JohnLambe.Util.Types;
using JohnLambe.Util.Diagnostic;
using System.Runtime.CompilerServices;

namespace JohnLambe.Util
{
    /// <summary>
    /// Non-domain specific utilities for working with strings (including extension methods of System.String).
    /// </summary>
    public static class StrUtil
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

        /// <summary>
        /// If any of the parameters is null or "" or there are no parameters, "" is returned,
        /// otherwise the concatentation of the strings is returned.
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static string BlankPropagate(params string[] parts)
        {
            int totalLength = 0;
            if (parts.Length == 0)
                return null;
            foreach (string s in parts)
            {
                if (string.IsNullOrEmpty(s))
                    return "";
                totalLength += s.Length;
            }
            StringBuilder sb = new StringBuilder(totalLength);
            sb.AppendArray(parts);
            return sb.ToString();
        }

        /// <summary>
        /// Converts the object to a string, but returns "" is it is null.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToNonNullString(object o)
        {
            return (o?.ToString()).NullToBlank();
        }

        #endregion

        #region Concatenation

        /// <summary>
        /// Concatenates all strings in the enumerable, with a separator.
        /// The separator is included even between blank parts.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static string ConcatWithSeparatorIncludeBlanks(string separator, params string[] parts)
        {
            return ConcatWithSeparatorIncludeBlanks(separator, parts);
        }

        /// <summary>
        /// Concatenates all strings in the enumerable, with a separator.
        /// The separator is included even between blank parts.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
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

        public static string ConcatWithSeparatorsTrim(params object[] parts)
        {
            return ConcatWithSeparatorsTrimEnclosed(null, parts);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static string ConcatWithSeparatorsTrimEnclosed(object leader, params object[] parts)
        {
            string[] partsString = new string[parts.Length];
            int totalLength = 0;   // maximum total length
            int index = 0;

            // convert all to strings:
            foreach (var p in parts)
            {
                partsString[index] = ToNonNullString(p);
                if (index % 2 == 0)            // if not a separator or the trailer
                    partsString[index] = partsString[index].Trim();
                totalLength += partsString[index].Length;
                index++;
            }
            leader = ToNonNullString(leader);
            totalLength += ((string)leader).Length;

            StringBuilder builder = new StringBuilder(totalLength);
            for (index = 0; index < partsString.Length; index += 2)
            {
                if (!string.IsNullOrEmpty(partsString[index]))   // if this part is not blank
                {
                    if (builder.Length == 0)               // if this is the first non-blank one
                        builder.Append(leader);           // add the leader (prefix) part
                    builder.Append(partsString[index]);   // add this part
                    if (builder.Length > 0 && !string.IsNullOrEmpty(partsString.ElementAtOrDefault(index + 2)))  // if result is not blank so far, and next part is not null or blank
                    {
                        builder.Append(partsString[index + 1]);  // add the separator
                    }
                }
            }
            if ((partsString.Length % 2) == 0 && builder.Length > 0)  // if even number of parts (then the last one is a suffix for the whole string), and not blank so far
                builder.Append(partsString[partsString.Length - 1]);    // add the suffix
            return builder.ToString();
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
        /// <param name="del">Delegate to be executed on each item in <paramref name="enumerable"/>.</param>
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
        /// <param name="prefix">The prefix to be removed. If this is null, the original string is returned.</param>
        /// <param name="comparison">How the prefix is compared to the string.</param>
        /// <returns>The string after removing the prefix.</returns>
        public static string RemovePrefix(this string s, string prefix, StringComparison comparison = StringComparison.InvariantCulture)
        {
            if (s == null)
                return null;
            if (prefix != null && s.StartsWith(prefix, comparison))
                return s.Substring(prefix.Length);
            else
                return s;
        }

        /// <summary>
        /// If s ends with <see cref="suffix"/>, it is removed.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="suffix">The suffix to be removed. If this is null, the original string is returned.</param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static string RemoveSuffix(this string s, string suffix, StringComparison comparison = StringComparison.InvariantCulture)
        {
            if (s == null)
                return null;
            if (suffix != null && s.EndsWith(suffix, comparison))
                return s.Substring(0, s.Length - suffix.Length);
            else
                return s;
        }

        #endregion

        /// <summary>
        /// Equivalent to s.StartsWith("" + prefix).
        /// </summary>
        /// <param name="s"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">If <paramref name="s"/> is null.</exception>
        public static bool StartsWith(this string s, char prefix)
        {
            return s != "" && s[0] == prefix;
        }

        /// <summary>
        /// Equivalent to s.EndsWith("" + <paramref name="suffix"/>).
        /// </summary>
        /// <param name="s"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">If <paramref name="s"/> is null.</exception>
        public static bool EndsWith(this string s, char suffix)
        {
            return s != "" && s[s.Length - 1] == suffix;
        }

        #region SplitToVars

        /// <summary>
        /// Split a string into parts, separated by a given separator.
        /// If the separator does not occur, the whole string is the first part.
        /// If the given string is null, all parts are null.
        /// The last part is terminated by the separator if there is another separator.
        /// <para>e.g. "a|b|c|d".SplitToVars('|',out a, out b) will assign `a` to "a" and `b` to "b".</para>
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator">the separator</param>
        /// <param name="p1">the first part. null if <paramref name="s"/> is null.</param>
        /// <param name="p2">the second part, or null if there is only one or no parts.</param>
        public static void SplitToVars(this string s, char separator, out string p1, out string p2)
        {   // separate implementation, rather than calling the other overload, for efficiency.
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
        /// The last part is terminated by the separator if there is another separator.
        /// <para>e.g. "a|b|c|d".SplitToVars('|',out a, out b) will assign `a` to "a" and `b` to "b".</para>
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator">the separator</param>
        /// <param name="p1">the first part. null if <paramref name="s"/> is null.</param>
        /// <param name="p2">the second part, or null if there is only one or no parts.</param>
        public static void SplitToVars(this string s, char[] separator, out string p1, out string p2)
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
        /// <param name="p1">the first part. null if <paramref name="s"/> is null.</param>
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

        /// <summary>
        /// Split a string into parts, separated by a given separator.
        /// If the separator does not occur, the whole string is the first part.
        /// If the given string is null, all parts are null.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator">the separator</param>
        /// <param name="p1">the first part. null if <paramref name="s"/> is null.</param>
        /// <param name="p2">the second part, or null if there is only one part.</param>
        /// <param name="p3">the third part, or null if there is fewer than three parts.</param>
        /// <param name="p4">the fourth part, or null if there is fewer than four parts.</param>
        public static void SplitToVars(this string s, char separator, out string p1, out string p2, out string p3, out string p4)
        {
            if (s == null)
            {
                p1 = null;
                p2 = null;
                p3 = null;
                p4 = null;
            }
            else
            {
                string[] parts = s.Split(separator);
                p1 = parts[0];
                p2 = parts.Length > 1 ? parts[1] : null;
                p3 = parts.Length > 2 ? parts[2] : null;
                p4 = parts.Length > 3 ? parts[3] : null;
            }
        }


        /// <summary>
        /// Split a string into parts, separated by a given separator.
        /// If the separator does not occur, the whole string is the first part.
        /// If the given string is null, all parts are null.
        /// <para>The last part is everything after the first occurrence of the separator, even if there are more occurrences of separators characters.
        /// Otherwise, the result is the same as <see cref="SplitToVars(string, char, out string, out string)"/>.
        /// </para>
        /// <para>e.g. "a|b|c|d".SplitToVars('|',out a, out b) will assign `a` to "a" and `b` to "b|c|d".</para>
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator">the separator</param>
        /// <param name="p1">the first part. null if <paramref name="s"/> is null.</param>
        /// <param name="p2">the second part, or null if there is only one or no parts.</param>
        public static void SplitWholeToVars(this string s, char[] separator, out string p1, out string p2)
        {
            if (s == null)
            {
                p1 = null;
                p2 = null;
            }
            else
            {
                string[] parts = s.Split(separator, 2);
                p1 = parts[0];
                p2 = parts.Length > 1 ? parts[1] : null;
            }
        }

        public static void SplitWholeToVars(this string s, char separator, out string p1, out string p2)
        {
            SplitWholeToVars(s, new char[] { separator }, out p1, out p2);
        }

        /// <summary>
        /// Split a string into parts, separated by a given separator.
        /// If the separator does not occur, the whole string is the first part.
        /// If the given string is null, all parts are null.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator">the separator</param>
        /// <param name="p1">the first part. null if <paramref name="s"/> is null.</param>
        /// <param name="p2">the second part, or null if there is only one part.</param>
        /// <param name="p3">the third part, or null if there is fewer than three parts.</param>
        public static void SplitWholeToVars(this string s, char[] separator, out string p1, out string p2, out string p3)
        {
            if (s == null)
            {
                p1 = null;
                p2 = null;
                p3 = null;
            }
            else
            {
                string[] parts = s.Split(separator, 3);
                p1 = parts[0];
                p2 = parts.Length > 1 ? parts[1] : null;
                p3 = parts.Length > 2 ? parts[2] : null;
            }
        }

        #endregion

        #region Other splitting

        /// <summary>
        /// Split a string on a separator, returning the first part and setting the string the rest.
        /// Repeated calls can extract a series of parts, one at a time.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="value">On entry: The string to be split.
        /// On exit: The remaining part of the string (after the separator). If nothing remains (i.e. there was no separator), this is null on exit.</param>
        /// <returns>The part before the separator. null if <paramref name="value"/> is null.</returns>
        public static string SplitFirst(char separator, ref string value)
        {
            if (value == null)
                return null;
            string[] parts = value.Split(new char[] { separator }, 2);     // split into two parts on the given separator
            if (parts.Length > 1)
                value = parts[1];      // everything after first part
            else                       // nothing remaining
                value = null;
            return parts[0];           // return first part
        }

        /// <summary>
        /// Splits the string into two parts, at a specified character position.
        /// The specified character is not included in either part.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="index"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public static void SplitOn(this string s, int index, out string first, out string second)
        {
            first = s.Substring(0, index);
            second = s.Substring(index + 1);
        }

        /// <summary>
        /// Splits the string into two parts, at a specified character position.
        /// The specified character is included in the second part.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="index"></param>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public static void SplitAtInclusive(this string s, int index, out string first, out string second)
        {
            first = s.Substring(0, index);
            second = s.Substring(index);
        }

        /// <summary>
        /// Return the part of the string before the separator,
        /// or the whole string if the separator is not present.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator">the separator. Undefined for null.</param>
        /// <returns></returns>
        public static string SplitBefore(this string s, string separator)
        {
            separator.ArgNotNull(nameof(separator));
            if (s == null)
                return null;
            int splitPoint = s.IndexOf(separator);
            if (splitPoint == -1)
                return s;
            else
                return s.Substring(0, splitPoint);
        }
        //TODO: Define behavior for null `separator`

        /// <summary>
        /// Return the part of the string after the separator.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator">the separator. Undefined for null.</param>
        /// <returns>The part of the string after the separator,
        /// or null if the separator is not present.</returns>
        public static string SplitAfter(this string s, string separator)
        {
            separator.ArgNotNull(nameof(separator));
            if (s == null)
                return null;
            int splitPoint = s.IndexOf(separator);
            if (splitPoint == -1)
                return null;
            else
                return s.Substring(splitPoint + separator.Length);
        }

        #endregion

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
        [return: Nullable]
        public static string EndSubstring([NotNull]this string s, int length)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char CharAt(this string s, int index, char defaultValue = '\0')
        {
            return CharAtNullable(s, index, defaultValue).Value;
            // this won't be null because we gave a non-null defaultValue.
        }

        /// <summary>
        /// Returns the character at index <see cref="index"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="index">0-based index of a character.</param>
        /// <param name="defaultValue">Value to return if the string is null or <see cref="index"/> is out of range.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char? CharAtNullable(this string s, int index, char? defaultValue = null)
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
        /// Remove all occurrences of a specified character from the string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="remove">The character to remove.</param>
        /// <returns>The string with <paramref name="remove"/> removed.</returns>
        [return: Nullable]
        public static string RemoveCharacter([Nullable]this string s, char remove)
        {
            if (s == null)
                return null;
            StringBuilder stringbuilder = new StringBuilder(s.Length);
            foreach (char currentCharacter in s)
            {
                if (currentCharacter != remove)   // if it contains it
                    stringbuilder.Append(currentCharacter);
            }
            return stringbuilder.ToString();
        }

        /// <summary>
        /// Remove specified characters from a string.
        /// If <paramref name="s"/> is null, null is returned.
        /// </summary>
        /// <param name="s">The original string.</param>
        /// <param name="remove">Characters to be removed. null is treated the same as "".</param>
        /// <param name="removeExcept">If true, all characters except those specified are removed. Otherwise, those specified are removed.</param>
        /// <returns>A copy of the string with characters removed.</returns>
        [return: Nullable]
        public static string RemoveCharacters([Nullable]this string s, string remove, bool removeExcept = false)
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

        enum CharacterSetCompareOption
        {
            ContainsOnly = 1,
            ContainsAny,
        }

        /// <summary>
        /// True if this string contains only characters in <paramref name="characters"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static bool ContainsOnlyCharacters(this string s, ISet<char> characters)
        {
            if (s == null)
                return true;      // it can't contain any character not in the set
            if (characters == null)
                return string.IsNullOrEmpty(s);          // if no characters in the set, then if there is any character in `s`, it fails.
            foreach (char currentCharacter in s)
            {
                if (!characters.Contains(currentCharacter))  // found a character in the string not in the set
                    return false;
            }
            return true;
        }

        /*
        /// <summary>
        /// True if this string does not contain any characters in the set.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static bool ContainsOnlyNotCharacters(this string s, ISet<char> characters)
        {
            if (s == null)
                return false;      // it can't contain any of them
            foreach (char currentCharacter in s)
            {
                if (!characters.Contains(currentCharacter))
                    return true;
            }
            return false;
        }
        */

        /// <summary>
        /// True if this string contains any character in the set.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static bool ContainsAnyCharacters([Nullable]this string s, ISet<char> characters)
        {
            if (s == null || characters == null)
                return false;      // it can't contain any of them
            foreach (char currentCharacter in s)
            {
                if (characters.Contains(currentCharacter))
                    return true;
            }
            return false;
        }

        /* Use !ContainsAnyCharacters:
        /// <summary>
        /// True if the string contains none of the given characters.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static bool ContainsNoCharacters(this string s, ISet<char> characters)
        {
            if (s == null)
                return true;      // it can't contain any of them
            foreach (char currentCharacter in s)
            {
                if (characters.Contains(currentCharacter))
                    return false;
            }
            return true;
        }
        */

        /// <summary>
        /// Repeat this string <paramref name="count"/> times.
        /// </summary>
        /// <param name="s">The string to repeat. If null, null is returned.</param>
        /// <param name="count">The number of times to repeat.
        /// 0 is valid (returns "").</param>
        /// <returns><paramref name="s"/> repeated <paramref name="count"/> times.</returns>
        /// <exception cref="ArgumentException">If count is negative.</exception>
        // SQL: Replicate
        [return: Nullable]
        public static string Repeat([Nullable]this string s, int count)
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
            if (s.IsEnclosedIn(prefix, suffix))
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

        /// <summary>
        /// Return the substring between the first occurrence of any character in <param name="startDelimiters"/>
        /// and the next occurence after that of any character in <param name="endDelimiters"/>.
        /// The delimiter characters are not included.
        /// If either the start or end delimiter (when not null) is not found, null is returned.
        /// </summary>
        /// <param name="s">The string to find a substring in. If null, null is returned.</param>
        /// <param name="startDelimiters">Delimiter characters for the start of the substring, or null for the start of the string.</param>
        /// <param name="endDelimiters">Delimiter characters for the end of the substring, or null for the end of the string.</param>
        /// <param name="delimitersOptional">Iff true, if the starting delimiter is not found, the start of the string is the start of the substring,
        /// and if the ending delimiter is not found, the end of the string is the end of the substring.</param>
        /// <returns>the substring, or null if not found.</returns>
        [return: Nullable]
        public static string ExtractDelimited([Nullable]this string s, char[] startDelimiters, char[] endDelimiters = null, bool delimitersOptional = false)
        {
            if (s == null)
                return null;

            int start = startDelimiters == null ? 0 : s.IndexOfAny(startDelimiters);
            if (start == -1)
            {
                if (delimitersOptional)
                    start = 0;
                else
                    return null;
            }

            int end = endDelimiters == null ? s.Length : s.IndexOfAny(endDelimiters, start + 1);
            if (end == -1)
            { 
                if (delimitersOptional)
                    end = s.Length;
                else
                    return null;
            }

            return s.Substring(start, end - start);

            /*
            StringBuilder sb = new StringBuilder(s.Length);
            foreach(var c in s)
            {
                if (startDelimiters?.Contains(c) ?? true)   // delimiter found
                    return sb.ToString();

                if (endDelimiters.Contains(c))   // delimiter found
                    return sb.ToString();
                else
                    sb.Append(c);
            }
            */
        }

        /// <summary>
        /// Replace a section of a string with another string (which may be a different length).
        /// <para>Similar to SQL 'stuff' function.</para>
        /// </summary>
        /// <param name="s">the original strng.</param>
        /// <param name="start">The (0-based) index of the first character of the section to be replaced.
        /// Must not be negative. If past the end of the string </param>
        /// <param name="length">The length of the section to be replaced.</param>
        /// <param name="newValue">The value to replace with. null is treated as "".</param>
        /// <returns>the string after doing the replace.</returns>
//        /// <exception cref="ArgumentException"/>
        // SQL: Stuff
        public static string ReplaceSubstring(this string s, int start, int length, string newValue)
        {
            if(!(start >= 0 && length >= 0))
                throw new ArgumentException("StrUtils.ReplaceSubstring: Invalid (negative) " + nameof(start) + " or " + nameof(length));
//            Contract.Requires<ArgumentException>(start >= 0 && length >= 0);
            return s.Substring(0, start) + newValue.NullToBlank() + s.Substring(start + length);
        }

        /// <summary>
        /// Replace a section of a string between the given delimieters with a given string.
        /// The delimiters themselves are not replaced.
        /// If either of the delimiters are not found, the original string is returned.
        /// </summary>
        /// <param name="s">the original string.</param>
        /// <param name="start">the starting delimiter (the string to be replaced starts immediately after the first occurrence of this).
        /// null for the start of the string.</param>
        /// <param name="end">the ending delimiter. The first occurrence of this after the end of <paramref name="start"/> marks the end of the substring to be replaced.
        /// null for the end of the string.</param>
        /// <param name="newValue">The value to replace with.</param>
        /// <returns>The modified string after replacing the substring.</returns>
        public static string ReplaceSubstringBetween(this string s, string start, string end, string newValue)
        {
            int startIndex = start == null ? 0 : s.IndexOf(start);
            int startLength = start == null ? 0 : start.Length;
            int endIndex = end == null ? s.Length : s.IndexOf(end, startIndex + startLength);

            if (startIndex < 0 || endIndex < 0)
                return s;
            else
                return ReplaceSubstring(s, startIndex + (start?.Length ?? 0), endIndex - startIndex - startLength, newValue);
        }

        /// <summary>
        /// Replace a section of a string with another string of the same length.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="start"></param>
        /// <param name="newValue"></param>
        /// <param name="allowExtend">Iff true and <paramref name="newValue"/> is longer than the rest of the string, after <paramref name="start"/>,
        /// the string is extended to fit it;
        /// otherwise, newValue is truncated to fit in the length of the original string.</param>
        /// <returns></returns>
        public static string OverwriteSubstring(this string s, int start, string newValue, bool allowExtend = true)
        {
            return s.SafeSubstring(0, start)
                + (allowExtend ?
                    newValue + s.SafeSubstring(start + newValue.Length)
                    : newValue.SafeSubstring(0, s.Length - start) + s.SafeSubstring(start + newValue.Length, s.Length - start - newValue.Length)
                    );
        }

        #region Text Formatting
        // These treat null as "".

        /// <summary>
        /// Truncates the string if it is longer than the specified length,
        /// otherwise returns it unmodified.
        /// </summary>
        /// <param name="s">The string to truncate. If null, "" is returned.</param>
        /// <param name="length"></param>
        /// <returns></returns>
        [return: NotNull]
        public static string Truncate([Nullable]string s, int length)
        {
            if (s == null)
                return "";
            if (length >= s.Length)    // no truncation required
                return s;
            return s.Substring(0, length);
        }

        /// <summary>
        /// Pad a string to the specified width, unless it is longer, in which case the original string is returned.
        /// </summary>
        /// <param name="s">The string to pad. null is treated the same as "".</param>
        /// <param name="width">The required width. If negative, "" is returned.</param>
        /// <param name="alignment">Which way to align the text.
        /// <para>Note: When <see cref="TextAlignment.Justify"/>, SPACEs are replaced by the padding character.
        /// If there are no SPACEs, it is just aligned left.
        /// </para>
        /// </param>
        /// <param name="paddingChar">The character to pad width.</param>
        /// <returns></returns>
        public static string Pad([Nullable]this string s, int width, TextAlignment alignment = TextAlignment.Left, char paddingChar = ' ')
        {
            if (width <= 0)
                return "";
            s = s ?? "";
            if (s.Length >= width)
                return s;
            switch (alignment)
            {
                case TextAlignment.Left:
                    return s.PadRight(width, paddingChar);
                case TextAlignment.Right:
                    return s.PadLeft(width, paddingChar);
                case TextAlignment.Center:
                    return (CharacterUtil.Repeat(paddingChar, (width - s.Length) / 2) + s)  // add half the required space (rounded down) to the left
                        .PadRight(width, paddingChar);                              // add the rest to the right
                case TextAlignment.Justify:
                    {
                        // Determines the number of padding characters required, and replaces existing SPACEs,
                        // so that the total width is the required width.
                        // Padding of each space is even except for rounding.
                        // Currently, multiple consecutive SPACEs are NOT treated as a single gap (so they get a multiple of the normal padding amount between words).
                        // This may change in a later version.

                        s = s.Trim();
                        int gaps = s.CountCharacter(' ');
                        int spaceToAdd = width - s.Length + gaps;   // total number of required SPACEs, including this already in the string

                        StringBuilder sb = new StringBuilder(width);
                        int remainingPadding = spaceToAdd;
                        int remainingGaps = gaps;
                        if (gaps == 0)   // if no gaps, it can't be justified
                        {
                            return Pad(s, width, TextAlignment.Left, paddingChar);
                        }
                        else
                        {
                            foreach (char c in s)
                            {
                                if (c == ' ')
                                {
                                    int thisSpace = remainingPadding / remainingGaps;
                                    // at the last gap, remainingGaps==1, so all remaining padding to be added is added here.
                                    remainingPadding -= thisSpace;
                                    remainingGaps--;
                                    sb.Append(paddingChar.Repeat(thisSpace));
                                }
                                else
                                {
                                    sb.Append(c);
                                }
                            }
                        }
                        s = sb.ToString();
                        Diagnostics.Assert(s.Length == width, "StrUtil.Pad Justify output the wrong width");
                        return s;
                    }
                default:
                    Diagnostics.Fail("Unrecognised alignment option: " + alignment);
                    return s;
            }
        }

        /// <summary>
        /// Makes the string exactly the specified width
        /// (Pads the string to the specified width, and truncates it if is already longer than the specified width).
        /// </summary>
        /// <param name="s">The string to pad. null is treated the same as "".</param>
        /// <param name="width">The required width. If negative, "" is returned.</param>
        /// <param name="alignment">How the string is aligned, as with <see cref="Pad(string, int, TextAlignment, char)"/>.</param>
        /// <param name="paddingChar">The character to pad width.</param>
        /// <returns></returns>
        [return: NotNull]
        public static string PadFixedWidth([Nullable]this string s, int width, TextAlignment alignment = TextAlignment.Left, char paddingChar = ' ')
        {
            if (width <= 0)
                return "";
            s = s ?? "";
            if (s.Length > width)
                return Truncate(s, width);
            else
                return Pad(s, width, alignment, paddingChar);
        }

        #endregion

        /// <summary>
        /// Counts the number of occurrences of the given character in the string.
        /// </summary>
        /// <param name="s">The string to count characters in. if null, 0 is returned.</param>
        /// <param name="charToCount"></param>
        /// <returns></returns>
        public static int CountCharacter([Nullable]this string s, char charToCount)
        {
            return s?.Where(c => c == charToCount).Count() ?? 0;
            //| Would it be more efficient to write a loop?
        }

        /// <summary>
        /// True iff the given position in the string is the start of a word.
        /// À character is considered to be at the start of a word if it is at the start of the string, or is immediately preceded by whitespace
        /// or optionally, punctuation.
        /// </summary>
        /// <param name="s">The string. If null, false is returned.</param>
        /// <param name="position">The 0-based index of the character to test. If out of range, false is returned.</param>
        /// <param name="recognisePunctuation">Iff true, a character is considered to be at the start of a word if it immediately preceded by punctuation.</param>
        /// <returns>true if the given character is at the start of a word.</returns>
        public static bool IsStartOfWord([Nullable]string s, int position, bool recognisePunctuation = true)
        {
            if (s == null || position < 0 || position > s.Length)   // null or out of range
                return false;
            if (Char.IsWhiteSpace(s[position]) || (recognisePunctuation && Char.IsPunctuation(s[position])))
                return false;
            if (position == 0 || Char.IsWhiteSpace(s[position - 1]))
                return true;
            if (recognisePunctuation && Char.IsPunctuation(s[position - 1]))
                return true;
            return false;
        }

        /// <summary>
        /// True iff the string is all capitals, i.e. if it contains no lowercase letters.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsAllUpperCase(this string s)
            => !s.Any(c => Char.IsLower(c));

        /// <summary>
        /// True iff the string is all lower case, i.e. if it contains no capital letters.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsAllLowerCase(this string s)
            => !s.Any(c => Char.IsUpper(c));

        /// <summary>
        /// </summary>
        /// <param name="s"></param>
        /// <returns>True iff the string contains any control character.</returns>
        public static bool ContainsControlCharacter(this string s)
            => s != null && s.Any(c => Char.IsControl(c));

        #region SafeSubstring

        [return: Nullable]
        public static string SafeSubstring([Nullable]this string s, int start, int length)
        {
            if (s == null)
                return null;
            if (start >= s.Length || length <= 0)       // start is beyond the end or length is zero or negative
                return "";
            if (start + length >= s.Length)             // length is beyond the end
                return s.Substring(start);
            return s.Substring(start, length);
            //TOOD: start < 0 ?
        }

        [return: Nullable]
        public static string SafeSubstring([Nullable]this string s, int start)
        {
            if (s == null)
                return null;
            if (start >= s.Length)
                return "";
            return s.Substring(start);
        }

        #endregion
    }
}
