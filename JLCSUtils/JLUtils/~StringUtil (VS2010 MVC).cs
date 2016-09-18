using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnLambe.Util
{
    public static class StringUtil
    {
        #region Remove Prefix / Suffix

        /// <summary>
        /// If the string ends with `suffix`, it is removed.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="suffix"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static string RemovePrefix(this string s, string prefix, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (s.StartsWith(prefix,comparison))
                return s.Substring(prefix.Length);
            else
                return s;
        }

        /// <summary>
        /// If the string ends with `suffix`, it is removed.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string RemoveSuffix(this string s, string suffix, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (s.EndsWith(suffix, comparison))
                return s.Substring(0, s.Length - suffix.Length);
            else
                return s;
        }

        /// <summary>
        /// If the string ends with `suffix`, it is removed.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string ChangeSuffix(this string s, string suffix, string newValue, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (s.EndsWith(suffix, comparison))
                return s.Substring(0, s.Length - suffix.Length) + newValue;
            else
                return s;
        }

        #endregion


        #region SplitToVars

        #region 2 Params

        public static string SplitToVars(this string s, char separator, out string p0, out string p1)
        {
            return SplitToVars(s, new char[] { separator }, out p0, out p1);
        }

        public static string SplitToVars(this string s, char[] separator, out string p0, out string p1)
        {
            string[] p = s == null ? new string[0] : s.Split(separator,3);
            p0 = p.Length == 0 ? null : p[0];
            p1 = p.Length < 2 ? null : p[1];
            return p.Length < 3 ? null : p[2];
        }

        public static string SplitToVars(this string s, string[] separator, out string p0, out string p1)
        {
            string[] p = s == null ? new string[0] : s.Split(separator, 3, StringSplitOptions.None);
            p0 = p.Length == 0 ? null : p[0];
            p1 = p.Length < 2 ? null : p[1];
            return p.Length < 3 ? null : p[2];
        }
        #endregion

        #region 3 Params

        public static string SplitToVars(this string s, char separator, out string p0, out string p1, out string p2)
        {
            return SplitToVars(s, new char[] { separator }, out p0, out p1, out p2);
        }

        public static string SplitToVars(this string s, char[] separator, out string p0, out string p1, out string p2)
        {
            string[] p = s==null? new string[0] : s.Split(separator, 4);
            p0 = p.Length == 0 ? null : p[0];
            p1 = p.Length < 2  ? null : p[1];
            p2 = p.Length < 3 ? null : p[2];
            return p.Length < 4 ? null : p[3];
        }

        public static string SplitToVars(this string s, string[] separator, out string p0, out string p1, out string p2)
        {
            string[] p = s == null ? new string[0] : s.Split(separator, 4, StringSplitOptions.None);
            p0 = p.Length == 0 ? null : p[0];
            p1 = p.Length < 2 ? null : p[1];
            p2 = p.Length < 3 ? null : p[2];
            return p.Length < 4 ? null : p[3];
        }

        #endregion

        public static string SplitToVars(this string s, char[] separator, out string p0, out string p1, out string p2, out string p3)
        {
            string[] p = s == null ? new string[0] : s.Split(separator,5);
            p0 = p.Length == 0 ? null : p[0];
            p1 = p.Length < 2 ? null : p[1];
            p2 = p.Length < 3 ? null : p[2];
            p3 = p.Length < 4 ? null : p[3];
            return p.Length < 5 ? null : p[4];
        }

        #endregion

        public static string Concat(string separator, params string[] p)
        {
            StringBuilder result = new StringBuilder();
            foreach (var param in p)
            {
                bool first = true;
                if (!string.IsNullOrWhiteSpace(param))
                {
                    if (!first)
                    {
                        result.Append(separator);
                        first = false;
                    }
                    result.Append(param.Trim());
                }
            }
            return result.ToString();
        }

    }

    public class StringProcessor
    {
        #region Settings

        /// <summary>
        /// Allowed quote characters.
        /// Never null. Assigning to null makes it an empty array.
        /// </summary>
        public virtual char[] Quotes
        {
            get
            {
                return _quotes;
            }
            set
            {
                if (value == null)
                    _quotes = _empty;
                else
                    _quotes = value;
            }
        }
        protected char[] _quotes = _empty;
        protected static char[] _empty = new char[] { };

        /// <summary>
        /// Iff true, quotes are escaped by doubling.
        /// </summary>
        public virtual bool DoubleQuotes { get; set; }

        /// <summary>
        /// Any character immediately following an instance of this character in the string
        /// is treated as literal, and this escape character is removed when processing the string.
        /// (e.g. '\' for C/C++/C# syntax).
        /// </summary>
        public virtual char? EscapeCharacter { get; set; }

        public virtual bool DefaultAll { get; set; }

        #endregion

        public virtual string ExtractQuoted(ref string value)
        {
            if (value == null)
                return null;
            if (value.Equals("") || !IsQuote(value[0])) // doesn't begin with a quote
            {
                if (DefaultAll)
                {
                    // return whole string and make value "":
                    string result = value;
                    value = "";
                    return result;
                }
                else
                {
                    // value is unmodified
                    return "";
                }
            }

            char quote = value[0];  // the first character is a quote
            int charIndex = 0;
            while (charIndex < value.Length)
            {
                charIndex++;

                if (value[charIndex] == quote)
                {

                }
            }


            return null;  //XXX
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Input value, which may have escape characters and quotes.</param>
        /// <param name="charIndex">0-based offset within `value`. See GetNextCharacter.</param>
        /// <param name="inQuote">See GetNextCharacter.</param>
        /// <returns>literal text after processing.</returns>
        public virtual string ProcessToEnd(string value, int charIndex, char? inQuote)
        {
            var result = new StringBuilder(value.Length);
            char? nextCharacter = null;
            do
            {
                nextCharacter = GetNextCharacter(value, ref charIndex, ref inQuote);
                if(nextCharacter != null)
                    result.Append(nextCharacter);
            } while (nextCharacter != null);
            return result.ToString();
        }

        /// <summary>
        /// Return the next character in the string, after processing escaping and quotes.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="charIndex">0-based index of a position in `value` which must not be escaped
        /// (i.e. not immediately following an escape character).</param>
        /// <param name="inQuote">if not null, the given position (charIndex) is within quotes, and this is the quote character.</param>
        /// <returns>The next character, or null if there are no more characters.</returns>
        public virtual char? GetNextCharacter(string value, ref int charIndex, ref char? inQuote)
        {
            if (charIndex >= value.Length)   // past the end
            {
                return null;
            }

            char currentChar = value[charIndex];
            charIndex++;   // advance one character
            char? nextCharacter = charIndex >= value.Length ? (char?)null : value[charIndex];

            if (IsEscapeCharacter(currentChar))
            {
                charIndex++;                 // skip the character being read (we skipped the escape character already)
                return value[charIndex - 1];
            }
            else if (inQuote.HasValue)                 // within quotes
            {
                if (currentChar == inQuote)            // reached a quote matching the opening one
                {
                    if (DoubleQuotes && nextCharacter == inQuote)   // if double quotes are supported and there are two consecutive ones
                    {
                        charIndex++;           // skip the second quote
                        return currentChar;    // this is an escaped quote character
                    }
                    else
                    {
                        inQuote = null;         // no longer in quotes
                        return null;            // this is the end
                    }
                }
                // anything else is a literal character, within quotes. It will be returned at the end of this method.
            }

/*
            else if (IsQuote(currentChar))
            {
                if (DoubleQuotes)
                {
                    if (value[charIndex + 1] == value[charIndex])
                    {
                        charIndex += 2;  // skip both quote characters
                        return value[charIndex - 1];   // return the second character
                    }
                }
                // we've come to a quote character that is not escaped or doubled
                if (inQuote.HasValue)
                {
                    inQuote = false;   // closing quote
                    return null;        // end
                }
                else
                {
                    inQuote = true;
                }
            }
*/

            return currentChar;
        }

        /// <summary>
        /// true iff the given character can be used as a quote character.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public virtual bool IsQuote(char c)
        {
            return Quotes.Contains(c);
        }

        /// <summary>
        /// true iff the given character is an escape character (escapes the next character).
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public virtual bool IsEscapeCharacter(char c)
        {
            return EscapeCharacter.HasValue && EscapeCharacter.Value == c;
        }
    }

}
