using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvpFramework.Dialog;

namespace MvpFramework.WinForms
{
    /// <summary>
    /// Base class for views that can open as a window.
    /// </summary>
    public partial class WindowViewBase : ViewBase, IWindowView
    {
        public WindowViewBase() : this((IMessageDialogService)null)
        {
        }
        public WindowViewBase(IMessageDialogService dialogService) : base(dialogService)
        {
            Init();
        }
        public WindowViewBase(IMvpFrameworkDetails mvpFramework) : base(mvpFramework)
        {
            Init();
        }
        private void Init()
        {
            InitializeComponent();

            base.VisibleChanged += View_VisibleChanged;
        }

        public virtual void Close()
        {
            var args = new ViewVisibilityChangedEventArgs(VisibilityChange.Closing);
            InvokeViewVisibilityChanged(args);
            if (args.Intercept)
            {
                //TODO
            }
            else
            {
                InvokeViewVisibilityChanged(new ViewVisibilityChangedEventArgs(VisibilityChange.Closed));
            }
        }

        public virtual object ShowModal()
        {
            Modal = true;
            DoShow();
            return ModalResult;
        }

        void IWindowView.Show()
        {
            Modal = false;
            DoShow();
        }

        protected virtual void DoShow()
        {
            Opening = true;      // for event that fires in base class
            try
            {
                base.Show();
            }
            finally
            {
                Opening = false;
            }
            InvokeViewVisibilityChanged(new ViewVisibilityChangedEventArgs(VisibilityChange.Opened) { IsModal = this.Modal });
        }

        /// <summary>
        /// true while opening (in the <see cref="IPresenter.Show"/> method).
        /// </summary>
        protected virtual bool Opening { get; private set; }

        protected virtual void View_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible && !Opening)
                InvokeViewVisibilityChanged(new ViewVisibilityChangedEventArgs(VisibilityChange.Closed));
        }

        /// <summary>
        /// Fires the <see cref="ViewVisibilityChanged"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void InvokeViewVisibilityChanged(ViewVisibilityChangedEventArgs args)
        {
            args.IsModal = Modal;
            ViewVisibilityChanged?.Invoke(this, args);
        }

        [Browsable(true)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] ?
        public virtual string Title
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /*
        protected virtual bool CanClose()
        {
//            ViewVisibilityChanged?.
        }
        */

        // To be synchronized with wrapper form:
        [DefaultValue(true)]
        public virtual bool ControlBox { get; set; } = true;
        //TODO: Other Form properties

        /// <summary>
        /// Fired when the View is Opened or closing.
        /// </summary>
        public virtual event ViewVisibilityChangedDelegate ViewVisibilityChanged;

        /// <summary>
        /// true iff the view was shown modally.
        /// If the view is closed, this indicates the state of the last time it was shown.
        /// If it was never shown, this is false.
        /// </summary>
        //| Named for consistency with System.Windows.Forms.Form.Modal.
        [DefaultValue(false)]
        public virtual bool Modal { get; private set; }

        /// <summary>
        /// The value to be returned to a modal caller.
        /// </summary>
        protected virtual object ModalResult { get; set; }
    }
}
