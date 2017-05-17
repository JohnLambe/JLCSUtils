using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.FilterDelegates
{
    public delegate bool? NullableFilterDelegate<T>(T candidate);

    public static class NullableFilterDelegate
    {
        /*
        public static NullableFilterDelegate<T> And<T>(this NullableFilterDelegate<T> a, NullableFilterDelegate<T> b)
        {
            return candidate => (a(candidate) && b(candidate));
        }

        public static NullableFilterDelegate<T> Or<T>(this NullableFilterDelegate<T> a, NullableFilterDelegate<T> b)
        {
            return candidate => (a(candidate) || b(candidate));
        }

        public static NullableFilterDelegate<T> Not<T>(this NullableFilterDelegate<T> a)
        {
            return candidate => (!a(candidate));
        }

        public static NullableFilterDelegate<T> Or<T>(params NullableFilterDelegate<T>[] p)
        {
            return
                (x =>
                {
                    foreach (var d in p)
                    {
                        if (d(x))
                            return true;
                    }
                    return false;
                }
            );
        }
        */

        /* Example usage:
          
        public static void x()
        {
            NullableFilterDelegate<string> x = (p => true);
            NullableFilterDelegate<string> y = x.And(p => p.Length > 10).Not();
            NullableFilterDelegate<string> z = Or(x, y);

            BooleanExpression<int> bi1 = new BooleanExpression<int>(arg => arg > 5);
            BooleanExpression<string> bs2 = x;

            Func<int,bool> d1 = (arg => arg == 10);
            //BooleanExpression<int> bi3 = (NullableFilterDelegate<int>)d1;   //invalid
            BooleanExpression<string> bs4 = x;
            BooleanExpression<string> bs5 = bs2 & !bs4 | BooleanExpression<string>.CreateConstant(true);

        }
        */
        /*
                public static explicit operator NullableFilterDelegate<T>(bool value)
                {
                    return x => value;
                }
        */
    }

    public static class NullableFilterDelegateConst<T>
    {
        /// <summary>
        /// Delegate that always retuns true.
        /// </summary>
        public static NullableFilterDelegate<T> True => x => true;

        /// <summary>
        /// Delegate that always retuns false.
        /// </summary>
        public static NullableFilterDelegate<T> False => x => false;

        /// <summary>
        /// Delegate that always retuns null.
        /// </summary>
        public static NullableFilterDelegate<T> Null => x => null;
    }

}
