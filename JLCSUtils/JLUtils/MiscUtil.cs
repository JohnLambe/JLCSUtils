// Author: John Bernard Lambe
//   See licence.
////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util
{
    public static class MiscUtil
    {

        #region obsolete

        // For earlier C# versions before the "??" operator:

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

        #endregion

        /// <summary>
        /// Execute a delegate, suppressing NullReferenceException,
        /// and returning <paramref name="defaultValue"/> if it is raised by the delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="del"></param>
        /// <param name="defaultValue">Value to return on NullReferenceException.</param>
        /// <returns>The value returned by the delegate, or null if it threw <see cref="NullReferenceException"/>.</returns>
        public static T IgnoreNull<T>(Func<T> del, T defaultValue = default(T))
        {
            try
            {
                return del();
            }
            catch (NullReferenceException)
            {
                return defaultValue;
            }
        }

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
            TryDispose(value);
            value = null;
        }

        /// <summary>
        /// Disposes <paramref name="value"/> if it implements <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TryDispose(object value)
        {
            var disposable = value as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// If the given <paramref name="value"/> is of the specified type (<typeparamref name="TReturn"/>),
        /// execute the delegate <paramref name="matchedDelegate"/> with <paramref name="value"/> as its parameter,
        /// otherwise execute <paramref name="notMatchedDelegate"/> (if it is not null).
        /// </summary>
        /// <typeparam name="TRequired"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="value"></param>
        /// <param name="matchedDelegate">Delegate to be executed if the value is not of the required type.</param>
        /// <param name="notMatchedDelegate">Optional delegate to be executed if the value is not of the required type.</param>
        /// <returns>
        /// The return value of the delegate that was executed.
        /// If the value is not of the required type, and <paramref name="notMatchedDelegate"/> is null,
        /// the default value for <typeparamref name="TReturn"/> is returned.
        /// </returns>
        public static TReturn Cast<TRequired,TReturn>(object value, Func<TRequired, TReturn> matchedDelegate, Func<TReturn> notMatchedDelegate = null)
        {
            if (value is TRequired)
                return matchedDelegate((TRequired)value);
            else
                return notMatchedDelegate != null ? notMatchedDelegate() : default(TReturn);
        }

        /// <summary>
        /// The same as <see cref="Cast{TRequired, TReturn}(object, Func{TRequired, TReturn}, Func{TReturn})"/>
        /// except that nothing is returned by the delegates.
        /// <para>
        /// If the given <paramref name="value"/> is of the specified type (<typeparamref name="TReturn"/>),
        /// execute the delegate <paramref name="matchedDelegate"/> with <paramref name="value"/> as its parameter,
        /// otherwise execute <paramref name="notMatchedDelegate"/> (if it is not null).
        /// </para>
        /// </summary>
        /// <typeparam name="TRequired"></typeparam>
        /// <param name="value"></param>
        /// <param name="matchedDelegate">Delegate to be executed if the value is not of the required type.</param>
        /// <param name="notMatchedDelegate">Optional delegate to be executed if the value is not of the required type.</param>
        /// <returns>true iff the given <paramref name="value"/> was of the specified type.</returns>
        public static bool CastNoReturn<TRequired>(object value, VoidDelegate<TRequired> matchedDelegate, VoidDelegate notMatchedDelegate = null)
        {
            if (value is TRequired)
            {
                matchedDelegate((TRequired)value);
                return true;
            }
            else
            {
                notMatchedDelegate?.Invoke();
                return false;
            }
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
