using System;

namespace MvpFramework
{
    /// <summary>
    /// For extension - provides extension points for <see cref="MvpResolver"/>:
    /// Can be used to handles placement of views in the user interface,
    /// finding existing presenters (and determining when an existing presenter should be used instead of creating a new one).
    /// </summary>
    public interface IResolverExtension
    {
        /// <summary>
        /// Called before creating a presenter.
        /// This can provide a different presenter to be used instead of what would otherwise be created,
        /// throw an exception to prevent the presenter from being created,
        /// or return null, for the default behavior.
        /// </summary>
        /// <typeparam name="TPresenter"></typeparam>
        /// <param name="param">The parameters to the PresenterFactory.Create method.</param>
        /// <returns></returns>
        TPresenter BeforeCreatePresenter<TPresenter>(params object[] param)
            where TPresenter : IPresenter;

        /// <summary>
        /// Called after creating a presenter.
        /// This can provide a different presenter to be used instead of this one (by assignined <paramref name="presenter"/>),
        /// or throw an exception to prevent the presenter from being returned.
        /// </summary>
        /// <typeparam name="TPresenter">The type of the presenter.</typeparam>
        /// <param name="presenter">The new presenter.</param>
        /// <param name="view">The View of the new presenter.</param>
        /// <returns></returns>
        ResolverExtensionStatus AfterCreatePresenter<TPresenter>(ref TPresenter presenter, IView view)
            where TPresenter : IPresenter;

        /// <summary>
        /// Called after creating a view.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <param name="view"></param>
        /// <returns></returns>
        ResolverExtensionStatus AfterCreateView<TView>(ref TView view)
            where TView : IView;
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
    /// null object implementation of <see cref="IResolverExtension"/>.
    /// </summary>
    public class NullUiManager : IResolverExtension
    {
        public TPresenter BeforeCreatePresenter<TPresenter>(params object[] param)
            where TPresenter : IPresenter
        {
            return default(TPresenter);
        }

        public ResolverExtensionStatus AfterCreatePresenter<TPresenter>(ref TPresenter presenter, IView view)
            where TPresenter : IPresenter
        {
            return ResolverExtensionStatus.Default;
        }

        public ResolverExtensionStatus AfterCreateView<TView>(ref TView view)
            where TView : IView
        {
            return ResolverExtensionStatus.Default;
        }
    }
}