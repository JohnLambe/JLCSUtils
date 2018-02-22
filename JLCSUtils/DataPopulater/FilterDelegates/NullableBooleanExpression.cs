using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.FilterDelegates
{
    /// <summary>
    /// Encapsulates a delegate that takes an argument of type <typeparamref name="T"/> and returns <see cref="Nullable"/>&lt;bool&gt;,
    /// and provides operators and methods to combine these to form other <see cref="NullableBooleanExpression{T}"/> instances.
    /// <para>Nulls are combined as in SQL: null acts as an identity with in any binary operation (doing any operation with null yields the other operand),
    /// and NOT(null) yields null.</para>
    /// </summary>
    /// <typeparam name="T">The type of parameter to the delegate.</typeparam>
    public class NullableBooleanExpression<T>
    {
        public NullableBooleanExpression(NullableFilterDelegate<T> del)
        {
            if (del == null)
                throw new ArgumentNullException(nameof(del), "NullableBooleanExpression: null delegate");
            _delegate = del;
        }

        /// <summary>
        /// Create a NullableBooleanExpression that always evaluates to the same value.
        /// </summary>
        /// <param name="constantValue"></param>
        /// <returns></returns>
        /// <remarks>This is not provided as a constructor, in order to make it more explicit that
        /// the parameter is not a delegate (in case someone mistakes a boolean expression
        /// argument for a delegate).</remarks>
        //| See Remarks.
        public static NullableBooleanExpression<T> CreateConstant(bool constantValue)
        {
            return new NullableBooleanExpression<T>(x => constantValue);
        }

        public static implicit operator NullableFilterDelegate<T>(NullableBooleanExpression<T> b)
        {
            return b._delegate;
        }

        public static implicit operator NullableBooleanExpression<T>(NullableFilterDelegate<T> d)
        {
            return new NullableBooleanExpression<T>(d);
        }

        /// <summary>
        /// Acts as a *logical* AND.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        //Note: && is not overloadable.
        public static NullableBooleanExpression<T> operator &(NullableBooleanExpression<T> a, NullableBooleanExpression<T> b)
        {
            return new NullableBooleanExpression<T>(x =>
            {
                bool? aValue = a.Invoke(x);
                if (aValue == null)
                    return b.Invoke(x);
                if (!aValue.Value)
                    return false;
                bool? bValue = b.Invoke(x);
                if (bValue == null)
                    return null;
                return aValue.Value && bValue.Value;
            });
        }

        /// <summary>
        /// Acts as a *logical* OR.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        //Note: || is not overloadable.
        public static NullableBooleanExpression<T> operator |(NullableBooleanExpression<T> a, NullableBooleanExpression<T> b)
        {
            return new NullableBooleanExpression<T>(x =>
            {
                bool? aValue = a.Invoke(x);   // evaluate the first operand
                if (aValue == null)             // if null,
                    return b.Invoke(x);       // the result is the second operand even if it is null
                if (aValue.Value)      
                    return true;                // lazy evaluation (result is true regardless of the value of b)
                bool? bValue = b.Invoke(x);   // a is not null or true, so we have to evaluate b
                if (bValue == null)             
                    return aValue;              // a is the only non-null operand
                return aValue.Value || bValue.Value;
            });
        }

        /// <summary>
        /// Returns a <see cref="NullableBooleanExpression{T}"/> this is true when this one is false,
        /// and vice versa.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static NullableBooleanExpression<T> operator !(NullableBooleanExpression<T> a)
        {
            return new NullableBooleanExpression<T>(x =>
            {
                var value = a.Invoke(x);
                if (value == null)
                    return null;
                else
                    return !value;
            });
        }

        //TODO: operator true and false
        /*
        public static implicit operator bool true(NullableBooleanExpression<T> a)
        {
            return true;
        }
        */

        public override bool Equals(object value)
        {
            if (value == (object)null)          // this can't be null
                return false;
            if (value is NullableBooleanExpression<T>)
                return this._delegate == ((NullableBooleanExpression<T>)value)._delegate;
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
        public static bool operator ==(NullableBooleanExpression<T> a, NullableBooleanExpression<T> b)
        {
            if ((object)a == null && (object)b == null)
                return true;
            else if ((object)a == null || (object)b == null)  // if either but not both are null
                return false;
            else
                return a.Equals(b); //a._delegate == b._delegate;
        }

        public static bool operator !=(NullableBooleanExpression<T> a, NullableBooleanExpression<T> b)
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
        public virtual bool? Invoke(T arg)
        {
            return _delegate(arg);
        }

        protected NullableFilterDelegate<T> _delegate;
    }


    public static class NullableBooleanExpressionExt
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
        public static bool? TryEvaluate<T>(this NullableBooleanExpression<T> expr, T arg, bool defaultValue = true)
        {
            if (expr != null)
                return expr.Invoke(arg);
            else
                return defaultValue;
        }
    }
}
