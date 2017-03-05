﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms
{
    public partial class WindowViewBase : ViewBase, IWindowView
    {
        public WindowViewBase()
        {
            InitializeComponent();

            base.VisibleChanged += View_VisibleChanged;
        }

        public virtual void Close()
        {
            //TODO
        }

        public virtual object ShowModal()
        {
            return null;
        }

        void IWindowView.Show()
        {
            Opening = true;
            base.Show();
            Opening = false;
            InvokeViewVisibilityChanged(new ViewVisibilityChangedEventArgs(VisibilityChange.Opened));
        }

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
            ViewVisibilityChanged?.Invoke(this, args);
        }

        [Browsable(true)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
        
        public virtual string Title
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
        

        /// <summary>
        /// Fired when the View is Opened or closing.
        /// </summary>
        public virtual event ViewVisibilityChangedDelegate ViewVisibilityChanged;

    }
}
