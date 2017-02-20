using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MvpFramework.Binding;

namespace MvpFramework
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
        void IView.Refresh()
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
                InvokeViewClosing(new ViewClosingEventArgs(true));
        }

        /// <summary>
        /// Fires the <see cref="ViewClosing"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void InvokeViewClosing(ViewClosingEventArgs args)
        {
            ViewClosing?.Invoke(this, args);
        }

        /// <summary>
        /// Fired when the View is closing.
        /// </summary>
        public virtual event ViewClosingDelegate ViewClosing;
    }

    /// <summary>
    /// Event fired when a closes, or before it closes, allowing handlers to prevent it from closing.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void ViewClosingDelegate(object sender, ViewClosingEventArgs args);

    /// <summary>
    /// Arguments to the <see cref="ViewClosingDelegate"/> event.
    /// </summary>
    public class ViewClosingEventArgs
    {
        public ViewClosingEventArgs(bool closed)
        {
            this.Closed = closed;
        }

        /// <summary>
        /// Iff true, the form is closed.
        /// </summary>
        public virtual bool Closed { get; protected set; }

        /// <summary>
        /// Set to true to prevent the view from closing.
        /// </summary>
        public virtual bool Intercept { get; set; }
    }
}
