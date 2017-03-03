using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MvpFramework.Binding;

namespace MvpFramework.WinForms
{
    /// <summary>
    /// Optional base class for Views.
    /// </summary>
    public class ViewBase : UserControl, IView
    {
        public ViewBase()
        {
            base.VisibleChanged += View_VisibleChanged;
        }

        /// <summary>
        /// Bind the model and presenter to this view.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="presenter"></param>
        /// <param name="binderFactory"></param>
        public virtual void Bind(object model, IPresenter presenter, IControlBinderFactory binderFactory)
        {
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
        }

        /// <summary>
        /// Collection of binders for the controls in this view.
        /// </summary>
        protected virtual IList<IControlBinder> Binders { get; private set; }

        /// <summary>
        /// (Re)populate the view from the model (to update it when the model changes).
        /// </summary>
        public virtual void RefreshView()
        {
            if (Binders != null)
            {
                foreach (var binder in Binders)
                {
                    binder.Refresh();
                }
            }
        }

        void IView.Show()
        {
            base.Show();
            InvokeViewVisibilityChanged(new ViewVisibilityChangedEventArgs(VisibilityChange.Opened));
        }

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

        protected void View_VisibleChanged(object sender, EventArgs e)
        {
            if(!Visible)
                InvokeViewVisibilityChanged(new ViewVisibilityChangedEventArgs(VisibilityChange.Closed));
        }

        /// <summary>
        /// Fires the <see cref="ViewVisibilityChanged"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void InvokeViewVisibilityChanged(ViewVisibilityChangedEventArgs args)
        {
            ViewVisibilityChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Fired when the View is closing.
        /// </summary>
        public virtual event ViewVisibilityChangedDelegate ViewVisibilityChanged;
    }

}
