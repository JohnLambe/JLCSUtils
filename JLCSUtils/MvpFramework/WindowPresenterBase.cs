using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvpFramework.Binding;

namespace MvpFramework
{
    public class WindowPresenterBase<TView, TModel> : PresenterBase<TView, TModel>, IWindowPresenter
            where TView : IView
    {
        public WindowPresenterBase(TView view, TModel model = default(TModel), IControlBinderFactory binderFactory = null) : base(view, model, binderFactory)
        {
        }

        public virtual void Close()
        {
        }

        public virtual void Close(object modalResult)
        {
        }

        public virtual object ShowModal()
        {
            return null;
        }

        protected virtual object ModalResult { get; set; }
    }
}
