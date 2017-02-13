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
        public ConstructorResolutionBehavior(IConstructorResolutionBehavior existing)
        {
            this.Existing = existing;
        }


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
            var allCtors = implementationType.GetConstructors();
            if(allCtors.Count() == 1)                // only one constructor
                return allCtors.First();

            var attributedCtors = allCtors.Where(c => c.IsDefined<InjectAttribute>());
            if (attributedCtors.Count() == 1)       // only one constructor with Inject attribute
                return attributedCtors.First();

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

        protected readonly IConstructorResolutionBehavior Existing;
    }
}
