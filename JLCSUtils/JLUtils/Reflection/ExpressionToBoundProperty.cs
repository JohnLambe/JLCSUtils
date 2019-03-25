using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Linq;

namespace JohnLambe.Util.Reflection
{
    /// <summary>
    /// Converts Expressions to bound properties, and does other parsing or conversion of them.
    /// </summary>
    public static class ExpressionToBoundProperty
    {
        /// <summary>
        /// Create a BoundProperty from a (root) target and an Expression to get a (possibly indirect) property value from it.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expr">Expression to return the property value (which may be of an indirect property), given the root target object.</param>
        /// <param name="target">The root object on which </param>
        /// <returns>An object representing the property and the binding to the target object.</returns>
        public static BoundProperty<TTarget, TProperty> FromExpression<TTarget, TProperty>(LambdaExpression expr, TTarget target)
        {
            var e = expr.Body;
            while (e is UnaryExpression && (e.NodeType == ExpressionType.Convert || e.NodeType == ExpressionType.ConvertChecked))   // remove any cast/conversion
            {
                e = ((UnaryExpression)e).Operand;
            }

            var memberExpression = e as MemberExpression;
            if (memberExpression != null)
            {
                var targetExpression = memberExpression.Expression.Reduce();
                var targetDelegate = SubExpressionUtil.CompileSubExpression(expr, targetExpression);
                var property = memberExpression.Member as PropertyInfo;

                return new DelegateBoundProperty<TTarget, TProperty>(() => (TTarget)targetDelegate.DynamicInvoke(target), property);
            }
            else
            {
                throw new ArgumentException(nameof(BoundProperty<TTarget, TProperty>) + ": Unsupported Linq expression: Must be a property.");
            }
        }

        /// <summary>
        /// Given an expression that returns a property (possibly indirectly on an object), return the property expression
        /// as a string.
        /// Casts and unary operations on property values are ignored.
        /// e.g. an expression of `x => x.b.c` would yield "b.c", and <paramref name="property"/> would be set to `c`.
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="property">The innermost property.</param>
        /// <returns></returns>
        public static string GetPropertyNameExpression(Expression expr, out PropertyInfo property)
        {
            if (expr is LambdaExpression)
                expr = ((LambdaExpression)expr).Body;
            while (expr is UnaryExpression && (expr.NodeType == ExpressionType.Convert || expr.NodeType == ExpressionType.ConvertChecked))   // remove any cast/conversion
            {
                expr = ((UnaryExpression)expr).Operand;
            }

            var memberExpression = expr as MemberExpression;
            if (memberExpression != null)
            {
                var targetExpression = memberExpression.Expression.Reduce();
                property = memberExpression.Member as PropertyInfo;

                return StrUtil.ConcatWithSeparator(".", GetPropertyNameExpression(targetExpression), property.Name);
            }
            else
            {
                property = null;
                return null;
                //                throw new ArgumentException("Unsupported Linq expression: Must be a property.");
            }
        }

        public static string GetPropertyNameExpression(Expression expr)
        {
            PropertyInfo property;
            return GetPropertyNameExpression(expr, out property);
        }
    }
}