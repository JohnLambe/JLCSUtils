using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MvpFramework.Binding;
using static System.Windows.Forms.Control;
using JohnLambe.Util.Reflection;
using MvpFramework.WinForms.Binding;
using JohnLambe.Util;
using System.ComponentModel;

namespace MvpFramework.WinForms
{
    /// <summary>
    /// Optional base class for Views.
    /// </summary>
    public class ViewBase : UserControl, IView, IOptionUpdate, IContainerView, INotifyOnDispose, INestableView
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
        }

        /// <summary>
        /// (Re)populate the view from the model (to update it when the model changes).
        /// </summary>
        // Implements IView method.
        // Not virtual because the other overload should be overridden instead.
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
        }

        /// <summary>
        /// Binds this View to the Model and Presenter.
        /// </summary>
        protected virtual ViewBinder ViewBinder { get; private set; }

        #region INestableView

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        // This is used only at runtime.
        // It must not appear in the WinForms generated code because it would conflict with the Parent property.
        // In Visual Studio 2015, it would cause the designer to crash.
        public virtual object ViewParent
        {
            get { return Parent; }
            set { Parent = (Control)value; }
        }

        #endregion

        #region IOptionUpdate

        public virtual void UpdateOption(OptionUpdateArgs args)
        {
            ViewBinder.UpdateOption(args);
        }

        #endregion

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

        /// <summary>
        /// Find a nested view or a parent control to receive a nessted view, within this View.
        /// </summary>
        /// <param name="nestedViewId"></param>
        /// <param name="viewParent"></param>
        /// <returns></returns>
        /// <seealso cref="INestedView"/>
        public virtual IView GetNestedView(string nestedViewId, out object viewParent)
        {
            foreach(var control in Controls)
            {
                if(control is INestedView)
                {
                    if(((INestedView)control).ViewId == nestedViewId)
                    {
                        if (control is IEmbeddedView)
                        {
                            viewParent = null;
                            return (IView)control;
                        }
                        else
                        {
                            viewParent = control;
                            return null;
                        }
                    }
                }
            }

            // not found:
            viewParent = null;
            return null;
        }

    }

}
