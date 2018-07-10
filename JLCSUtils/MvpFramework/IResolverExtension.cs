using System;
using DiExtension;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Types;

namespace MvpFramework
{
    /// <summary>
    /// Provides extension points for <see cref="MvpResolver"/>:
    /// Can be used to handle placement of views in the user interface,
    /// finding existing presenters (and determining when an existing presenter should be used instead of creating a new one),
    /// and modifying the depndency injection context.
    /// <para>
    /// On creating a presenter, the methods are fired in this order:<br/>
    /// - StartInjection<br/>
    /// - BeforeCreatePresenter<br/>
    /// - AfterCreateView<br/>
    /// - AfterCreatePresenter
    /// - EndInjection
    /// </para>
    /// </summary>
    public interface IResolverExtension
    {
        // These methods are declared in the order in which they are called:

        //| Some items are passed as parameters separate from the ResolverExtensionContext to make it clear when they are available or modifiable.

        /// <summary>
        /// Called before injection of any parameters (and creating values to be injected) to the presenter.
        /// This can be used to change the DI context (<see cref="ResolverExtensionContext.DiResolver"/>), either by assigning a new one
        /// (for nested/child DI containers) or changing the state of the existing context.
        /// <para>
        /// If this throws and exception, later methods will not be called and the presenter will not be created.
        /// </para>
        /// </summary>
        /// <param name="context"></param>
        ResolverExtensionStatus StartInjection([NotNull] ResolverExtensionContext context);

        /// <summary>
        /// Called before creating a presenter (and view).
        /// This can provide a different presenter to be used instead of what would otherwise be created,
        /// throw an exception to prevent the presenter from being created,
        /// or return default(TPresenter), for the default behavior.
        /// <para>If this returns a Presenter or throws an exception, the other methods of this interface are not called.</para>
        /// <para><see href="ResolverExtensionContext.PresenterType"/> is the concrete type of the presenter to be created if not intercepted.</para>
        /// </summary>
        /// <typeparam name="TPresenter">The type of the Presenter (it can be anything assignable to this).</typeparam>
        /// <param name="param">The parameters to the IPresenterFactory.Create method.</param>
        /// <returns>The presenter to be used, or <code>default(TPresenter)</code> to cause it to be created in the normal way.</returns>
        /// <exception>If this throws an exception, it is thrown to the code that tried to create the presenter/view, and the presenter and view are not created.</exception>
        TPresenter BeforeCreatePresenter<TPresenter>([NotNull] ResolverExtensionContext param)
            where TPresenter : IPresenter;

        /// <summary>
        /// Called after creating a view.
        /// The presenter will not have been created at this point.
        /// An implementor of this method can modify or replace the view (before it is passed to the presenter).
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <param name="param">Parameters to the 'Create' method.</param>
        /// <param name="view">The new view.                                                                                                          s
        /// The implementation can modify or replace (assign) this. If this is assigned, the implementor is responsible for disposing the original one if it impleemnts <see cref="IDisposable"/>,
        /// unless it is keeping a reference to it.
        /// </param>
        /// <returns></returns>
        /// <exception>If this throws an exception, it is thrown to the code that tried to create the presenter/view, and the presenter is not created.</exception>
        ResolverExtensionStatus AfterCreateView<TView>([NotNull] ResolverExtensionContext param, ref TView view)
            where TView : IView;

        /// <summary>
        /// Called after creating a view and presenter.
        /// This can provide a different presenter to be used instead of this one (by assigning <paramref name="presenter"/>),
        /// or throw an exception to prevent the presenter from being returned.
        /// <para></para>
        /// </summary>
        /// <typeparam name="TPresenter">The type of the presenter.</typeparam>
        /// <param name="presenter">The new presenter. It can be replaced by assigning this.</param>
        /// <param name="param">Parameters to the 'Create' method.</param>
        /// <param name="view">The View of the new presenter (which will have already been given to the presenter).</param>
        /// <returns></returns>
        /// <exception>If this throws an exception, it is thrown to the code that tried to create the p</exception>
        ResolverExtensionStatus AfterCreatePresenter<TPresenter>([NotNull] ref TPresenter presenter, [NotNull] ResolverExtensionContext param, IView view)
            where TPresenter : IPresenter;

        /// <summary>
        /// Called after injection of any parameters (and creating values to be injected) to the presenter.
        /// This can restore changes made in <see cref="StartInjection(ResolverExtensionContext)"/>.
        /// (If <see cref="ResolverExtensionContext.DiResolver"/> is assigned a new value, it does not have to be restored. It is not used after this call.)
        /// </summary>
        /// <param name="context"></param>
        ResolverExtensionStatus EndInjection([NotNull] ResolverExtensionContext context);
    }


    /// <summary>
    /// Parameters / context passed to methods of <see cref="IResolverExtension"/>.
    /// The same instance of this is passed for all calls relating to creating the same presenter instance.
    /// </summary>
    //| `Nested` is read-only because if the extension changed it, the Presenter Factory wouldn't necessarily know what to do (if it set it to true, we wouldn't know how to nest it).
    //| `PresenterType` could be made modifiable by the extension, but we would then also have to provide the ability to modify PresenterFactory.TargetConstructor, which the extension would have to keep consistent,
    //| reducing the encapsulation of PresenterFactory.
    //| There is a case for providing TargetConstructor (read-only) and making CreateParameters writeable. (To be considered for a later version.)
    public class ResolverExtensionContext
    {
        public ResolverExtensionContext(object[] createParameters, Type presenterType = null, bool nested = false)
        {
            this.CreateParameters = createParameters;
            this.PresenterType = presenterType;
            this.Nested = nested;
        }

        /// <summary>
        /// The class of the Presenter that is about to be created.
        /// </summary>
        [Nullable]
        public virtual Type PresenterType { get; }

        /// <summary>Parameters to the 'Create' method.</summary>
        public virtual object[] CreateParameters { get; }

        /// <summary>
        /// True iff the view is nested in another one.
        /// See <see cref="INestedPresenterFactory"/>.
        /// </summary>
        public virtual bool Nested { get; }

        /// <summary>
        /// Whether a child dependency injection context (or equivalent) should be used for the new presenter and view.
        /// This is set to the value that is about to be used before calling <see cref="IResolverExtension.StartInjection(ResolverExtensionContext)"/>,
        /// and can be modified in that method.
        /// <para>
        /// This can be assigned in <see cref="IResolverExtension.StartInjection(ResolverExtensionContext)"/>, but not after it.
        /// </para>
        /// </summary>
        /// <exception cref="InvalidOperationException">An attempt was made to modify this after <see cref="IResolverExtension.StartInjection(ResolverExtensionContext)"/>.
        /// (NOT IMPLEMENTED YET).
        /// </exception>
        /// <seealso cref="PresenterFactory{TPresenter}.EffectiveUseChildContext"/>
        public virtual bool UseChildContext { get; set; }

        /// <summary>
        /// DI context used for injecting the presenter.
        /// Can be assigned in <see cref="IResolverExtension"/> implementations.
        /// </summary>
        [Nullable]
        public virtual IDiResolver DiResolver { get; set; }

        /// <summary>
        /// For use by <see cref="IResolverExtension"/> implementations.
        /// This is null when an instance is created and is not assigned nor used by the Presenter Factory.
        /// Since the same instance of this class is used on all calls to <see cref="IResolverExtension"/> for creation of the same presenter instance,
        /// it can use this to hold state between calls.
        /// </summary>
        [Nullable]
        public virtual object ExtensionData { get; set; }
    }


    /// <summary>
    /// To support extension in future versions.
    /// Currently instances of this always have the value `Default`.
    /// </summary>
    public enum ResolverExtensionStatus
    {
        Default = 0
    }


    /// <summary>
    /// Base class for <see cref="IResolverExtension"/> implementations, with default implementation,
    /// the same as <see cref="NullResolverExtension"/>.
    /// </summary>
    public abstract class ResolverExtensionBase : IResolverExtension
    {
        /// <inheritdoc cref="IResolverExtension.StartInjection(ResolverExtensionContext)"/>
        public virtual ResolverExtensionStatus StartInjection([NotNull] ResolverExtensionContext context)
        {
            return ResolverExtensionStatus.Default;
        }

        /// <inheritdoc cref="IResolverExtension.BeforeCreatePresenter{TPresenter}(ResolverExtensionContext)"/>
        public virtual TPresenter BeforeCreatePresenter<TPresenter>([NotNull] ResolverExtensionContext param) where TPresenter : IPresenter
        {
            return default(TPresenter);
        }

        /// <inheritdoc cref="IResolverExtension.AfterCreateView{TView}(ResolverExtensionContext, ref TView)"/>
        public virtual ResolverExtensionStatus AfterCreateView<TView>([NotNull] ResolverExtensionContext context, ref TView view) where TView : IView
        {
            return ResolverExtensionStatus.Default;
        }

        /// <inheritdoc cref="IResolverExtension.BeforeCreatePresenter{TPresenter}(ResolverExtensionContext)"/>
        public virtual ResolverExtensionStatus AfterCreatePresenter<TPresenter>([NotNull] ref TPresenter presenter, [NotNull] ResolverExtensionContext context, IView view)
            where TPresenter : IPresenter
        {
            return ResolverExtensionStatus.Default;
        }

        /// <summary>
        /// Determine whether a child DI context should be created for the new presenter and view.
        /// Called before <see cref="BeforeCreatePresenter"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true to use a </returns>
        /// <seealso cref="ResolverExtensionContext.UseChildContext"/>
        public virtual bool GetUseChildContext([NotNull] ResolverExtensionContext context)
        {
            return context.PresenterType.GetCustomAttribute<PresenterAttribute>()?.UseChildContextBool ?? context.UseChildContext;
        }

        /// <inheritdoc cref="IResolverExtension.EndInjection(ResolverExtensionContext)"/>
        public virtual ResolverExtensionStatus EndInjection([NotNull] ResolverExtensionContext context)
        {
            return ResolverExtensionStatus.Default;
        }
    }


    /// <summary>
    /// null object implementation of <see cref="IResolverExtension"/>.
    /// </summary>
    //| This is 'sealed' because extending it would be a Liskov Substitution Principle violation.
    public sealed class NullResolverExtension : ResolverExtensionBase
    {
    }

}