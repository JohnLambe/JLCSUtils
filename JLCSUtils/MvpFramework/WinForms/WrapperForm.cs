using MvpFramework;
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
            VisibleChanged += WrapperForm_VisibleChanged;

            if (Child is IWindowView)
            {
                ((IWindowView)Child).ViewVisibilityChanged += WrapperForm_ViewVisibilityChanged;
                //TODO modal
                // if(e.Modal)
                //   ShowDialog();
                //                ((IView)Child).ViewClosing += WrapperForm_ViewClosing;
            }
        }

        private void WrapperForm_VisibleChanged(object sender, EventArgs e)
        {
            
        }

        /*
        private void WrapperForm_SizeChanged(object sender, EventArgs e)
        {

        }
        */

        protected virtual void Child_SizeChanged(object sender, EventArgs e)
        {
            // When the child's size changes:
            // Synchronise the minimum and maximum sizes (they could have changed since we last checked):
            MaximumSize = SizeFromClientSize(Child.MaximumSize);
            MinimumSize = SizeFromClientSize(Child.MinimumSize);

            // Synchronise the size of this form with the child:
            ClientSize = Child.Size;
        }

        protected virtual void WrapperForm_ViewVisibilityChanged(object sender, ViewVisibilityChangedEventArgs args)
        {
            if(args.Action == VisibilityChange.Opened)
            {
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

        protected Control Child { get; private set; }
    }
}
