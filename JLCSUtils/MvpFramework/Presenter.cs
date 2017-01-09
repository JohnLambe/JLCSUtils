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

namespace MvpFramework
{

    /// <summary>
    /// Interface of an MVP Presenter, as seen by other Presenters.
    /// </summary>
    public interface IPresenter
    {
        /// <summary>
        /// Show the Presenter.
        /// May be modal or non-modal (derived interfaces may specify which).
        /// </summary>
        /// <returns>Depends on the Presenter (may be defined on the derived interface). May be null.</returns>
        object Show();
        //| TODO: Consider removing this (with derived interfaces having their own way to show them).
        //| TODO: Consider using a different name, to avoid conflict with a 'Show' method of classes that may be used as base classes of implementations of this.
        //| This could use a generic type for the return value.
    }

    /// <summary>
    /// Base class for Presenters.
    /// </summary>
    /// <typeparam name="TView">The type of the View. Should be an interface.</typeparam>
    /// <typeparam name="TModel">The type of the Model.
    /// Can be anything. Using primitive types is not recommended and may not be supported in future versions.</typeparam>
    public class PresenterBase<TView, TModel> : IPresenter
        where TView : IView
    {
//        public static Type ModelType => typeof(TModel);

        public PresenterBase(
            TView view,                                                   // resolved by MVP framework
            TModel model = default(TModel),                               // from parameter when creating
            [Inject] IControlBinderFactory binderFactory = null           // injected by DI
            )
        {
            Contract.Requires(view != null);
            View = view;
            Model = model;
            Bind(view, binderFactory);
        }

        /// <summary>
        /// Bind to the view.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="binderFactory"></param>
        protected virtual void Bind(TView view, IControlBinderFactory binderFactory)
        {
            View.Bind(Model, this, binderFactory);

        }

        public virtual object Show()
        {
            View.Show();
            return null;
        }

        /// <summary>
        /// <see cref="IDisposable.Dispose"/>
        /// </summary>
        public virtual void Dispose()
        {
            if(View is IDisposable)
                ((IDisposable)View).Dispose();
        }


        protected virtual TModel Model { get; private set; }
            // make readonly ?

        protected readonly TView View;
    }
}
