using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.FilterDelegates
{
    /// <summary>
    /// Encapsulates a delegate that takes an argument of type <paramref name="T"/> and returns <see cref="bool"/>,
    /// and provides operators and methods to combine these to form other <see cref="BooleanExpression{T}"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of parameter to the delegate.</typeparam>
    public class BooleanExpression<T>
    {
        public BooleanExpression(FilterDelegate<T> del)
        {
            if (del == null)
                throw new ArgumentNullException(nameof(del),"BooleanExpression: null delegate");
            _delegate = del;
        }

        /// <summary>
        /// Create a BooleanExpression that always evaluates to the same value.
        /// </summary>
        /// <param name="constantValue"></param>
        /// <returns></returns>
        /// <remarks>This is not provided as a constructor, in order to make it more explicit that
        /// the parameter is not a delegate (in case someone mistakes a boolean expression
        /// argument for a delegate).</remarks>
        //| See Remarks.
        public static BooleanExpression<T> CreateConstant(bool constantValue)
        {
            return new BooleanExpression<T>( x => constantValue );
        }

        public static implicit operator FilterDelegate<T>(BooleanExpression<T> b)
        {
            return b._delegate;
        }

        public static implicit operator BooleanExpression<T>(FilterDelegate<T> d)
        {
            return new BooleanExpression<T>(d);
        }

        /// <summary>
        /// Acts as a *logical* AND.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        //Note: && is not overloadable.
        public static BooleanExpression<T> operator & (BooleanExpression<T> a, BooleanExpression<T> b)
        {
            return new BooleanExpression<T>( x => a._delegate(x) && b._delegate(x) );
        }

        /// <summary>
        /// Acts as a *logical* OR.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        //Note: || is not overloadable.
        public static BooleanExpression<T> operator | (BooleanExpression<T> a, BooleanExpression<T> b)
        {
            return new BooleanExpression<T>(x => a._delegate(x) || b._delegate(x));
        }

        /// <summary>
        /// Returns a <see cref="BooleanExpression"/> this is true when this one is false,
        /// and vice versa.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static BooleanExpression<T> operator !(BooleanExpression<T> a)
        {
            return new BooleanExpression<T>(x => !a._delegate(x));
        }

        //TODO: operator true and false
        /*
        public static implicit operator bool true(BooleanExpression<T> a)
        {
            return true;
        }
        */

        public override bool Equals(object value)
        {
            if (value == (object)null)          // this can't be null
                return false;
            if (value is BooleanExpression<T>)
                return this._delegate == ((BooleanExpression<T>)value)._delegate;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// True iff the two parameters have the same expression (delegate).
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(BooleanExpression<T> a, BooleanExpression<T> b)
        {
            if ((object)a == null && (object)b == null)
                return true;
            else if ((object)a == null || (object)b == null)  // if either but not both are null
                return false;
            else
                return a.Equals(b); //a._delegate == b._delegate;
        }

        public static bool operator !=(BooleanExpression<T> a, BooleanExpression<T> b)
        {
            if ((object)a == null && (object)b == null)
                return false;
            else if ((object)a == null || (object)b == null)   // if either but not both are null
                return true;
            else
                return !a.Equals(b); // !(a._delegate == b._delegate);
        }

        /// <summary>
        /// Apply the filter to the argument (invoke the delegate).
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>true iff the filter accepts the argument.</returns>
        public virtual bool Evaluate(T arg)
        {
            return _delegate(arg);
        }

        protected FilterDelegate<T> _delegate;
    }


    public static class BooleanExpressionExt
    {
        /// <summary>
        /// Evaluate if not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <param name="arg"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        //| defaultValue defaults to true since this is typically used for filtering items, and the absence of a filter means
        //| that all items should be included.
        public static bool TryEvaluate<T>(this BooleanExpression<T> expr, T arg, bool defaultValue = true)
        {
            if (expr != null)
                return expr.Evaluate(arg);
            else
                return defaultValue;
        }
    }
}
