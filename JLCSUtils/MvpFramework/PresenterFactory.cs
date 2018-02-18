using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

using DiExtension;
using DiExtension.Attributes;
using JohnLambe.Util;
using JohnLambe.Util.Text;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;

namespace MvpFramework
{
    /// <summary>
    /// Generic factory for creating Presenters.
    /// Could be subclassed for custom presenter factory logic.
    /// </summary>
    /// <typeparam name="TPresenter"></typeparam>
    public class PresenterFactory<TPresenter> :
        IPresenterFactory<TPresenter>,
        INestedPresenterFactory
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
            this.UiManager = uiManager ?? new NullResolverExtension();
        }

        protected virtual void Init()
        {   // This is separate from the constructor since it involves resolving items which may come from a DI container,
            // and SimpleInjector cannot register items after any resolve (so this should be called only after everything is registered).
            if (TargetConstructor == null)    // if the target constructor is not resolved yet (it is resolved when first needed)
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
                    Resolver.ThrowException("Failed to resolve Presenter type or constructor: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Create the Presenter.
        /// </summary>
        /// <returns></returns>
        //TPresenter IPresenterFactory<TPresenter>.Create()
        //|TODO?: Consider making virtual.
        public TPresenter Create()
        {
            return CreatePresenter();
        }

        /// <summary>
        /// Create the Presenter (with its View).
        /// </summary>
        /// <param name="createArguments">Arguments to the 'Create' method.</param>
        /// <returns>the new Presenter.</returns>
        protected virtual TPresenter CreatePresenter(params object[] createArguments)
        {
            try
            {
                Init();

                IDiResolver currentDiResolver = DiResolver;
                if (EffectiveUseChildContext && DiResolver is IChainableDiResolver)
                    currentDiResolver = ((IChainableDiResolver)DiResolver).CreateChildContext();

                var resolverContext = new ResolverExtensionContext(createArguments)
                {
                    Nested = ContainingView != null
                    // DiResolver = currentDiResolver ?
                };

                var existingPresenter = UiManager.BeforeCreatePresenter<TPresenter>(TargetClass, resolverContext);
                if (existingPresenter != null)
                    return existingPresenter;

                var constructorParameters = TargetConstructor.GetParameters();   // constructor parameters

                // Populate the view:
                IView view = null;
                if (constructorParameters.Any())            // if the Presenter constructor has at least one parameter
                {
                    if (SuppliedView != null)     // if a View is supplied
                    {
                        view = SuppliedView;      // use it
                    }
                    else
                    {
                        INestedViewPlaceholder viewParent = null;
                        if (ContainingView != null)
                        {
                            view = GetNestedView(ContainingView, NestedViewId, out viewParent);
                        }

                        if (view == null)
                        {
                            // Determine the view to be injected (if there are no parameters, the View is not injected):
                            try
                            {
                                // Try resolving for the concrete type first.
                                // This is important for KnownPresenterFactory, where TPresenter may be a base interface (even IPresenter),
                                // but still relevant for other cases (for exmaple, a particular Presenter implementation may have its own View that overrides
                                // the usual one for the presenter's interface - Views are often specific to Presenter implementations).
                                view = Resolver.GetViewForPresenterType<IView>(TargetClass);
                            }
                            catch (Exception)   //TODO: Restrict exception type
                            {   // if resolving the view for the concrete presenter type fails, try for the declared type (usually an interface; possibly a base class) of presenter being created:
                                view = Resolver.GetViewForPresenterType<IView>(typeof(TPresenter));
                            }

                            if (viewParent != null)      // if the view has to be placed in a specifc parent
                            {
                                if (view is INestableView)    // if it has the ability to have its parent assigned
                                {
                                    viewParent.NestedView = (INestableView)view;
                                    //((INestableView)view).ViewParent = viewParent;
                                }
                                else   // Exception if viewParent != null && !(view is INestableView)
                                {
                                    throw new MvpResolutionException("Attempt to nest non-nestable View: "
                                        + view + " in " + viewParent
                                        + NL + "(View must implement " + nameof(INestableView) + ")");
                                    //| Alternativelty, if view is of the control class for its UI framework and it has a Parent property, we could use it.
                                    //| This would require a UI-framework-specific handler to provide the way of assigning controls to parents in the UI framework.
                                    //| But implementing ViewParent on a base class basically provides the similar functionality for this purpose, and is simpler.
                                }
                            }
                        }
                    }
                    //| Could provide parameters for context-based injection of View.
                    try
                    {
                        UiManager.AfterCreateView(TargetClass, resolverContext, ref view);
                    }
                    catch (Exception)
                    {
                        MiscUtil.TryDispose(view);
                        throw;
                    }
                }

                var args = currentDiResolver.PopulateArgs(constructorParameters, createArguments,
                    DiMvpResolver.AttributeSourceSelector,
                    1    // skip the first parameter (it's for the View)
                    );

                int paramIndex = 0;
                foreach (var arg in args)   // for each of the (now populated) arguments
                {
                    if (paramIndex > 0)    // ignore the first parameter - the View
                    {
                        var attribute = constructorParameters[paramIndex].GetCustomAttribute<MvpNestedAttribute>();
                        if (attribute != null)    // if flagged as nested
                        {                         // nested presenter factories are created as regular presenter factories above (by DiResolver.PopulateArgs), but have additional properties configured here.
                            if (arg is INestedPresenterFactory)                        // and the argument supports this
                            {
                                ((INestedPresenterFactory)arg).ContainingView = view;            // provide the View of the Presenter being created
                                ((INestedPresenterFactory)arg).NestedViewId = attribute.NestedViewId ?? ReflectionUtil.CamelCaseToPascalCase(constructorParameters[paramIndex].Name);     // provide the View of the Presenter being created
                            }
                            else
                            {
                                throw new MvpResolutionException("Invalid use of " + typeof(MvpNestedAttribute) + ": This can be used only on Presenter Factories implementing " + nameof(INestedPresenterFactory));
                            }
                        }
                        
                        var sharedConextAttribute = constructorParameters[paramIndex].GetCustomAttribute<MvpSharedContextAttribute>();
                        if (sharedConextAttribute != null)    // if flagged as shared context
                        {
                            if (arg is ISharedContextPresenterFactory)                        // and the argument supports this
                            {
                                ((ISharedContextPresenterFactory)arg).UseChildContext = false;
                            }
                            else
                            {
                                throw new MvpResolutionException("Invalid use of " + typeof(MvpSharedContextAttribute) + ": This can be used only on Presenter Factories implementing " + nameof(ISharedContextPresenterFactory));
                            }
                        }
                        
                    }
                    paramIndex++;
                }

                if (args.Length > 0)
                {
                    args[0] = view;   // assign the view (determined before creating the `args` array)
                }

                var presenter = (TPresenter)TargetConstructor.Invoke(args);    // invoke the constructor
                try
                {
                    currentDiResolver.BuildUp(presenter);                                 // inject properties
                }
                catch(Exception)        //TODO: catch only errors that imply that `presenter` doesn't support property injection
                {
                }

                try
                {
                    UiManager.AfterCreatePresenter<TPresenter>(ref presenter, resolverContext, view);
                }
                catch (Exception)
                {
                    MiscUtil.TryDispose(presenter);
                    MiscUtil.TryDispose(view);
                    throw;
                }

                return presenter;
            }
            catch(Exception ex)
            {
                throw new MvpResolutionException("Error on creating presenter: Type: " + typeof(TPresenter) + NL
                    + (TargetClass != null ? "Presenter class: " + TargetClass + NL : "")
                    + (TargetConstructor != null ? "Target Presenter constructor: " + TargetConstructor + NL : "")
                    + (SuppliedView != null ? "With supplied View: " + SuppliedView + NL : "")
                    + ex.Message
                    , ex);
                //TODO: Include more information in error message
            }
        }

        /// <summary>
        /// <inheritdoc cref="IContainerView.GetNestedView(string, out INestedViewPlaceholder)"/>
        /// </summary>
        /// <param name="containingView">The view in which to find the nested view.</param>
        /// <param name="nestedViewId">The ID of the nested view to find.</param>
        /// <param name="viewParent"></param>
        /// <returns></returns>
        protected virtual IView GetNestedView(IView containingView, string nestedViewId, out INestedViewPlaceholder viewParent)
        {
            if (containingView is IContainerView)
            {
                return ((IContainerView)containingView).GetNestedView(nestedViewId, out viewParent);
            }
            else
            {
                viewParent = null;
                return null;
            }
        }

        /// <summary>
        /// The View to be used for any presenter created by this class.
        /// null (the default and usual case) to have a View created on each Create call.
        /// </summary>
        IView INestedPresenterFactory.View { get; set; }
        //| This could have been protected, instead of having the INestedPresenterFactory inteface.
        //| This way is more extendable - unrelated classes could implement the IPresenterFactory<> interfaces.

        /// <summary>
        /// Same as <see cref="INestedPresenterFactory.View"/>.
        /// </summary>
        protected virtual IView SuppliedView
            => ((INestedPresenterFactory)this).View;

        /// <summary>
        /// <inheritdoc cref="ISharedContextPresenterFactory.UseChildContext"/>
        /// </summary>
        /// <seealso cref="EffectiveUseChildContext"/>
        public virtual bool? UseChildContext { get; set; }

        /// <summary>
        /// Whether a child context is to be created (the effective value of <see cref="UseChildContext"/>, applying a default if it is null).
        /// </summary>
        /// <seealso cref="UseChildContext"/>
        public virtual bool EffectiveUseChildContext => UseChildContext ?? ContainingView == null;
        // By default, nested views share the context of their container.

        /// <summary>
        /// True iff views created by this factory are modal.
        /// </summary>
        public virtual bool Modal { get; set; } = true;
        //| Could be replaced by an object that encapsulates this and more settings relating to how a view is shown.

        /// <summary>
        /// The type of the Presenter created by this factory.
        /// This must not be null if <see cref="TargetConstructor"/> is not null.
        /// </summary>
        [Nullable]
        protected virtual Type TargetClass { get; set; }

        /// <summary>
        /// The constructor of <see cref="TargetClass"/> to be used.
        /// Must be null if <see cref="TargetClass"/> is null.
        /// </summary>
        [Nullable]
        protected virtual ConstructorInfo TargetConstructor { get; set; }

        #region INestedPresenterFactory

        /// <inheritdoc cref="INestedPresenterFactory.ContainingView"/>
        public virtual IView ContainingView { get; set; }

        /// <inheritdoc cref="INestedPresenterFactory.NestedViewId"/>
        public virtual string NestedViewId { get; set; }

        #endregion

        /// <summary>
        /// Interface to the dependency injection container.
        /// </summary>
        [NotNull]
        protected readonly IDiResolver DiResolver;

        /// <summary>
        /// Resolver for resolving the View.
        /// </summary>
        [NotNull]
        protected readonly MvpResolver Resolver;

        /// <summary>
        /// UI Manager (places new views in the UI, etc.).
        /// Non null. May be a null object.
        /// </summary>
        [NotNull]
        protected readonly IResolverExtension UiManager;

        /// <summary>
        /// Line separator used in error messages.
        /// </summary>
        protected const string NL = "\r\n";
    }


    #region KnownPresenterFactory

    /// <summary>
    /// Presenter factory for use when the target class is known and supplied by the consumer of the factory.
    /// </summary>
    /// <typeparam name="TPresenter"></typeparam>
    public class KnownPresenterFactory<TPresenter> : PresenterFactory<TPresenter>
        where TPresenter : IPresenter
    {
        /// <summary>
        /// </summary>
        /// <param name="resolver"><see cref="PresenterFactory{TPresenter}.Resolver"/></param>
        /// <param name="diResolver"><see cref="PresenterFactory{TPresenter}.DiResolver"/></param>
        /// <param name="uiManager"><see cref="PresenterFactory{TPresenter}.UiManager"/></param>
        /// <param name="targetClass">The concrete class of the presenter created by this factory - <see cref="PresenterFactory{TPresenter}.TargetClass"/>.</param>
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
    /// <typeparam name="TParam1">The type of the first parameter to the 'Create' method.</typeparam>
    /// <typeparam name="TParam2">The type of the 2nd parameter to the 'Create' method.</typeparam>
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
    /// <typeparam name="TParam1">The type of the first parameter to the 'Create' method.</typeparam>
    /// <typeparam name="TParam2">The type of the 2nd parameter to the 'Create' method.</typeparam>
    /// <typeparam name="TParam3">The type of the 3rd parameter to the 'Create' method.</typeparam>
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

        public virtual TPresenter Create(TParam1 param)
//        TPresenter IPresenterFactory<TPresenter, TParam1>.Create(TParam1 param)
        {
            return CreatePresenter(param);
        }

        public virtual TPresenter Show(TParam1 param)
        {
            var presenter = Create(param);
            presenter.Show();
            return presenter;
        }

        /*
        /// <summary>
        /// Create and show the presenter modally, and dispose it if necessary.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual object ShowModal(TParam1 param)
        {
            object result = null;
            var presenter = Create(param);
            try
            {
                result = presenter.ShowModal();
            }
            finally
            {
                if (presenter is IDisposable)
                    ((IDisposable)presenter).Dispose();
            }
            return result;
        }
        */
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

        public virtual TPresenter Create(TParam1 param1, TParam2 param2)
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

        public virtual TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3)
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

        public virtual TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
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

        public virtual TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        //TPresenter IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5>.Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5)
        {
            return CreatePresenter(param1, param2, param3, param4, param5);
        }
    }

    public class PresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> : PresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5>,
        IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        where TPresenter : IPresenter
    {
        public PresenterFactory(MvpResolver resolver, IDiResolver diResolver, IResolverExtension uiManager) : base(resolver, diResolver, uiManager)
        {
        }

        public virtual TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6)
        {
            return CreatePresenter(param1, param2, param3, param4, param5, param6);
        }
    }

    public class PresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7> : PresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>,
        IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>
        where TPresenter : IPresenter
    {
        public PresenterFactory(MvpResolver resolver, IDiResolver diResolver, IResolverExtension uiManager) : base(resolver, diResolver, uiManager)
        {
        }

        public virtual TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7)
        {
            return CreatePresenter(param1, param2, param3, param4, param5, param6, param7);
        }
    }

    #endregion
}