using System;

namespace MvpFramework
{
    /// <summary>
    /// For extension - provides extension points for <see cref="MvpResolver"/>:
    /// Can be used to handle placement of views in the user interface,
    /// finding existing presenters (and determining when an existing presenter should be used instead of creating a new one).
    /// </summary>
    public interface IResolverExtension
    {
        // These methods are declared in the order in which they are called:

        /// <summary>
        /// Called before creating a presenter (and view).
        /// This can provide a different presenter to be used instead of what would otherwise be created,
        /// throw an exception to prevent the presenter from being created,
        /// or return default(TPresenter), for the default behavior.
        /// <para>If this returns a Presenter or throws an exception, the other methods of this interface are not called.</para>
        /// </summary>
        /// <typeparam name="TPresenter">The type of the Presenter (it can be anything assignable to this).</typeparam>
        /// <param name="presenterType">The concrete type of the presenter to be created if not intercepted.</param>
        /// <param name="param">The parameters to the IPresenterFactory.Create method.</param>
        /// <returns>The presenter to be used, or <code>default(TPresenter)</code> to cause it to be created in the normal way.</returns>
        /// <exception>If this throws an exception, it is thrown to the code that tried to create the presenter/view, and the presenter and view are not created.</exception>
//        TPresenter BeforeCreatePresenter<TPresenter>(Type presenterType, params object[] param)
//            where TPresenter : IPresenter;
        TPresenter BeforeCreatePresenter<TPresenter>(Type presenterType, ResolverExtensionContext param)
            where TPresenter : IPresenter;

        /// <summary>
        /// Called after creating a view.
        /// The presenter will not have been created at this point.
        /// An implementor of this method can modify or replace the view (before it is passed to the presenter).
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <param name="presenterType">The concrete type of the presenter to be created if not intercepted.</param>
        /// <param name="param">Parameters to the 'Create' method.</param>
        /// <param name="view">The new view.
        /// This can modify or replace (assign) this. If this is assigned, the implementor is responsible for disposing the original one if it impleemnts <see cref="IDisposable"/>, unless it is keeping a reference to it.
        /// </param>
        /// <returns></returns>
        /// <exception>If this throws an exception, it is thrown to the code that tried to create the presenter/view, and the presenter is not created.</exception>
        ResolverExtensionStatus AfterCreateView<TView>(Type presenterType, ResolverExtensionContext param, ref TView view)
            where TView : IView;

        /// <summary>
        /// Called after creating a view and presenter.
        /// This can provide a different presenter to be used instead of this one (by assigning <paramref name="presenter"/>),
        /// or throw an exception to prevent the presenter from being returned.
        /// <para></para>
        /// </summary>
        /// <typeparam name="TPresenter">The type of the presenter.</typeparam>
        /// <param name="presenter">The new presenter.</param>
        /// <param name="param">Parameters to the 'Create' method.</param>
        /// <param name="view">The View of the new presenter (which will have already been given to the presenter).</param>
        /// <returns></returns>
        /// <exception>If this throws an exception, it is thrown to the code that tried to create the p</exception>
        ResolverExtensionStatus AfterCreatePresenter<TPresenter>(ref TPresenter presenter, ResolverExtensionContext param, IView view)
            where TPresenter : IPresenter;

    }


    /// <summary>
    /// Parameters / context passed to methods of <see cref="IResolverExtension"/>.
    /// </summary>
    public class ResolverExtensionContext
    {
        public ResolverExtensionContext(object[] createParameters)
        {
            this.CreateParameters = createParameters;
        }

        /// <summary>Parameters to the 'Create' method.</summary>
        public virtual object[] CreateParameters { get; set; }

        /// <summary>
        /// True iff the view is nested in another one.
        /// See <see cref="INestedPresenterFactory"/>.
        /// </summary>
        public virtual bool Nested { get; set; }
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
        /*
        public TPresenter BeforeCreatePresenter<TPresenter>(Type presenterType, params object[] param)
            where TPresenter : IPresenter
        {
            return BeforeCreatePresenter(presenterType, new ResolverExtensionContext(param));
        }
        */
        public TPresenter BeforeCreatePresenter<TPresenter>(Type presenterType, ResolverExtensionContext param) where TPresenter : IPresenter
        {
            return default(TPresenter);
        }

        public virtual ResolverExtensionStatus AfterCreateView<TView>(Type presenterType, ResolverExtensionContext context, ref TView view) where TView : IView
        {
            return ResolverExtensionStatus.Default;
        }

        public virtual ResolverExtensionStatus AfterCreatePresenter<TPresenter>(ref TPresenter presenter, ResolverExtensionContext context, IView view) where TPresenter : IPresenter
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