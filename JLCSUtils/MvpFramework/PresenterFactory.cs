using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

using JohnLambe.Util.DependencyInjection;


namespace MvpFramework
{
    /// <summary>
    /// Interface for factory that creates a Presenter.
    /// Can be used for automatic factory creation on dependency injection.
    /// </summary>
    /// <typeparam name="TPresenter"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public interface IPresenterFactory<TPresenter, TModel>
        where TPresenter : IPresenter
    {
        /// <summary>
        /// Create the Presenter.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        TPresenter Create(TModel model);
    }

    public interface IPresenterFactory<TPresenter>
        where TPresenter : IPresenter
    {
        TPresenter Create();
    }

    /// <summary>
    /// Base class for factories for creating Presenters.
    /// </summary>
    /// <typeparam name="TPresenter"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public class PresenterFactory<TPresenter, TModel> : IPresenterFactory<TPresenter, TModel>,
        IPresenterFactory<TPresenter>
        where TPresenter : IPresenter
        // Change TModel to TParam (not necessarily the Model) ?
    {
        public PresenterFactory(MvpResolver resolver, /*UiNavigator navigator,*/ IDiResolver diResolver
            /*, Type targetClass*/
            )
        // To remove service locators:
        //   MvpResolver:
        //     Could provide presenter type and view factory (new class) as constructor parameters.
        //     (There is some reduction in flexibility, since that would mean that the presenter type could not vary depending
        //     on parameters to the 'Create' method).
        //     Then DI system would have to create these using MvpResolver (moving the service locator into the DI system).
        //     MvpResolver shouldn't be an integral part of the DI system (it's specific to MVP),
        //     so it should be an optional extension of it.
        //     That would probably be more complex than the current solution.
        //     *This* could be considered that extension of the DI system.
        //   IDiResolver:
        //     Needed for creating the Presenter, and this is the presenter factory.
        //     The alternative would be to pass factories of each parameter of the presenter's constructor (except those already being passed)
        //     to this constructor. (SimpleInjector could be extended to provide an array of Expression for the parameters).
        //     That would be more complex and less flexible (for example, the presenter constructor parameters could not potentially depend
        //     on parameters to the 'Create' method).
        //     (It may not be possible or desirable to create these in advance.)
        //     That still doesn't handle property injection of the created presenter. (We could provide a map from propery name to Expression, but that's similar to a service locator.)
        //     We could move the work of actually populating the parameters to the DI system:
        //       Instead of calling GetInstance and BuildUp, we would call a single method, providing the View, and the Model and/or other parameters.
        {
            //            this.Navigator = navigator;
            this.DiResolver = diResolver;
            this.Resolver = resolver;
            //this.TargetClass = targetClass;
        }

        protected void Init()
        {   // This is separate from the constructor since it involves resolving items which may come from a DI container,
            // and SimpleInjector cannot register items after any resolve (so this should be called only after everything is registered).
            if (TargetConstructor == null)
            {
                this.TargetClass = Resolver.ResolvePresenterType(typeof(TPresenter), typeof(TModel));
                TargetConstructor = TargetClass.GetConstructors().First();
                //TODO if multiple constructors, choose one.
                //   Evaluate which are compatible? Use Attribute.
            }
        }

        TPresenter IPresenterFactory<TPresenter, TModel>.Create(TModel model)
        {
            return CreatePresenter(model);
        }

        TPresenter IPresenterFactory<TPresenter>.Create()
        {
            return CreatePresenter(null);
        }

        protected virtual TPresenter CreatePresenter(object model)
        { 
            Init();

            var parameters = TargetConstructor.GetParameters();   // constructor parameters
            object[] args = new object[parameters.Count()];       // constructor arguments

            // Populate the constructor arguments:
            int parameterIndex = 0;
            foreach (var parameter in parameters)
            {
                //                if(parameter.ParameterType.IsAssignableFrom(typeof(TView)))
                if (parameterIndex == 0)
                {   // first parameter is always the View
                    //                    args[parameterIndex] = Navigator.ViewForPresenterType(typeof(TPresenter));
                    // OR Resolver.ViewForPresenterType
                    // ...  (TargetClass)
                    args[parameterIndex] = Resolver.ViewForPresenterType<IView>(typeof(TPresenter));
                }
                else if (parameterIndex == 1)
                {   // second parameter (if present) is the Model
                    args[parameterIndex] = model;
                }
                else
                {   // other parameters are injected from the DI container
                    args[parameterIndex] = DiResolver.GetInstance<object>(parameter.ParameterType);
                }
                parameterIndex++;
            }

            var presenter = (TPresenter)TargetConstructor.Invoke(args);    // invoke the constructor
            DiResolver.BuildUp(presenter);                                 // inject properties
            return presenter;
        }

        /// <summary>
        /// The type of the Presenter created by this.
        /// </summary>
        protected Type TargetClass { get; private set; }
        /// <summary>
        /// The constructor of `TargetClass` to be used.
        /// </summary>
        protected ConstructorInfo TargetConstructor { get; private set; }
//        protected readonly UiNavigator Navigator;
        /// <summary>
        /// Interface to the dependency injection container.
        /// </summary>
        protected readonly IDiResolver DiResolver;
        /// <summary>
        /// Resolver for resolving the View.
        /// </summary>
        protected readonly MvpResolver Resolver;
    }

}