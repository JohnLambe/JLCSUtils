using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiExtension.Attributes;
using JohnLambe.Util.Collections;
using JohnLambe.Util.Types;
using System.Reflection;
using JohnLambe.Util.FilterDelegates;
using System.Diagnostics;

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
        /// <param name="diResolver">DI container to resolve some parameters for injection.</param>
        /// <param name="parameters">The parameters to be populated.</param>
        /// <param name="contextArgs">Optional list of arguments to be populated for the first parameters indicated for it. (See <paramref name="selector"/>).
        /// null is treated the same as an empty array.
        /// </param>
        /// <param name="selector">Delegate to choose between populating each parameter from DI (if it returns false) or <paramref name="contextArgs"/> (if it returns true).
        /// When using <paramref name="contextArgs"/>, the next unused item is taken from this (parameters are populated in ascending order of their index).
        /// If it returns null, the next unused parameter from <paramref name="contextArgs"/> is used if there are any remaining, otherwise DI is used.
        /// If null, it behaves as if it always returns null.</param>
        /// <param name="startIndex">The (0-based) index of the first parameter (in <paramref name="parameters"/>) to populate.</param>
        /// <param name="endIndex">The (0-based) index of the last parameter (in <paramref name="parameters"/>) to populate. -1 for the last parameter in <paramref name="parameters"/>.</param>
        /// <returns>
        /// An array the same length as <paramref name="parameters"/>, with the requested range of values populated.
        /// Elements outside the requested range (<paramref name="startIndex"/> to <paramref name="endIndex"/>) are null.
        /// </returns>
        public static object[] PopulateArgs(this IDiResolver diResolver,
            [NotNull] ParameterInfo[] parameters, [Nullable] object[] contextArgs = null, [Nullable] SourceSelectorDelegate selector = null,
            int startIndex = 0, int endIndex = -1)
        {
            if (contextArgs == null)
                contextArgs = EmptyCollection<object>.EmptyArray;

            object[] args = new object[parameters.Count()];       // populated arguments (to be returned)

            SourceSelectionDetails s = new SourceSelectionDetails();

            int parameterIndex = 0;                               // index of the parameter
            int contextArgsIndex = 0;                             // index of next unused element in contextArgs
            foreach (var parameter in parameters)
            {
                if (parameterIndex >= startIndex && (parameterIndex <= endIndex || endIndex == -1))
                {
                    bool useDefault = false;  // true if the parameters default value is to be used

                    s.Clear();
                    if (selector != null)
                        selector.Invoke(s, parameter);
                    else
                        s.Required = !parameter.HasDefaultValue;
                    if (s.FromParams == null)
                        s.FromParams = contextArgsIndex < contextArgs.Length;  // not specified as a context parameter, and all context parameters have been used

                    if (s.FromParams.Value)
                    {   // Context parameter:
                        if (contextArgsIndex >= contextArgs.Length)
                        {
                            if (s.Required)
                                throw new InjectionFailedException("Too many parameters for injection from context parameters", null, parameter.Name, null);
                            else
                                useDefault = true;
                        }
                        else
                        {
                            args[parameterIndex] = contextArgs[contextArgsIndex];
                            contextArgsIndex++;   // next parameter
                        }
                    }
                    else
                    {   // other parameters are injected from the DI container
                        try
                        {
                            args[parameterIndex] = diResolver.GetInstanceFor<object>(parameter);
                        }
                        catch(DependencyInjectionException /*ex*/)
                        {
                            if (s.Required)
                            {
                                //TODO: trap exception and throw more specific one
                                throw;
                            }
                            else
                            {
                                useDefault = true;
                            }
                        }
                    }

                    if (useDefault)
                        args[parameterIndex] = parameter.DefaultValue;
                }
                parameterIndex++;
            }

            return args;
        }

        /// <summary>
        /// Delegate to provide information on how a parameter should be injected.
        /// </summary>
        /// <param name="s">
        /// The delegate populates this with the result.
        /// On entry, this is passed in its initial state (as after a call to <see cref="SourceSelectionDetails.Clear"/>).
        /// </param>
        /// <param name="parameter"></param>
        //| `s` is passed this way rather than returned, for efficiency reasons.
        public delegate void SourceSelectorDelegate([NotNull] SourceSelectionDetails s, [NotNull] ParameterInfo parameter);

        /// <summary>
        /// Details of how an item is to be injected.
        /// Parameter to <see cref="SourceSelectorDelegate"/>.
        /// </summary>
        public sealed class SourceSelectionDetails
        {
            public SourceSelectionDetails()
            {
                Clear();
            }

            /// <summary>
            /// Reset all properties to their initial values.
            /// </summary>
            public void Clear()
            {
                FromParams = null;
                Required = true;
            }

            /// <summary>
            /// true iff the current parameter is to be populated from the context arguments.
            /// </summary>
            public bool? FromParams { get; set; }
            public bool Required { get; set; }
        }

        public static SourceSelectorDelegate AttributeSourceSelector =
            (s,parameter) =>
            {
                Debug.Assert(s.FromParams == null);
                var attribute = parameter.GetCustomAttribute<InjectAttribute>();
                if (attribute != null)
                {
                    s.FromParams = !attribute.Enabled;    // attributed as a context parameter
                    s.Required = attribute.Required;
                }
                // s.FromParams is still null if there was no InjectAttribute.
            };

        /// <summary>
        /// Registers the value of each readable non-primitive property, with a non-null value, of the given object as a singleton in the DI container.
        /// </summary>
        /// <param name="diContext"></param>
        /// <param name="source">The object whose properties to register.</param>
        public static void RegisterProperties([NotNull] this IDiInstanceRegistrar diContext, [Nullable] object source,
            [Nullable] FilterDelegate<PropertyInfo> propertyFilter = null)
        {
            if (source != null)
            {
                propertyFilter = propertyFilter ?? FilterDelegateConst<PropertyInfo>.True;
                foreach (var property in source.GetType().GetProperties()
                    .Where(p => p.CanRead && propertyFilter(p) && !p.PropertyType.IsPrimitive && p.PropertyType!=typeof(string)))
                {
                    var value = property.GetValue(source, null);
                    if (value != null)
                        diContext.RegisterInstance(property.PropertyType, value);
                }
            }
        }

        /// <summary>
        /// Registers the values of properties of a given object, including nested properties.
        /// </summary>
        /// <param name="diContext">The context to register with.</param>
        /// <param name="source">The object whose properties are to be to registered.</param>
        /// <param name="propertyFilter">Called on all properties. Returns true for the properties to be registered.</param>
        /// <param name="recurseFilter">Called on all properties. Returns true on those to be scanned for nested properties.</param>
        /// <param name="levels">Number of levels to recurse. 0 for top level only (no recursion).</param>
        public static void RegisterPropertiesNested([NotNull]this IDiInstanceRegistrar diContext,
            [Nullable] object source,
            [Nullable] FilterDelegate<PropertyInfo> propertyFilter = null,
            [Nullable] FilterDelegate<PropertyInfo> recurseFilter = null,
            int levels = 1)
        {
            if (source != null)
            {
                propertyFilter = propertyFilter ?? FilterDelegateConst<PropertyInfo>.True;
                foreach (var property in source.GetType().GetProperties()
                    .Where(p => p.CanRead && propertyFilter(p) && !p.PropertyType.IsPrimitive && p.PropertyType != typeof(string)))
                {
                    var value = property.GetValue(source, null);
                    if (value != null)
                    {
                        diContext.RegisterInstance(property.PropertyType, value);
                        if (levels > 0 && (recurseFilter?.Invoke(property) ?? true))
                        {
                            diContext.RegisterPropertiesNested(value,propertyFilter,recurseFilter,levels-1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Registers the values of properties of a given object,
        /// with names derived from the property names.
        /// </summary>
        /// <param name="diContext">The context to register with.</param>
        /// <param name="source">The object whose properties are to be to registered.</param>
        /// <param name="namePrefix"></param>
        /// <param name="propertyFilter">Called on all properties. Returns true for the properties to be registered.</param>
        public static void RegisterPropertiesNamed([NotNull]this IDiExtInstanceRegistrar diContext,
            [Nullable] object source,
            [Nullable] string namePrefix = null,
            [Nullable] FilterDelegate<PropertyInfo> propertyFilter = null)
        {
            if (source != null)
            {
                propertyFilter = propertyFilter ?? FilterDelegateConst<PropertyInfo>.True;
                foreach (var property in source.GetType().GetProperties()
                    .Where( p => p.CanRead && propertyFilter(p)))
                {
                    var value = property.GetValue(source, null);
                    if (value != null)
                        diContext.RegisterInstance(property.Name, (namePrefix ?? "") + value);
                }
            }
        }

    }
}
