// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util
{
    public static class MiscUtil
    {
        // Rename Coalesce ?
        /// <summary>
        /// Return the first parameter that is not null or the default for the type.
        /// <para>In C# 4.0 and later, use the '??' operator.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T FirstNonNull<T>(params T[] values)
        {
            foreach (T value in values)
            {
                if (!value.Equals(default(T)))
                    return value;
            }
            return default(T);
        }


        #region obsolete

        // For earlier C# versions before the "??" operator:

        #region IfNotNull

        /// <summary>
        /// Use a delegate with this as a parameter if this is not null/default:
        /// <para>If `a` is null or the default for its type, the default value of the return type is returned.
        /// Otherwise, b is returned, using `a` as the parameter to b.
        /// </para>
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <typeparam name="P"></typeparam>
        /// <param name="a"></param>
        /// <param name="b">Delegate to execute if `a` is not null.</param>
        /// <param name="defaultValue">If present, this is returned instead of the default for the return type.</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">If `b` is null and `a` is not.</exception>
        public static R IfNotNull<R, P>(this P a, Func<P, R> b, R defaultValue = default(R))
            where P: class
        {
            if(a == default(P))
                return defaultValue;
            else
                return b(a);
        }

        public static R IfNotNull<R, R1, P>(this P a, Func<P,R1> b, Func<R1,R> c, R defaultValue = default(R))
            where P : class
            where R1: class
        {
            return a.IfNotNull(b).IfNotNull(c,defaultValue);
        }

        public static R IfNotNull<R, R1, R2, P>(this P a, Func<P, R1> b, Func<R1, R2> c, Func<R2, R> d, R defaultValue = default(R))
            where P : class
            where R1 : class
            where R2 : class
        {
            return a.IfNotNull(b).IfNotNull(c).IfNotNull(d, defaultValue);
        }

        public static R IfNotNull<R, R1, R2, R3, P>(this P a,
            Func<P, R1> b,
            Func<R1, R2> c,
            Func<R2, R3> d,
            Func<R3, R> e,
            R defaultValue = default(R))
            where P : class
            where R1 : class
            where R2 : class
            where R3 : class
        {
            return a.IfNotNull(b).IfNotNull(c).IfNotNull(d).IfNotNull(e, defaultValue);
        }

        #endregion

        /// <summary>
        /// Execute a delegate, suppressing NullReferenceException,
        /// and returning `defaultValue` if it is raised by the delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="del"></param>
        /// <param name="defaultValue">Value to return on NullReferenceException.</param>
        /// <returns></returns>
        public static T IgnoreNull<T>(Func<T> del, T defaultValue = default(T))
        {
            try
            {
                return del();
            }
            catch(NullReferenceException)
            {
                return defaultValue;
            }
        }

        #endregion

        #region Choose

        /// <summary>
        /// Choose one of the parameters based on the <paramref name="selector"/> parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector">The 0-based index in <paramref name="options"/> of the item to return.</param>
        /// <param name="options">0-based list of options.</param>
        /// <returns>The chosen option, or the default for the return type if <paramref name="selector"/> is out of range.</returns>
        public static T Choose<T>(int selector, params T[] options)
        {
            if(selector < 0 || selector > options.Length)
            {
                return default(T);
            }
            else
            {
                return options[selector];
            }
        }

        /// <summary>
        /// Choose one of the parameters based on the <paramref name="selector"/> parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="selector">The 1-based index in <paramref name="options"/> of the item to return.</param>
        /// <param name="options">List of options.</param>
        /// <returns>The chosen option, or the default for the return type if <paramref name="selector"/> is out of range.</returns>
        public static T Choose1Based<T>(int selector, params T[] options)
        {
            if(selector <= 0 || selector > options.Length)
            {
                return default(T);
            }
            else
            {
                return options[selector + 1];
            }
        }

        public static T ChooseWithDefault<T>(int selector, T defaultValue, params T[] options)
        {
            if(selector < 0 || selector > options.Length)
            {
                return defaultValue;
            }
            else
            {
                return options[selector];
            }
        }

        public static T ChooseValidated<T>(int selector, params T[] options)
        {
            if(selector <= 0 || selector > options.Length)
            {
                throw new IndexOutOfRangeException("Invalid selector for Choose: "
                    + selector.ToString() + 
                    + options.Length + " options");
            }
            else
            {
                return options[selector];
            }
        }

        #endregion

        /// <summary>
        /// Returns true if any of the given objects matches this one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        public static bool In<T>(this T obj, params T[] compareTo) where T: class
        {
            foreach(var x in compareTo)
            {
                if (obj == x)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Set this object reference to null and 
        /// dispose the object (call Dispose) if it implements <see cref="System.IDisposable" />.
        /// </summary>
        /// <param name="value">An object reference to be disposed and set to null.</param>
        public static void DisposeAndNull<T>(ref T value)
            where T: class
        {
            var disposable = value as IDisposable;
            if(disposable != null)
            {
                disposable.Dispose();
            }
            value = null;
        }
    }



    public class WriteOnceVariable<T> : VarBaseExplicitCast<T>
        where T: class
    {
        public override T Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if (base.Value == value)
                {
                }
                else if (base.Value != default(T))
                {
                    base.Value = value;
                }
                else
                {
                    throw new InvalidOperationException("WriteOnceVariable: Already assigned");
                }
            }
        }
    }


}
