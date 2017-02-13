using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    public static class GenericTypeUtils
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
    }
}
