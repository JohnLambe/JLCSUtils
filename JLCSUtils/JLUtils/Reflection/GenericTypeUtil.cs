using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    public static class GenericTypeUtil
    {
        /// <summary>
        /// The separator between the type name and number of arguments in type names (as passed to Type.GetType()).
        /// </summary>
        public const string GenericIndicator = "`";

        /// <summary>
        /// Looks for an (open generic) type with the same name (including namespace) as the given one, with a different number of generic type parameters
        /// (in the same assembly).
        /// </summary>
        /// <param name="baseType">A type with same name as the required one (open generic, closed generic or non-generic).</param>
        /// <param name="genericParameterCount">New number of generic type parameters.
        /// If 0, a non-generic type is returned.</param>
        /// <returns>an open generic type (unless <paramref name="genericParameterCount"/> is 0, in which case it returns a non-generic type) with the requested number of type parameters.</returns>
        /// <exception cref="System.TypeLoadException">If no such type exists.</exception>
        public static Type ChangeGenericParameterCount(Type baseType, int genericParameterCount)
        {
            string typeName = baseType.FullName.SplitBefore(GenericIndicator)
                + (genericParameterCount > 0 ? 
                    GenericIndicator + genericParameterCount : "");
            return baseType.Assembly.GetType(typeName, true);
        }

        /// <summary>
        /// Return a (closed) generic type with the same name as the given one,
        /// but with the given generic type arguments.
        /// There can be a different number of type arguments given to what the given type has.
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="genericParameters">The new generic parameters. If an empty list is given, a non-generic type is returned.</param>
        /// <returns></returns>
        public static Type ChangeGenericParameters(Type baseType, params Type[] genericParameters)
        {
            var openGenericType = ChangeGenericParameterCount(baseType, genericParameters.Length);
            if (genericParameters.Length == 0)
                return openGenericType;            // in this case, it is non-generic
            else
                return openGenericType.MakeGenericType(genericParameters);
        }

        #region Nullable

        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <returns>true iff <paramref name="t"/> is a nullable (<see cref="Nullable{T}"/>) value type.</returns>
        public static bool IsNullableValueType(this Type t)
            => Nullable.GetUnderlyingType(t) != null;
            // same as: => t.IsGenericType && typeof(Nullable<>) == t.GetGenericTypeDefinition();

        /// <summary>
        /// Returns the <see cref="Nullable{T}"/> type for the given type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception>If <paramref name="t"/> is not a value type.</exception>
        public static Type MakeNullableValueType(this Type t)
        {
//            if (t.IsValueType)
            return typeof(Nullable<>).MakeGenericType(t);
//            else
//                throw new ArgumentException("Type " + t + " is not nullable");
        }

        /// <summary>
        /// If this is a nullable value type, the underlying type is returned, otherwise this type is returned.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Type GetNonNullableType(this Type t)
            => Nullable.GetUnderlyingType(t) ?? t;

        /// <summary>
        /// If the given cannot be assigned null (i.e. if it is neither a nullable value type, nor a reference type),
        /// this returns an equivalent type that can (a nullable value type),
        /// otherwise returns the given type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns>the corresponding nullable type.</returns>
        public static Type EnsureNullable(this Type t)
        {
            if(t.IsValueType)
            {
                if (t.IsNullableValueType())
                    return t;                        // already nullable
                else
                    return t.MakeNullableValueType();
            }
            else
            {   // not a value type, so it is already nullable
                return t;
            }
        }

        #endregion
    }
}
