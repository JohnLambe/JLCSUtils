using MvpFramework.Binding;
using MvpFramework.WinForms.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MvpFramework.WinForms.Binding;

namespace MvpFramework.WinForms.Binding
{
    /// <summary>
    /// Binds a View to a Model and Presenter.
    /// </summary>
    public class ViewBinder
    {
        /// <summary>
        /// Bind the model and presenter to the view.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="presenter"></param>
        /// <param name="binderFactory"></param>
        public virtual void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory, Control view)
        {
            if (binderFactory != null)
            {
                var controls = view.Controls;

                Binders = new List<IControlBinderExt>();
                var modelBinder = new ModelBinderWrapper(model);

                var viewTitle = modelBinder.Title;
                if (viewTitle != null)
                    view.Text = viewTitle;

                //TODO: var presenterBinder = new PresenterBinderWrapper(presenter);  then use presenterBinder where presenter is used after this.
                foreach (Control control in controls)
                {
                    var binder = binderFactory.Create(control, presenter);
                    if (binder != null)
                    {
                        Binders.Add(binder);
                        binder.BindModel(modelBinder, presenter);
                    }
                }
            }
        }

        /// <summary>
        /// Refresh the view, or a specified control on it, from the model.
        /// </summary>
        /// <param name="control">null to refresh the whole view, otherwise, this control and all children (direct and indirect) are refreshed.</param>
        public virtual void RefreshView(Control control)
        {
            if (Binders != null)
            {
                foreach (var binder in Binders)
                {
                    if (control == null || binder.IsInControl(control))
                        binder.Refresh();
                }
            }
        }

        /// <summary>
        /// Collection of binders for the controls in this view.
        /// </summary>
        protected virtual IList<IControlBinderExt> Binders { get; private set; }
    }

}
