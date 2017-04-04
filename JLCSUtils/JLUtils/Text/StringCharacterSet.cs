using JohnLambe.Util.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Text
{
    /// <summary>
    /// Provides a Set interface to the characters of a string.
    /// </summary>
    public class StringCharacterSet : SetBase<char>, ISet<char>, IReadOnlySet<char>
    {
        public StringCharacterSet(string value = null)
        {
            StringValue = value;
        }

        /// <summary>
        /// Returns a string with one instance of each character in the given string.
        /// The order of the characters is undefined.
        /// If <paramref name="s"/> is null, null is returned.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveDuplicateChars(string s)
        {
            if (s == null)
                return null;

            StringBuilder builder = null;   // for the result. If this is null, no duplicates have been found, and the result so far is the same as the input.
            int index = 0;
            foreach (var character in s)
            {
                if(s.IndexOf(character,index+1) > 0)           // found duplicate
                {
                    if (builder == null)                      // ensure that we have the builder
                    {
                        builder = new StringBuilder(s.Length-1);   // there is at least one duplicate, so final length is shorter
                        builder.Append(s.Substring(0,index));      // copy everthing before this point
                    }
                    // don't add this character
                }
                else
                {
                    builder?.Append(character);
                }
                index++;
            }
            return builder?.ToString() ?? s;              // `builder` if not null, otherwise `s`
        }

        public virtual string StringValue
        {
            get { return _stringValue; }
            set { _stringValue = RemoveDuplicateChars(value); }
        }
        protected string _stringValue = "";

        public override int Count
            => StringValue.Length;

        public override bool Contains(char item)
            => StringValue.Contains(item);

        public override IEnumerator<char> GetEnumerator()
        {
            return StringValue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return StringValue.GetEnumerator();
        }
    }
}
