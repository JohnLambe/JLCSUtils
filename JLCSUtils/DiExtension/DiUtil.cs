using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiExtension.Attributes;
using JohnLambe.Util.Collections;
using System.Reflection;

namespace DiExtension
{
    public static class DiUtil
    {
        public static T CallMethod<T>(IDiResolver diResolver, MethodInfo method, object target, object[] contextArgs = null, Func<ParameterInfo, bool?> selector = null)
        {
            var args = PopulateArgs(diResolver, method.GetParameters(), contextArgs, selector);
            return (T)method.Invoke(target, args);
        }

        /// <summary>
        /// Returns values to use for supplying to each of a list of parameters, using DI, and optionally another list of values (<paramref name="contextArgs"/>).
        /// (The latter can be used for injecting some parameters based on the context).
        /// </summary>
        /// <param name="diResolver"></param>
        /// <param name="parameters">The parameters to be populated.</param>
        /// <param name="contextArgs">Optional list of arguments to be populated for the first parameters indicated for it. (See <paramref name="selector"/>).</param>
        /// <param name="selector">Delegate to choose between populating each parameter from DI (if it returns false) or <paramref name="contextArgs"/> (if it returns true).
        /// When using <paramref name="contextArgs"/>, the next unused item is taken from this (parameters are populated in ascending order of their index).
        /// If it returns null, the next unused parameter from <paramref name="contextArgs"/> is used if there are any remaining, otherwise DI is used.
        /// If null, it behaves as if it always returns null.</param>
        /// <param name="startIndex">The (0-based) index of the first parameter (in <paramref name="parameters"/>) to populate.</param>
        /// <param name="endIndex">The (0-based) index of the last parameter (in <paramref name="parameters"/>) to populate. -1 for the last parameter in <paramref name="parameters"/>.</param>
        /// <returns>An array the same length as <paramref name="parameters"/>, with the requested range of values populated.
        /// Elements outside the requested range (<paramref name="startIndex"/> to <paramref name="endIndex"/>) are null.
        /// </returns>
        public static object[] PopulateArgs(IDiResolver diResolver, ParameterInfo[] parameters, object[] contextArgs = null, Func<ParameterInfo, bool?> selector = null, int startIndex = 0, int endIndex = -1)
        {
            if (contextArgs == null)
                contextArgs = EmptyCollection<object>.EmptyArray;

            object[] args = new object[parameters.Count()];       // populated arguments (to be returned)

            int parameterIndex = 0;                               // index of the constructor parameter
            int contextArgsIndex = 0;                             // index of next unused element in contextArgs
            bool? createParam = null;                             // true iff the current parameter is to be populated from the context arguments
            foreach (var parameter in parameters)
            {
                if (parameterIndex >= startIndex && (parameterIndex <= endIndex || endIndex == -1))
                {
                    if (selector != null)
                        createParam = selector.Invoke(parameter);
                    if (createParam == null)
                        createParam = contextArgsIndex < contextArgs.Length;  // not specified as a context parameter, and all context parameters have been used

                    if (createParam.Value)
                    {   // Context parameter:
                        if (contextArgsIndex >= contextArgs.Length)
                            throw new InjectionFailedException("To many parameters for injection from context", null, parameter.Name, null);
                        args[parameterIndex] = contextArgs[contextArgsIndex];
                        contextArgsIndex++;   // next parameter
                    }
                    else
                    {   // other parameters are injected from the DI container
                        args[parameterIndex] = diResolver.GetInstance<object>(parameter.ParameterType);
                    }
                }
                parameterIndex++;
            }

            return args;
        }

    }
}
