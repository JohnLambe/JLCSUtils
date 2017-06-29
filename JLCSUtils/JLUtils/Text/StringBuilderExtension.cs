using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Text
{
    public static class StringBuilderExtension
    {
        /// <summary>
        /// Append each non-null string in the array to the StringBuilder.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static StringBuilder AppendArray(this StringBuilder sb, params string[] strings)
        {
            foreach (var s in strings)
            {
                if (s != null)
                    sb.Append(s);
            }
            return sb;
        }

        /// <summary>
        /// Replace a section of a string with another string (which may be a different length).
        /// <para>Similar to SQL 'stuff' function.</para>
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="start">The (0-based) index of the first character of the section to be replaced.
        /// Must not be negative.
        /// If past the end of the string, <paramref name="newValue"/> is appended.
        /// </param>
        /// <param name="length">The length of the section to be replaced. (If 0, the value is inserted.)</param>
        /// <param name="newValue">The value to replace with. null is treated as "".</param>
        /// <returns>the same instance that this is called on.</returns>
        /// <exception cref="IndexOutOfRangeException"/>
        /// <exception cref="ArgumentException"/>
        /// <seealso cref="StrUtil.ReplaceSubstring(string, int, int, string)"/>
        // SQL: Stuff
        public static StringBuilder ReplaceSubstring(this StringBuilder sb, int start, int length, string newValue)
        {
            if (start < 0)
                throw new IndexOutOfRangeException("Index out of range - " + nameof(start) + " = " + start);
            if (length < 0)
                throw new ArgumentException("Length cannot be negative");

            if (length > 0)
                sb.Remove(start, System.Math.Min(sb.Length - start, length));
            if(newValue != null)
                sb.Insert(start, newValue);
            return sb;
        }

    }
}
