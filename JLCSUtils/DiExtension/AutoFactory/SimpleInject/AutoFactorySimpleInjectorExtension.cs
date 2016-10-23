using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiExtension.AutoFactory;

using JohnLambe.Util.Reflection;
using JohnLambe.Util.Text;
using DiExtension.Unity;

using SimpleInjector;
using System.Diagnostics;

namespace DiExtension.AutoFactory.SimpleInject
{
    /// <summary>
    /// Integrates AutoFactory with SimpleInjector.
    /// </summary>
    public class AutoFactorySimpleInjectorExtension
    {
        public AutoFactorySimpleInjectorExtension(Container container)
        {
            this.Container = container;
            Container.ResolveUnregisteredType += Container_ResolveUnregisteredType;
            _factoryFactory.OnResolveAutoInterface += ResolveAutoInterface;
        }

        protected virtual void Container_ResolveUnregisteredType(object sender, UnregisteredTypeEventArgs e)
        {
            Type serviceType = e.UnregisteredServiceType;
            e.Register( () => _factoryFactory.ResolveEx(serviceType, null) );
            // Passing no key/name to ResolveEx since SimpleInjector doesn't support keys.
            //TODO: Could supply ConfigInject key. Probably no need if ConfigInject is registered before this.
        }

        /// <summary>
        /// Handle callback from AutoFactoryFactory to resolve a type from the DI container.
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual object ResolveAutoInterface(Type interfaceType, string name)
        {
            Debug.Assert(name == null);  // because we didn't provide one in the call to AutoFactoryFactory.ResolveEx.
            //TODO: Could support ConfigInject key.
            try
            {
                return Container.GetInstance(interfaceType);
            }
            catch(Exception ex)
            {
                throw new Exception("Resolving " + interfaceType.FullName + " for AutoFactory failed", ex);
                    //TODO exception type
            }
        }

        protected AutoFactoryFactory<string> _factoryFactory = new AutoFactoryFactory<string>();
        protected readonly Container Container;
    }
}
