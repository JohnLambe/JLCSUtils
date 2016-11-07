using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection
{
    public static class GenericTypeUtils
    {
        const string GenericIndicator = "`";

        /// <summary>
        /// Looks for a type with the same name (including namespace) as the given one, with a different number of generic type parameters
        /// (in the same assembly).
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="genericParameterCount">New number of generic type parameters.
        /// If 0, a non-generic type is returned.</param>
        /// <returns></returns>
        /// <exception cref="">If no such type exists.</exception>
        public static Type ChangeGenericParameterCount(Type baseType, int genericParameterCount)
        {
            string typeName = baseType.FullName.SplitBefore(GenericIndicator)
                + (genericParameterCount > 0 ? 
                    GenericIndicator + genericParameterCount : "");
            return baseType.Assembly.GetType(typeName)
                .NotNull("Generic type not found: " + typeName);
            //TODO: Exception type
        }
    }
}
