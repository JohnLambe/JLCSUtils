using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

using DiExtension;
using JohnLambe.Util;
using DiExtension.Attributes;

namespace MvpFramework
{
    public interface IPresenterFactory<TPresenter>
        where TPresenter : IPresenter
    {
        TPresenter Create();
    }

    /// <summary>
    /// Interface for factory that creates a Presenter.
    /// Can be used for automatic factory creation on dependency injection.
    /// </summary>
    /// <typeparam name="TPresenter">Type of the presenter created by the factory.</typeparam>
    /// <typeparam name="TParam">Type of the parameter to the Create method.</typeparam>
    public interface IPresenterFactory<TPresenter, TParam>
        where TPresenter : IPresenter
    {
        /// <summary>
        /// Create the Presenter.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        TPresenter Create(TParam param);
    }

    /// <summary>
    /// Interface for factory that creates a Presenter.
    /// Can be used for automatic factory creation on dependency injection.
    /// </summary>
    /// <typeparam name="TPresenter">Type of the presenter created by the factory.</typeparam>
    /// <typeparam name="TParam1"></typeparam>
    /// <typeparam name="TParam2"></typeparam>
    public interface IPresenterFactory<TPresenter, TParam1, TParam2>
        where TPresenter : IPresenter
    {
        TPresenter Create(TParam1 param1, TParam2 param2);
    }

    /// <summary>
    /// Interface for factory that creates a Presenter.
    /// Can be used for automatic factory creation on dependency injection.
    /// </summary>
    /// <typeparam name="TPresenter">Type of the presenter created by the factory.</typeparam>
    /// <typeparam name="TParam1"></typeparam>
    /// <typeparam name="TParam2"></typeparam>
    /// <typeparam name="TParam3"></typeparam>
    public interface IPresenterFactory<TPresenter, TParam1, TParam2, TParam3>
        where TPresenter : IPresenter
    {
        TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3);
    }

    /// <summary>
    /// Interface for factory that creates a Presenter.
    /// Can be used for automatic factory creation on dependency injection.
    /// </summary>
    /// <typeparam name="TPresenter">Type of the presenter created by the factory.</typeparam>
    /// <typeparam name="TParam1"></typeparam>
    /// <typeparam name="TParam2"></typeparam>
    /// <typeparam name="TParam3"></typeparam>
    /// <typeparam name="TParam4"></typeparam>
    public interface IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4>
        where TPresenter : IPresenter
    {
        TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
    }

    /// <summary>
    /// Interface for factory that creates a Presenter.
    /// Can be used for automatic factory creation on dependency injection.
    /// </summary>
    /// <typeparam name="TPresenter">Type of the presenter created by the factory.</typeparam>
    /// <typeparam name="TParam1"></typeparam>
    /// <typeparam name="TParam2"></typeparam>
    /// <typeparam name="TParam3"></typeparam>
    /// <typeparam name="TParam4"></typeparam>
    public interface IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5>
        where TPresenter : IPresenter
    {
        TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5);
    }


    /// <summary>
    /// Generic factory for creating Presenters.
    /// Could be subclassed for custom presenter factory logic.
    /// </summary>
    /// <typeparam name="TPresenter"></typeparam>
    /// <typeparam name="TParam1"></typeparam>
    public class PresenterFactory<TPresenter> :
        IPresenterFactory<TPresenter>
        where TPresenter : IPresenter
    {
        public PresenterFactory(MvpResolver resolver, IDiResolver diResolver,
            IResolverExtension uiManager = null
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
            this.UiManager = uiManager ?? new NullUiManager();
            //this.TargetClass = targetClass;
        }

        protected virtual void Init()
        {   // This is separate from the constructor since it involves resolving items which may come from a DI container,
            // and SimpleInjector cannot register items after any resolve (so this should be called only after everything is registered).
            if (TargetConstructor == null)
            {
                try
                {
                    this.TargetClass = Resolver.ResolvePresenterType(typeof(TPresenter));
                    //                this.TargetClass = Resolver.ResolvePresenterType(typeof(TPresenter), typeof(TParam1));
                    TargetConstructor = TargetClass.GetConstructors().First();
                    //TODO: if multiple constructors, choose one.
                    //   Evaluate which are compatible? Use Attribute.
                }
                catch (Exception ex)
                {
                    throw new DependencyInjectionException("Failed to resolve Presenter type or constructor: " + ex.Message, ex);
                }
            }
        }

        TPresenter IPresenterFactory<TPresenter>.Create()
        {
            return CreatePresenter();
        }

        protected virtual TPresenter CreatePresenter(params object[] param)
        {
            Init();

            var existingPresenter = UiManager.BeforeCreatePresenter<TPresenter>(param);
            if (existingPresenter != null)
                return existingPresenter;

            var parameters = TargetConstructor.GetParameters();   // constructor parameters
            object[] args = new object[parameters.Count()];       // constructor arguments
            IView view = null;

            // Populate the constructor arguments:
            int parameterIndex = 0;
            int createMethodParameterIndex = 0;
            bool? createParam = null;
            foreach (var parameter in parameters)
            {
                //                if(parameter.ParameterType.IsAssignableFrom(typeof(TView)))
                if (parameterIndex == 0)
                {   // first parameter is always the View
                    view = Resolver.GetViewForPresenterType<IView>(typeof(TPresenter));
                    //| Could provide parameters for context-based injection of View.
                    UiManager.AfterCreateView(ref view);
                    args[parameterIndex] = view;
                }
                else
                {
                    var attribute = parameter.GetCustomAttribute<InjectAttribute>();
                    if (attribute != null)
                    {
                        createParam = attribute is MvpInjectAttribute;
                    }
                    else
                    {
                        createParam = parameterIndex < param.Length + 1;
                    }

                    if (createParam.Value)
                    {   // Create method parameters (possibly including the Model)
                        args[parameterIndex] = param[createMethodParameterIndex];
                        createMethodParameterIndex++;   // next parameter
                    }
                    else
                    {   // other parameters are injected from the DI container
                        args[parameterIndex] = DiResolver.GetInstance<object>(parameter.ParameterType);
                    }
                }
                parameterIndex++;

                /*
                                if (parameterIndex == 0)
                                {   // first parameter is always the View
                                    view = Resolver.GetViewForPresenterType<IView>(typeof(TPresenter));
                                        //| Could provide parameters for context-based injection of View.
                                    UiManager.AfterCreateView(ref view);
                                    args[parameterIndex] = view;
                                }
                                else if (parameterIndex < param.Length + 1)
                                {   // the next parameters (if present) are the Create method parameters (possibly including the Model)
                                    args[parameterIndex] = param[parameterIndex-1];
                                }
                                else
                                {   // other parameters are injected from the DI container
                                    args[parameterIndex] = DiResolver.GetInstance<object>(parameter.ParameterType);
                                }
                                parameterIndex++;
                */
            }

            var presenter = (TPresenter)TargetConstructor.Invoke(args);    // invoke the constructor
            DiResolver.BuildUp(presenter);                                 // inject properties

            UiManager.AfterCreatePresenter<TPresenter>(ref presenter, view);
            /*
            if (UiManager != null)
            {
                var newPresenter = UiManager.AfterCreatePresenter<TPresenter>(ref presenter, view);
                if(!presenter.Equals(newPresenter))
                {
                    MiscUtil.TryDispose(presenter);
                    presenter = newPresenter;
                }
            }
            */

            return presenter;
        }

        /// <summary>
        /// The type of the Presenter created by this factorys.
        /// </summary>
        protected Type TargetClass { get; private set; }

        /// <summary>
        /// The constructor of `TargetClass` to be used.
        /// </summary>
        protected ConstructorInfo TargetConstructor { get; private set; }

        /// <summary>
        /// Interface to the dependency injection container.
        /// Non null.
        /// </summary>
        protected readonly IDiResolver DiResolver;

        /// <summary>
        /// Resolver for resolving the View.
        /// Non null.
        /// </summary>
        protected readonly MvpResolver Resolver;

        /// <summary>
        /// UI Manager (places new views in the UI, etc.).
        /// Non null. May be a null object.
        /// </summary>
        protected readonly IResolverExtension UiManager;
    }


    public class PresenterFactory<TPresenter, TParam1> : PresenterFactory<TPresenter>,
        IPresenterFactory<TPresenter, TParam1>
        where TPresenter : IPresenter
    {
        public PresenterFactory(MvpResolver resolver, IDiResolver diResolver, IResolverExtension uiManager) : base(resolver, diResolver, uiManager)
        {
        }

        TPresenter IPresenterFactory<TPresenter, TParam1>.Create(TParam1 param)
        {
            return CreatePresenter(param);
        }
    }

    public class PresenterFactory<TPresenter, TParam1, TParam2> : PresenterFactory<TPresenter, TParam1>,
        IPresenterFactory<TPresenter>,
        IPresenterFactory<TPresenter, TParam1>,
        IPresenterFactory<TPresenter, TParam1, TParam2>
        where TPresenter : IPresenter
    {
        public PresenterFactory(MvpResolver resolver, IDiResolver diResolver, IResolverExtension uiManager) : base(resolver, diResolver, uiManager)
        {
        }

        TPresenter IPresenterFactory<TPresenter, TParam1, TParam2>.Create(TParam1 param1, TParam2 param2)
        {
            return CreatePresenter(param1, param2);
        }
    }

    public class PresenterFactory<TPresenter, TParam1, TParam2, TParam3> : PresenterFactory<TPresenter, TParam1, TParam2>,
        IPresenterFactory<TPresenter>,
        IPresenterFactory<TPresenter, TParam1>,
        IPresenterFactory<TPresenter, TParam1, TParam2>,
        IPresenterFactory<TPresenter, TParam1, TParam2, TParam3>
        where TPresenter : IPresenter
    {
        public PresenterFactory(MvpResolver resolver, IDiResolver diResolver, IResolverExtension uiManager) : base(resolver, diResolver, uiManager)
        {
        }

        TPresenter IPresenterFactory<TPresenter, TParam1, TParam2, TParam3>.Create(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            return CreatePresenter(param1, param2, param3);
        }
    }

    public class PresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4> : PresenterFactory<TPresenter, TParam1, TParam2, TParam3>,
        IPresenterFactory<TPresenter>,
        IPresenterFactory<TPresenter, TParam1>,
        IPresenterFactory<TPresenter, TParam1, TParam2>,
        IPresenterFactory<TPresenter, TParam1, TParam2, TParam3>,
        IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4>
        where TPresenter : IPresenter
    {
        public PresenterFactory(MvpResolver resolver, IDiResolver diResolver, IResolverExtension uiManager) : base(resolver, diResolver, uiManager)
        {
        }

        TPresenter IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4>.Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            return CreatePresenter(param1, param2, param3, param4);
        }
    }

    public class PresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5> : PresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4>,
        IPresenterFactory<TPresenter>,
        IPresenterFactory<TPresenter, TParam1>,
        IPresenterFactory<TPresenter, TParam1, TParam2>,
        IPresenterFactory<TPresenter, TParam1, TParam2, TParam3>,
        IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4>,
        IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5>
        where TPresenter : IPresenter
    {
        public PresenterFactory(MvpResolver resolver, IDiResolver diResolver, IResolverExtension uiManager) : base(resolver, diResolver, uiManager)
        {
        }

        TPresenter IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5>.Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            return CreatePresenter(param1, param2, param3, param4, param5);
        }
    }
}