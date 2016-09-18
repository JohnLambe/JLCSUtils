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

    }
}
