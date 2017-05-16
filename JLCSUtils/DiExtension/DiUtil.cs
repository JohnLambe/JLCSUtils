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
    /// <summary>
    /// Dependency injection utilities.
    /// </summary>
    public static class DiUtil
    {
        /// <summary>
        /// Call a method, populating some or all of its parameters by dependeny injection.
        /// </summary>
        /// <typeparam name="T">The return type. The return value of the method is cast to this.</typeparam>
        /// <param name="diResolver">The DI resolver to use for injecting parameters.</param>
        /// <param name="method">The method to invoke.</param>
        /// <param name="target">The instance to invoke the method on. Ignored if the method is static.</param>
        /// <param name="contextArgs">See <see cref="PopulateArgs(IDiResolver, ParameterInfo[], object[], Func{ParameterInfo, bool?}, int, int)"/>.</param>
        /// <param name="selector">See <see cref="PopulateArgs(IDiResolver, ParameterInfo[], object[], Func{ParameterInfo, bool?}, int, int)"/>.</param>
        /// <returns>The value returned by the invoked method.</returns>
        public static T CallMethod<T>(IDiResolver diResolver, MethodInfo method, object target, object[] contextArgs = null, SourceSelectorDelegate selector = null)
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
        public static object[] PopulateArgs(this IDiResolver diResolver, ParameterInfo[] parameters, object[] contextArgs = null, SourceSelectorDelegate selector = null, int startIndex = 0, int endIndex = -1)
        {
            if (contextArgs == null)
                contextArgs = EmptyCollection<object>.EmptyArray;

            object[] args = new object[parameters.Count()];       // populated arguments (to be returned)

            int parameterIndex = 0;                               // index of the parameter
            int contextArgsIndex = 0;                             // index of next unused element in contextArgs
            foreach (var parameter in parameters)
            {
                if (parameterIndex >= startIndex && (parameterIndex <= endIndex || endIndex == -1))
                {
                    bool? createParam = null;                             // true iff the current parameter is to be populated from the context arguments
                    if (selector != null)
                        createParam = selector.Invoke(parameter);
                    if (createParam == null)
                        createParam = contextArgsIndex < contextArgs.Length;  // not specified as a context parameter, and all context parameters have been used

                    if (createParam.Value)
                    {   // Context parameter:
                        if (contextArgsIndex >= contextArgs.Length)
                            throw new InjectionFailedException("Too many parameters for injection from context", null, parameter.Name, null);
                        args[parameterIndex] = contextArgs[contextArgsIndex];
                        contextArgsIndex++;   // next parameter
                    }
                    else
                    {   // other parameters are injected from the DI container
                        args[parameterIndex] = diResolver.GetInstance<object>(parameter.ParameterType);
                        //TODO: trap exception and throw more specific one
                    }
                }
                parameterIndex++;
            }

            return args;
        }

        public delegate bool? SourceSelectorDelegate(ParameterInfo parameter);

        public static SourceSelectorDelegate AttributeSourceSelector =
            parameter =>
            {
                bool? fromContextParam = null;
                var attribute = parameter.GetCustomAttribute<InjectAttribute>();
                if (attribute != null)
                {
                    fromContextParam = !attribute.Enabled;    // attributed as a context parameter
                }
                // still null if there was no InjectAttribute.
                return fromContextParam;
            };

    }
}
