using SimpleInjector.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using JohnLambe.Util.Reflection;
using DiExtension.Attributes;
using SimpleInjector;

namespace DiExtension.SimpleInject
{
    /// <summary>
    /// Chooses a constructor based on the <see cref="InjectAttribute"/> attribute.
    /// </summary>
    public class ConstructorResolutionBehavior : IConstructorResolutionBehavior
    {
        /// <summary>
        /// </summary>
        /// <param name="existing">The behaviour to be used when this one cannot resolve a constructor
        /// - usually the behavior in place before the new one is installed.</param>
        public ConstructorResolutionBehavior(IConstructorResolutionBehavior existing)
        {
            this.Existing = existing;
        }

        /// <summary>
        /// Register an instance of this class with a given SimpleInjector container.
        /// </summary>
        /// <param name="options">The <see cref="ContainerOptions"/> of the container to register with.</param>
        /// <returns>The new <see cref="ConstructorResolutionBehavior"/> that was registered.</returns>
        public static ConstructorResolutionBehavior RegisterWith(ContainerOptions options)
        {
            var constructorResolutionBehavior = new ConstructorResolutionBehavior(options.ConstructorResolutionBehavior);
            options.ConstructorResolutionBehavior = constructorResolutionBehavior;
            return constructorResolutionBehavior;
        }
        
        /// <summary>
        /// If there is only one construcotr, it is used.
        /// If there are multiple, but only one has the <see cref="InjectAttribute"/> attribute,
        /// it is used.
        /// Otherwise, resolution fails.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        public virtual ConstructorInfo GetConstructor(Type serviceType, Type implementationType)
        {
            var allCtors = implementationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);   // get all public contstructors
            if(allCtors.Count() == 1)                // if only one constructor
                return allCtors.First();

            var attributedCtors = allCtors.Where(c => c.IsDefined<InjectAttribute>());   // all public constructors with the attribute
            if (attributedCtors.Count() == 1)       // if only one constructor with Inject attribute
                return attributedCtors.First();     // use it

            // Otherwise, fall back on the default behaviour. (It will fail unless there is another extension that resolves it).
            return Existing.GetConstructor(serviceType, implementationType);

/*
            // failed, throw an exception:
            if(attributedCtors.Count() > 1)
                throw new SimpleInjector.ActivationException("Can't choose constructor to inject: Multiple have the [Inject] attribute (" + serviceType.FullName + "->" + implementationType.FullName + ")");
            else
                throw new SimpleInjector.ActivationException("Can't choose constructor to inject (" + serviceType.FullName + "->" + implementationType.FullName + ")");
*/
        }

        /// <summary>
        /// The behaviour to be used when this one cannot resolve a constructor.
        /// </summary>
        protected readonly IConstructorResolutionBehavior Existing;
    }
}
