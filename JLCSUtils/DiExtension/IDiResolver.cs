using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension
{
    /// <summary>
    /// Abstraction of DI container,
    /// for use by consumers that only resolve items (get instances from the DI container).
    /// </summary>
    public interface IDiResolver
    {
        /// <summary>
        /// Get an instance of `serviceType` from the DI container.
        /// </summary>
        /// <typeparam name="T">Type to cast the result to (it may be any type that `serviceType` can be cast to).</typeparam>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        T GetInstance<T>(Type serviceType);

        T GetInstanceFor<T>(MemberInfo member);
        T GetInstanceFor<T>(ParameterInfo member);

        //| We could also have the method below, but the one above can be used in all cases,
        //| and is needed for cases where the required type is determined at run time:
        //|        T GetInstance<T>()
        //|            where T : class;

        /// <summary>
        /// Return the implementation type to be returned for a given service type.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        Type ResolveType(Type serviceType);

        /// <summary>
        /// Run property (and/or method) injection on a given instance.
        /// </summary>
        /// <returns><paramref name="instance"/></returns>
        /// <param name="instance"></param>
        T BuildUp<T>(T instance);

    }
    //TODO: Specify types of exceptions that may be raised.
    // And/or return null on failure.
    //TODO: These can currently throw SimpleInjector.ActivationException.
    // Wrap in DependencyInjectionException.
}
