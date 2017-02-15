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
            this.DiResolver = diResolver;
            this.Resolver = resolver;
            this.UiManager = uiManager ?? new NullUiManager();
        }

        protected virtual void Init()
        {   // This is separate from the constructor since it involves resolving items which may come from a DI container,
            // and SimpleInjector cannot register items after any resolve (so this should be called only after everything is registered).
            if (TargetConstructor == null)
            {
                try
                {
                    if (TargetClass == null)              // if target class is not already known (it can be assigned by a subclass)
                        TargetClass = Resolver.ResolvePresenterType(typeof(TPresenter));
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

        //TPresenter IPresenterFactory<TPresenter>.Create()
        public TPresenter Create()
        {
            return CreatePresenter();
        }

        /// <summary>
        /// Create the Presenter (with its View).
        /// </summary>
        /// <param name="param">Arguments to the 'Create' method.</param>
        /// <returns>the new Presenter.</returns>
        protected virtual TPresenter CreatePresenter(params object[] param)
        {
            Init();

            var existingPresenter = UiManager.BeforeCreatePresenter<TPresenter>(TargetClass,param);
            if (existingPresenter != null)
                return existingPresenter;

            var parameters = TargetConstructor.GetParameters();   // constructor parameters

            var args = DiUtil.PopulateArgs(DiResolver, parameters, param,
                parameter =>
                {
                    bool? fromCreateParam = null;
                    var attribute = parameter.GetCustomAttribute<InjectAttribute>();
                    if (attribute != null)
                    {
                        fromCreateParam = attribute is MvpInjectAttribute;    // attributed as a 'Create' parameter
                        // will be false if there was an InjectAttribute but not MvpInjectAttribute. 
                    }
                    // still null if there was no InjectAttribute.
                    return fromCreateParam;
            },
                1    // skip the first parameter (its for the View)
                );

            // Populate the view:
            IView view = null;
            if (parameters.Count() > 0)
            {
                try
                {
                    view = Resolver.GetViewForPresenterType<IView>(typeof(TPresenter));
                }
                catch (Exception)   //TODO: Exception type
                {   // if resolving the view for the declared presenter type (usually an interface) fails, try for the concrete type of presenter being created:
                    view = Resolver.GetViewForPresenterType<IView>(TargetClass);
                }
                //| Could provide parameters for context-based injection of View.
                try
                {
                    UiManager.AfterCreateView(TargetClass, param, ref view);
                }
                catch(Exception)
                {
                    MiscUtil.TryDispose(view);
                    throw;
                }
                args[0] = view;
            }


            /*
            object[] args = new object[parameters.Count()];       // constructor arguments
            IView view = null;

            // Populate the constructor arguments:
            int parameterIndex = 0;                               // index of the constructor parameter
            int createMethodParameterIndex = 0;
            bool? createParam = null;                             // true iff the current parameter is to be populated from the arguments to the 'Create' method
            foreach (var parameter in parameters)
            {
                //                if(parameter.ParameterType.IsAssignableFrom(typeof(TView)))
                if (parameterIndex == 0)
                {   // first parameter is always the View
                    try
                    {
                        view = Resolver.GetViewForPresenterType<IView>(typeof(TPresenter));
                    }
                    catch (Exception)   //TODO: Exception type
                    {   // if resolving the view for the declared presenter type (usually an interface) fails, try for the concrete type of presenter being created:
                        view = Resolver.GetViewForPresenterType<IView>(TargetClass);
                    }
                    //| Could provide parameters for context-based injection of View.
                    UiManager.AfterCreateView(ref view);
                    args[parameterIndex] = view;
                }
                else
                {
                    var attribute = parameter.GetCustomAttribute<InjectAttribute>();
                    if (attribute != null)
                    {
                        createParam = attribute is MvpInjectAttribute;    // attributed as a 'Create' parameter
                    }
                    else
                    {
                        createParam = parameterIndex < param.Length + 1;   // not specified as a 'Create' parameter, and its index is beyond the range of the 'Create' parameters
                        //TODO: To support [Inject] before last create parameter: createParam = createMethodParameterIndex < param.Length + 1;
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
            }
            */

            var presenter = (TPresenter)TargetConstructor.Invoke(args);    // invoke the constructor
            DiResolver.BuildUp(presenter);                                 // inject properties

            try
            {
                UiManager.AfterCreatePresenter<TPresenter>(ref presenter, param, view);
            }
            catch(Exception)
            {
                MiscUtil.TryDispose(presenter);
                MiscUtil.TryDispose(view);
                throw;
            }

            return presenter;
        }

        /// <summary>
        /// The type of the Presenter created by this factory.
        /// This must not be null if <see cref="TargetConstructor"/> is not null.
        /// </summary>
        protected virtual Type TargetClass { get; set; }

        /// <summary>
        /// The constructor of <see cref="TargetClass"/> to be used.
        /// Must be null if <see cref="TargetClass"/> is null.
        /// </summary>
        protected virtual ConstructorInfo TargetConstructor { get; set; }

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


    #region KnownPresenterFactory

    /// <summary>
    /// Presenter factory for use when the target class is known and supplied by the consumer of the factory.
    /// </summary>
    /// <typeparam name="TPresenter"></typeparam>
    /// <typeparam name="TParam1"></typeparam>
    public class KnownPresenterFactory<TPresenter> : PresenterFactory<TPresenter>
        where TPresenter : IPresenter
    {
        public KnownPresenterFactory(MvpResolver resolver, IDiResolver diResolver,
            IResolverExtension uiManager,
            Type targetClass
            ) : base(resolver, diResolver, uiManager)
        {
            this.TargetClass = targetClass;
        }
/*
        public virtual TPresenter Create()
        {
            return CreatePresenter();
        }
        */
    }

    /// <summary>
    /// Presenter factory for use when the target class is known and supplied by the consumer of the factory.
    /// </summary>
    /// <typeparam name="TPresenter"></typeparam>
    /// <typeparam name="TParam1"></typeparam>
    public class KnownPresenterFactory<TPresenter, TParam1> : PresenterFactory<TPresenter, TParam1>
        where TPresenter : IPresenter
    {
        public KnownPresenterFactory(MvpResolver resolver, IDiResolver diResolver,
            IResolverExtension uiManager,
            Type targetClass
            ) : base(resolver, diResolver, uiManager)
        {
            this.TargetClass = targetClass;
        }
    }

    /// <summary>
    /// Presenter factory for use when the target class is known and supplied by the consumer of the factory.
    /// </summary>
    /// <typeparam name="TPresenter"></typeparam>
    /// <typeparam name="TParam1"></typeparam>
    public class KnownPresenterFactory<TPresenter, TParam1, TParam2> : PresenterFactory<TPresenter, TParam1, TParam2>
        where TPresenter : IPresenter
    {
        public KnownPresenterFactory(MvpResolver resolver, IDiResolver diResolver,
            IResolverExtension uiManager,
            Type targetClass
            ) : base(resolver, diResolver, uiManager)
        {
            this.TargetClass = targetClass;
        }
    }

    /// <summary>
    /// Presenter factory for use when the target class is known and supplied by the consumer of the factory.
    /// </summary>
    /// <typeparam name="TPresenter"></typeparam>
    /// <typeparam name="TParam1"></typeparam>
    public class KnownPresenterFactory<TPresenter, TParam1, TParam2, TParam3> : PresenterFactory<TPresenter, TParam1, TParam2, TParam3>
        where TPresenter : IPresenter
    {
        public KnownPresenterFactory(MvpResolver resolver, IDiResolver diResolver,
            IResolverExtension uiManager,
            Type targetClass
            ) : base(resolver, diResolver, uiManager)
        {
            this.TargetClass = targetClass;
        }
    }

    #endregion


    #region PresenterFactory generic types

    public class PresenterFactory<TPresenter, TParam1> : PresenterFactory<TPresenter>,
        IPresenterFactory<TPresenter, TParam1>
        where TPresenter : IPresenter
    {
        public PresenterFactory(MvpResolver resolver, IDiResolver diResolver, IResolverExtension uiManager) : base(resolver, diResolver, uiManager)
        {
        }

        public TPresenter Create(TParam1 param)
//        TPresenter IPresenterFactory<TPresenter, TParam1>.Create(TParam1 param)
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

        public TPresenter Create(TParam1 param1, TParam2 param2)
        //TPresenter IPresenterFactory<TPresenter, TParam1, TParam2>.Create(TParam1 param1, TParam2 param2)
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

        public TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3)
        //TPresenter IPresenterFactory<TPresenter, TParam1, TParam2, TParam3>.Create(TParam1 param1, TParam2 param2, TParam3 param3)
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

        public TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        //TPresenter IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4>.Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
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

        public TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        //TPresenter IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5>.Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            return CreatePresenter(param1, param2, param3, param4, param5);
        }
    }

    #endregion
}