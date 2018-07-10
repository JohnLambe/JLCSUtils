using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.Reflection;
using DiExtension;
using System.Diagnostics;
using DiExtension.Attributes;
using System.Diagnostics.Contracts;
using JohnLambe.Util.Types;
using JohnLambe.Util;

/* TODO: Done.
 * 
 * Presenter Factory creation and registration:
 *   Each parameter of the Presenter class's constructor that does not have an [Inject] attribute becomes a parameter to the factory method.
 *   The others are injected from the DI container.
 *   The factory implements an interface with a generic type argument for each parameter to the 'Create' (factory) method.
 *   A mapping from this interface to an implementation is registered with the DI container.
 *   
 *   public interface IPresenterFactory<TPresenter, P1..Pn>
 *   {
 *     TPresenter Create(P1 p1 .. Pn pn);
 *   }
 * 
 *   
 *   The factory will be injected with a reference to the DI container (or a limited interface to it).
 *   A later version of the DI extension will disallow injection of certain things including the DI container itself into most classes
 *   (those allowed to receive it (typically framework classes or extensions of the DI system could be identified by their namespace, or an attribute).
 * 
 */

//TODO: Presenter Action and Model interface registration. (IPresenterAction<Model>)

//TODO: When using SimpleInjector, provide a way to supply a LifeStyle. Not here. Could be Proxy implementation of IDiTypeRegistrar.

namespace MvpFramework
{
    /// <summary>
    /// Registers types for MVP resolution.
    /// </summary>
    public class RegistrationHelper
    {
        /// <summary>
        /// </summary>
        /// <param name="resolver"><see cref="Resolver"/></param>
        /// <param name="diContext"><see cref="IDiTypeRegistrar"/> or <see cref="DiExtension.IDiContext"/>.</param>
        public RegistrationHelper([NotNull] MvpResolver resolver, [NotNull] IDiTypeRegistrar diContext)
        {
            this._resolver = resolver.ArgNotNull(nameof(resolver), "RegistrationHelper: resolver cannot be null");
            this._diContext = diContext.ArgNotNull(nameof(diContext), "RegistrationHelper: diContexxt cannot be null");
        }

        /// <summary>
        /// Scan a list of assemblies and register types/instances.
        /// </summary>
        /// <param name="assemblies">The list of assemblies to scan. If empty, the calling assembly is scanned.</param>
        public virtual void ScanAssemblies(params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = new Assembly[] { Assembly.GetCallingAssembly() };
            Scan(assemblies);
        }

        /// <summary>
        /// Scan a list of assemblies for MVP registration.
        /// </summary>
        /// <param name="assemblies">The assemblies to be scanned.</param>
        public virtual void Scan(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies.Distinct())
            {
                // Register views first, because PresenterFactory tries to resolve the view interface to the view:
                foreach (var view in
                    assembly.GetTypes().Where(t => t.IsDefined<ViewAttribute>())
                    )
                {
                    IEnumerable<Type> viewInterfaces = _resolver.ResolveInterfacesForViewType(view);
                    if (viewInterfaces == null)
                        throw new MvpResolutionException("No interface found for view: " + view.FullName);
                    foreach(var viewInterface in viewInterfaces)
                        RegisterType(viewInterface, view);    // register the View class to be resolved from its interface
                    //                    RegisterType(Resolver.ResolveInterfaceForViewType(view), () => ReflectionUtils.Create<IView>(view));
                }

                // Presenters:
                foreach ( var presenter in
                    assembly.GetTypes().Where( t => t.IsDefined<PresenterAttribute>() )
                    )
                {
                    // Get the type (usually an interface) returned by the presenter factory:
                    var presenterInterface = _resolver.ResolveInterfaceForPresenterType(presenter);
                    if (presenterInterface != null)       // if the interface can be resolved
                    {                                     // (nothing is registered otherwise).  //TODO?: Register the concrete type
                        RegisterType(presenterInterface, presenter);             // register the Presenter class to be resolved from its interface

                        var factoryInterfaceType = typeof(IPresenterFactory<>).MakeGenericType(presenterInterface);

                        //                    factoryInterfaceType = typeof(IPresenterFactory<>)...MakeGenericType(typeof(object),typeof(int));
                        /*TODO*/
                        // Get the Presenter constructor to be invoked by the Presenter Factory:
                        var constructor = GetConstructor(presenter);

                        // Get the type parameters of the generic factory interface:
                        IList<Type> argTypes = new List<Type>();
                        argTypes.Add(presenterInterface);                   // first argument is always the returned presenter type
                        foreach (var arg in constructor.GetParameters())
                        {
                            //                        if(!arg.IsDefined<InjectAttribute>())   // if not injected by DI
                            if (arg.IsDefined<MvpParamAttribute>())
                            {
                                argTypes.Add(arg.ParameterType);
                            }
                        }

                        var argTypesArray = argTypes.ToArray();
                        // Make the closed generic type of the factory interface:
                        factoryInterfaceType = GenericTypeUtil.ChangeGenericParameters(typeof(IPresenterFactory<>), argTypesArray);
                        // Register a mapping from this generic factory interface to the concrete generic factory class:
                        RegisterType(factoryInterfaceType, GenericTypeUtil.ChangeGenericParameters(typeof(PresenterFactory<>), argTypesArray));

                        /*
                        //TODO: Determine Model or Parameters:
                                            Console.WriteLine(presenter.BaseType);
                                            Type modelType = presenter.BaseType.GenericTypeArguments[1];

                                            RegisterType(factoryInterfaceType, typeof(PresenterFactory<,>).MakeGenericType(presenterInterface, typeof(object)));

                                            factoryInterfaceType = typeof(IPresenterFactory<,>).MakeGenericType(presenterInterface, modelType);
                                            RegisterType(factoryInterfaceType, typeof(PresenterFactory<,>).MakeGenericType(presenterInterface, modelType));
                        */
                    }
                    //TOOD?: else generate warning
                }
            }
        }

        /// <summary>
        /// Get the constructor of a Presenter class to be used by the PresenterFactory.
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        protected virtual ConstructorInfo GetConstructor(Type cls)
        {
            IEnumerable<ConstructorInfo> constructors = cls.GetConstructors();
            if (constructors.Count() == 1)              // if only one
                return constructors.First();            // use it
            // otherwise, filter to those with the attribute:
            constructors = constructors.Where(c => c.IsDefined<InjectAttribute>());
            // return the first one:
            return constructors.First();

            //TODO: return all; support interfaces for all of them.
            //TODO: Move to PresenterFactory.
        }

        /// <summary>
        /// Register a type mapping with the DI container.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        protected virtual void RegisterType(Type serviceType, Type implementationType)
        {
            Contract.Requires(serviceType != null, "INTERNAL ERROR: Trying to register null serviceType");
            if(serviceType != null && implementationType != null)
            {
                _diContext.RegisterType(serviceType, implementationType);
            }
        }

        protected readonly MvpResolver _resolver;
        protected readonly IDiTypeRegistrar _diContext;
    }
}
