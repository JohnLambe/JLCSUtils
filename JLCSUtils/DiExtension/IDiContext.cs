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
        void RegisterInstance(object instance);
        void RegisterInstance(Type serviceType, object instance);
    }

    public interface IDiExtInstanceRegistrar : IDiInstanceRegistrar
    {
        void RegisterInstance(string name, object instance, bool buildUp = false);
    }
}
