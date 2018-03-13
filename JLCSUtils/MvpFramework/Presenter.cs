using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DiExtension;
using MvpFramework.Binding;
using DiExtension.Attributes;
using JohnLambe.Util;

namespace MvpFramework
{
    /// <summary>
    /// A presenter that implements <see cref="IDisposable"/>.
    /// </summary>
    public interface IDisposablePresenter : IPresenter, IDisposable
    {
    }

    /// <summary>
    /// Base class for Presenters.
    /// </summary>
    /// <typeparam name="TView">The type of the View. Should be an interface.</typeparam>
    /// <typeparam name="TModel">The type of the Model.
    /// Can be anything. Using primitive types is not recommended and may not be supported in future versions.</typeparam>
    public class PresenterBase<TView, TModel> : IPresenter, INotifyOnDispose
        where TView : IView
    {
        /// <summary>
        /// </summary>
        /// <param name="view">The View for this Presenter.</param>
        /// <param name="model">The Model.</param>
        /// <param name="binderFactory"></param>
        public PresenterBase(
            TView view,                                                   // resolved by MVP framework
            [MvpParam] TModel model = default(TModel),                               // from parameter when creating
            [Inject] IControlBinderFactory binderFactory = null           // injected by DI
            )
        {
            Contract.Requires(view != null);
            View = view;
            Model = model;
            Bind(view, binderFactory);
        }

        /// <summary>
        /// Iff true, this is disposed when the view is closed.
        /// </summary>
        public virtual bool DisposeOnClose { get; set; } = true;

        /// <summary>
        /// Bind to the view.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="binderFactory"></param>
        protected virtual void Bind(TView view, IControlBinderFactory binderFactory)
        {
            View.Bind(Model, this, binderFactory);
//            view.ViewVisibilityChanged += View_ViewVisibilityChanged;
        }

        /// <summary>
        /// Handles the ViewClosing event of the View.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void View_ViewVisibilityChanged(object sender, ViewVisibilityChangedEventArgs args)
        {
            ViewVisibilityChanged?.Invoke(sender, args);               // fire this Presenter's ViewClosing event
            if (args.Closed && DisposeOnClose)
                Dispose();
        }

        /// <summary>
        /// Show the view.
        /// </summary>
        /// <returns></returns>
        public virtual object Show()
        {
//            View.Show();
            return null;
        }

        /// <summary>
        /// Validate the model if the View supports it (implements <seealso cref="IValidatableView"/>).
        /// </summary>
        /// <param name="model">null to validate the model, otherwise an object to be validated.</param>
        /// <returns>true iff valid, or if the View does not support this validation.</returns>
        /// <seealso cref="ViewBinderBase{TControl}.ValidateModel"/>
        protected virtual bool ValidateModel(object model = null)
        {
            return (View as IValidatableView)?.ValidateModel() ?? true;
        }

        #region Dispose

        /// <summary>
        /// <see cref="IDisposable.Dispose"/>
        /// </summary>
        public virtual void Dispose()
        {
            (View as IDisposable)?.Dispose();
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Fired when this instance is disposed.
        /// </summary>
        public virtual event EventHandler Disposed;

        #endregion

        /// <summary>
        /// The MVP Model.
        /// </summary>
        protected virtual TModel Model { get; private set; }

        /// <summary>
        /// The View for this Presenter.
        /// </summary>
        protected readonly TView View;
        //protected virtual TView View { get; private set; }

        /// <summary>
        /// Fired when the view opens or closes.
        /// </summary>
        public virtual event ViewVisibilityChangedDelegate ViewVisibilityChanged;
    }
}
