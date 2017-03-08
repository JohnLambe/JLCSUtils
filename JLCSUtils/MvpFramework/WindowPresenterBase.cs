using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvpFramework.Binding;

namespace MvpFramework
{
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

        public virtual object ShowModal()
        {
            View.ShowModal();
//            OnShowModal?.Invoke(this, EventArgs.Empty);
            return ModalResult;
        }

        /// <summary>
        /// The value to be returned from the <see cref="ShowModal"/> method.
        /// </summary>
        protected virtual object ModalResult { get; set; }

//        public virtual event EventHandler OnShowModal;
    }
}
