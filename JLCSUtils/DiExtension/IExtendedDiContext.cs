using DiExtension.ConfigInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension
{
    /// <summary>
    /// Interface to DI container independent of the underlying DI framework.
    /// </summary>
    public interface IExtendedDiContext : IDiContext, IConfigProvider
    {
        //TODO: better documentation

        /// <summary>
        /// Specify the concrete type to be returned (<paramref name="implementationType"/>) when
        /// a certain type (<paramref name="serviceType"/>; usually an interface) is requested.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType">The type to be returned. Must be assignable to <paramref name="serviceType"/>.</param>
        void RegisterType(Type serviceType, Type implementationType);

        void RegisterType(Type serviceType, string name);

        /// <summary>
        /// Register a named instance with the DI container.
        /// </summary>
        /// <param name="name">The name of this instance.</param>
        /// <param name="instance">The instance to be registered.</param>
        /// <param name="buildUp">Iff true, dependency injection is run on the <paramref name="instance"/> (for property injection, etc.).</param>
        void RegisterInstance(string name, object instance, bool buildUp = true);

        ConfigProviderChain ProviderChain { get; }
    }
}
