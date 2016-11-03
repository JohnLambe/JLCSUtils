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

/* TODO:
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

namespace MvpFramework
{
    public class RegistrationHelper
    {
        public RegistrationHelper(MvpResolver resolver, IDiTypeRegistrar diContext)
        {
            this._resolver = resolver;
            this._diContext = diContext;
        }

        public virtual void ScanAssemblies(params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = new Assembly[] { Assembly.GetCallingAssembly() };
            Scan(assemblies);
        }

        public virtual void Scan(IEnumerable<Assembly> assemblies)  //  IEnumerable<Assembly>
        {
            foreach (var assembly in assemblies)
            {
                // Register views first, because PresenterFactory tries to resolve the view interface to the view:
                foreach (var view in
                    assembly.GetExportedTypes().Where(t => t.IsDefined<ViewAttribute>())
                    )
                {
                    RegisterType(_resolver.ResolveInterfaceForViewType(view), view);
                    //                    RegisterType(Resolver.ResolveInterfaceForViewType(view), () => ReflectionUtils.Create<IView>(view));
                }

                foreach ( var presenter in
                    assembly.GetExportedTypes().Where( t => t.IsDefined<PresenterAttribute>() )
                    )
                {
                    var presenterInterface = _resolver.ResolveInterfaceForPresenterType(presenter);
                    RegisterType(presenterInterface, presenter);

                    var factoryInterfaceType = typeof(IPresenterFactory<>).MakeGenericType(presenterInterface);
/*TODO
                    var constructor = GetConstructor(presenter);
                    IList<Type> argTypes = new List<Type>();
                    foreach(var arg in constructor.GetParameters())
                    {
                        if(!arg.HasCustomAttribute<InjectAttribute>())   // if not injected by DI
                        {
                            argTypes.Add(arg.ParameterType);
                        }
                    }
*/

                    //TODO: Determine Model or Parameters:
                    Console.WriteLine(presenter.BaseType);
                    Type modelType = presenter.BaseType.GenericTypeArguments[1];

                    RegisterType(factoryInterfaceType, typeof(PresenterFactory<,>).MakeGenericType(presenterInterface, typeof(object)));

                    factoryInterfaceType = typeof(IPresenterFactory<,>).MakeGenericType(presenterInterface, modelType);
                    RegisterType(factoryInterfaceType, typeof(PresenterFactory<,>).MakeGenericType(presenterInterface, modelType));
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

        protected virtual void RegisterType(Type serviceType, Type implementationType)
        {
            Debug.Assert(serviceType != null);
            if(serviceType != null && implementationType != null)
            {
                _diContext.RegisterType(serviceType, implementationType);
            }
        }

        protected readonly MvpResolver _resolver;
        protected readonly IDiTypeRegistrar _diContext;
    }
}
