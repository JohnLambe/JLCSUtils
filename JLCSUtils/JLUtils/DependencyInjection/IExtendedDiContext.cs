using JohnLambe.Util.DependencyInjection.ConfigInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.DependencyInjection
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

        void RegisterInstance(string name, object instance, bool buildUp = true);

        ConfigProviderChain ProviderChain { get; }
    }
}
