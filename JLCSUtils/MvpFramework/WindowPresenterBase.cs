using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvpFramework.Binding;

namespace MvpFramework
{
    /// <summary>
    /// Base class for Presenters that may show a window. (They can also have non-window views).
    /// </summary>
    /// <typeparam name="TView"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public class WindowPresenterBase<TView, TModel> : PresenterBase<TView, TModel>, IWindowPresenter
            where TView : IWindowView
    {
        public WindowPresenterBase(TView view, TModel model = default(TModel), IControlBinderFactory binderFactory = null)
            : base(view, model, binderFactory)
        {
        }

        public virtual void Close()
        {
            View.Close();
        }

        protected virtual void Close(object modalResult)
        {
            ModalResult = modalResult;
        }

        /// <summary>
        /// Show the view.
        /// </summary>
        /// <returns></returns>
        public override object Show()
        {
            var result = base.Show();
            View.Show();
            return result;
        }

        public virtual object ShowModal()
        {
            View.ShowModal();
//            OnShowModal?.Invoke(this, EventArgs.Empty);
            return ModalResult;
        }

        /// <summary>
        /// Bind to the view.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="binderFactory"></param>
        protected override void Bind(TView view, IControlBinderFactory binderFactory)
        {
            base.Bind(View,binderFactory);
            view.ViewVisibilityChanged += View_ViewVisibilityChanged;
        }

        /// <summary>
        /// The value to be returned from the <see cref="ShowModal"/> method.
        /// </summary>
        protected virtual object ModalResult { get; set; }

//        public virtual event EventHandler OnShowModal;
    }
}
