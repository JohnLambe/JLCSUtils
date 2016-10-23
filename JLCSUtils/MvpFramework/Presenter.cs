using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DiExtension;
using MvpFramework.Binding;

namespace MvpFramework
{

    /// <summary>
    /// Interface of an MVP Presenter, as seen by other Presenters.
    /// </summary>
    public interface IPresenter
    {
        void Show();
    }

    /// <summary>
    /// Base class for Presenters.
    /// </summary>
    /// <typeparam name="TView">The type of the View. Should be an interface.</typeparam>
    /// <typeparam name="TModel">The type of the Model.
    /// Can be anything. Maybe not a primitive type?</typeparam>
    public class PresenterBase<TView, TModel> : IPresenter
        where TView : IView
    {
//        public static Type ModelType => typeof(TModel);

        public PresenterBase(TView view, TModel model = default(TModel), IControlBinderFactory binderFactory = null)
        {
            Contract.Requires(view != null);
            View = view;
            Model = model;
            Bind(view, binderFactory);
        }

        protected virtual void Bind(TView view, IControlBinderFactory binderFactory)
        {
            View.Bind(Model, this, binderFactory);

        }

        public virtual void Show()
        {
            View.Show();
        }

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
