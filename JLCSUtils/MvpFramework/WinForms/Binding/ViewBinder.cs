using MvpFramework.Binding;
using MvpFramework.WinForms.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MvpFramework.WinForms.Binding;
using JohnLambe.Util.Reflection;

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
            View = view;
            ModelBinder = new ModelBinderWrapper(model);

            if (binderFactory != null)
            {
                Binders = new List<IControlBinder>();

                //TODO: var presenterBinder = new PresenterBinderWrapper(presenter);  then use presenterBinder where presenter is used after this.
                BindControl(view, binderFactory, presenter);   // bind the root control recursively
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        /// <param name="binderFactory"></param>
        /// <param name="presenter"></param>
        protected virtual void BindControl(Control control, IControlBinderFactory binderFactory, IPresenter presenter)
        {
            var binder = binderFactory.Create(control);
            if (binder != null)
            {
                Binders.Add(binder);
                /*
                if (binder is IEmbeddedView)
                {
                    var viewId = ((IEmbeddedView)binder).ViewId;
                    var subModel = ReflectionUtils.TryGetPropertyValue<object>(binder,viewId);
                    IPresenter subPresenter = ReflectionUtils.TryGetPropertyValue<IPresenter>(presenter, viewId);
                    ((IEmbeddedView)binder).Bind(subModel, subPresenter, binderFactory);
                    //TODO
                }
                else */
                {
                    binder.BindModel(ModelBinder, presenter);
                }
            }

            var controls = control.Controls;
            foreach (Control childControl in controls)
            {
                BindControl(childControl, binderFactory, presenter);
            }
        }

        /// <summary>
        /// Refresh the view, or a specified control on it, from the model.
        /// </summary>
        /// <param name="control">null to refresh the whole view, otherwise, this control and all children (direct and indirect) are refreshed.</param>
        public virtual void RefreshView(Control control)
        {
            var viewTitle = ModelBinder.Title;
            if (viewTitle != null)
                View.Text = viewTitle;

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
        protected virtual IList<IControlBinder> Binders { get; private set; }

        /// <summary>
        /// The binder for the model.
        /// </summary>
        protected virtual ModelBinderWrapper ModelBinder { get; set; }

        /// <summary>
        /// The bound view.
        /// </summary>
        protected virtual Control View { get; set; }
    }

}
