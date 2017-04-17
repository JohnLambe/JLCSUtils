using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MvpFramework.Binding;
using static System.Windows.Forms.Control;
using JohnLambe.Util.Reflection;
using MvpFramework.WinForms.Binding;

namespace MvpFramework.WinForms
{

    /// <summary>
    /// Optional base class for Views.
    /// </summary>
    public class ViewBase : UserControl, IView
    {
        public ViewBase()
        {
        }

        #region Binding

        /// <summary>
        /// Bind the model and presenter to this view.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="presenter"></param>
        /// <param name="binderFactory"></param>
        public virtual void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory)
        {
            ViewBinder = new ViewBinder();
            ViewBinder.Bind(model, presenter, binderFactory, this);

            // Set the 'Model' property if there is one: 
            // (So derived classes can declare one of the expected type. This doesn't use a type parameter because the Forms Designer does not support it.)
            ReflectionUtil.TrySetPropertyValue(this, "Model", model);
            //TODO: Move to non-WinForms-specific ViewBinder

            /*
            if (binderFactory != null)
            {
                Binders = new List<IControlBinder>();
                var modelBinder = new ModelBinderWrapper(model);
                //TODO: var presenterBinder = new PresenterBinderWrapper(presenter);  then use presenterBinder where presenter is used after this.
                foreach (Control control in Controls)
                {
                    var binder = binderFactory.Create(control, presenter);
                    if (binder != null)
                    {
                        Binders.Add(binder);
                        binder.BindModel(modelBinder, presenter);
                    }
                }
            }
            */
        }

        /*
        /// <summary>
        /// Collection of binders for the controls in this view.
        /// </summary>
        protected virtual IList<IControlBinder> Binders { get; private set; }
        */

        /// <summary>
        /// (Re)populate the view from the model (to update it when the model changes).
        /// </summary>
        public void RefreshView()
        {
            RefreshView(null);
        }

        /// <summary>
        /// Refresh the view, or a specified control on it, from the model.
        /// </summary>
        /// <param name="control">null to refresh the whole view, otherwise, this control and all children (direct and indirect) are refreshed.</param>
        protected virtual void RefreshView(Control control)
        {
            ViewBinder.RefreshView(control);
            /*
            if (Binders != null)
            {
                foreach (var binder in Binders)
                {
                    if(control == null || )
                    binder.Refresh();
                }
            }
            */
        }

        /// <summary>
        /// Binds this View to the Model and Presenter.
        /// </summary>
        protected virtual ViewBinder ViewBinder { get; private set; }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ViewBase
            // 
            this.Name = "ViewBase";
            //            this.Size = new System.Drawing.Size(500, 300);
            this.ResumeLayout(false);
        }

    }

}
