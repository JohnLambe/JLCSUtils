using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
