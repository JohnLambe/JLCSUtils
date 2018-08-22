using MvpFramework;
using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms
{
    /// <summary>
    /// Window that wraps a form and synchronises certain properties and events.
    /// </summary>
    public partial class WrapperForm : Form
    {
        public WrapperForm(Control child)
        {
            InitializeComponent();

            Setup(child);
        }

        private void WrapperForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
        }

        protected virtual void Setup(Control child)
        {
            Child = child;
            ClientSize = child.Size;
            Text = child.Text;

            child.Dock = DockStyle.Fill;
            child.Parent = this;

            child.VisibleChanged += Child_VisibleChanged;
            child.TextChanged += Child_TextChanged;
            child.SizeChanged += Child_SizeChanged;
            child.Disposed += Child_Disposed;

//            SizeChanged += WrapperForm_SizeChanged;
//            VisibleChanged += WrapperForm_VisibleChanged;

            if (Child is IWindowView)
            {
                ((IWindowView)Child).ViewVisibilityChanged += WrapperForm_ViewVisibilityChanged;
                //TODO modal
                // if(e.Modal)
                //   ShowDialog();
                //                ((IView)Child).ViewClosing += WrapperForm_ViewClosing;
            }

            if(Child is IChildWindow)
            {
                ((IChildWindow)Child).WindowOptions.Changed += WindowOptions_Changed;
                ((IChildWindow)Child).WindowOptions.Apply(this);
            }
        }

        /// <summary>
        /// Fired when a property of the <see cref="IChildWindow.WindowOptions"/> of the child changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void WindowOptions_Changed(object sender, EventArgs e)
        {
            ((IChildWindow)Child).WindowOptions.Apply(this);
        }

        /*
        private void WrapperForm_VisibleChanged(object sender, EventArgs e)
        {
            
        }

        private void WrapperForm_SizeChanged(object sender, EventArgs e)
        {

        }
        */

        /// <summary>
        /// Handles the size of the wrapped control changing.
        /// This may be as a result of this form changing size, or the size of the wrapped contol
        /// being assigned directly. Only the latter case actually requires handling, to adjust the
        /// window size to match the new child size.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Child_SizeChanged(object sender, EventArgs e)
        {
            // When the child's size changes:
            // Synchronise the minimum and maximum sizes (they could have changed since we last checked):
            MaximumSize = SizeFromChildSize(Child.MaximumSize);
            MinimumSize = SizeFromChildSize(Child.MinimumSize);

            // Synchronise the size of this form with the child:
            ClientSize = Child.Size;
        }

        /// <summary>
        /// Calculate the size of this form from the size of the wrapped control.
        /// </summary>
        /// <param name="childSize">The size of the wrapped control.</param>
        protected virtual Size SizeFromChildSize(Size childSize)
        {
            if (childSize.IsEmpty)
                return childSize;
            else
                return SizeFromClientSize(childSize);
        }

        protected virtual void WrapperForm_ViewVisibilityChanged(object sender, ViewVisibilityChangedEventArgs args)
        {
            if(args.Action == VisibilityChange.Opened)
            {
                if (args.IsModal)
                    ShowDialog();
                else
                    Visible = true;
            }
            else if (args.Action == VisibilityChange.Closed)
            {
                Visible = false;
            }
        }

        protected virtual void Child_Disposed(object sender, EventArgs e)
        {
            Dispose();
        }

        protected virtual void Child_TextChanged(object sender, EventArgs e)
        {
            Text = Child.Text;
        }

        protected virtual void Child_VisibleChanged(object sender, EventArgs e)
        {
            Visible = Child.Visible;
        }

        protected override void OnKeyDown(KeyEventArgs evt)
        {
            if(Child is IChildWindow)
            {
                if(((IChildWindow)Child).KeyPreview)
                {
                    ((IChildWindow)Child).NotifyKeyDown(evt);
                }
            }
            if(!evt.Handled)
                base.OnKeyDown(evt);
        }

        /*
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {            
            base.OnPreviewKeyDown(e);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            return base.ProcessDialogKey(keyData);
        }
        */

        /*
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if(Child is IView)
            {
                ((IView)Child).
            }
        }
        */

        protected Control Child { get; private set; }
    }


    /// <summary>
    /// Optional interface for a window embedded in <see cref="WrapperForm"/> (or equivalent) to provide additional form-like features.
    /// </summary>
    public interface IChildWindow
    {
        /// <summary>
        /// true to have <see cref="NotifyKeyDown"/> called for key events on the form.
        /// </summary>
        bool KeyPreview { get; }

        void NotifyKeyDown(KeyEventArgs args);

        WindowOptions WindowOptions { get; }
    }


    public static class WindowOptionsExt
    {
        /// <summary>
        /// Apply these options to the given form.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="window"></param>
        public static void Apply(this WindowOptions options, Form window)
        {
            if(options.Opacity.HasValue)
                window.Opacity = Math.Round((double)options.Opacity.Value * 100);

            window.ShowInTaskbar = options.ShowInTaskBar ?? window.ShowInTaskbar;
            window.MinimizeBox = options.MinimizeButton ?? window.MinimizeBox;
            window.MaximizeBox = options.MinimizeButton ?? window.MaximizeBox;

            window.ControlBox = options.CloseButton ?? window.ControlBox;
            //TODO: Handle CloseBox==true with MinimizeBox or MaximizeBox false (disable only the close box)

            window.HelpButton = options.HelpButton ?? window.HelpButton;

            window.TopMost = options.StayOnTop ?? window.TopMost;
        }
    }

}
