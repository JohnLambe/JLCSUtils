using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.FilterDelegates
{
    public delegate bool FilterDelegate<T>(T candidate);

    public static class FilterDelegateExt
    {
        public static FilterDelegate<T> And<T>(this FilterDelegate<T> a, FilterDelegate<T> b)
        {
            return candidate => (a(candidate) && b(candidate));
        }

        public static FilterDelegate<T> Or<T>(this FilterDelegate<T> a, FilterDelegate<T> b)
        {
            return candidate => (a(candidate) || b(candidate));
        }

        public static FilterDelegate<T> Not<T>(this FilterDelegate<T> a)
        {
            return candidate => (!a(candidate));
        }

        public static FilterDelegate<T> Or<T>(params FilterDelegate<T>[] p)
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

        /* Example usage:
          
        public static void x()
        {
            FilterDelegate<string> x = (p => true);
            FilterDelegate<string> y = x.And(p => p.Length > 10).Not();
            FilterDelegate<string> z = Or(x, y);

            BooleanExpression<int> bi1 = new BooleanExpression<int>(arg => arg > 5);
            BooleanExpression<string> bs2 = x;

            Func<int,bool> d1 = (arg => arg == 10);
            //BooleanExpression<int> bi3 = (FilterDelegate<int>)d1;   //invalid
            BooleanExpression<string> bs4 = x;
            BooleanExpression<string> bs5 = bs2 & !bs4 | BooleanExpression<string>.CreateConstant(true);

        }
        */
/*
        public static explicit operator FilterDelegate<T>(bool value)
        {
            return x => value;
        }
*/
    }
}
