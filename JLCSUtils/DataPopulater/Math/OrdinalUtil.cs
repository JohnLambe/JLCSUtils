// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

namespace JohnLambe.Util.Math
{
    /// <summary>
    /// Utilities relating to ordinal numbers.
    /// </summary>
    public class OrdinalUtil
    {
        /// <summary>
        /// Retuns the suffix of the ordinal number of n.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        static public string OrdinalSuffix(int n)
        {
            n = System.Math.Abs(n) % 100;
            if (n >= 10 && n <= 20)
            {
                return "th";
            }
            switch (n % 10)
            {
                case 1: return "st";
                case 2: return "nd";
                case 3: return "rd";
                default: return "th";
            }
        }

        /// <summary>
        /// Returns the ordinal number of n.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        static public string Ordinal(int n)
        {
            return n.ToString() + OrdinalSuffix(n);
        }
    }
}
