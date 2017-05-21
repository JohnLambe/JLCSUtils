using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension
{
    public interface IDiContext 
    {
        T BuildUp<T>(T target);
        //TODO
    }

    /// <summary>
    /// Provides the ability to register types with the DI container.
    /// </summary>
    public interface IDiTypeRegistrar
    {
        void RegisterType(Type serviceType, Type implementationType);
    }

    public interface IDiInstanceRegistrar
    {
//        void RegisterInstance<TService>(object instance);
        void RegisterInstance(Type serviceType, object instance);
    }

    public static class DiInstantaceRegistrarExtension
    {
        /// <summary>
        /// Register an instance by its compile-time type.
        /// </summary>
        /// <typeparam name="TService">The type to be resolved to <paramref name="instance"/>.</typeparam>
        /// <param name="registrar"></param>
        /// <param name="instance">The instance to be registered.</param>
        public static void RegisterInstance<TService>(this IDiInstanceRegistrar registrar, object instance)
        {
            registrar.RegisterInstance(typeof(TService), instance);
        }
    }


    public interface IDiExtInstanceRegistrar : IDiInstanceRegistrar
    {
        void RegisterInstance(string name, object instance, bool buildUp = false);
    }
}
