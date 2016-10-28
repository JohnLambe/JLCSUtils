﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms
{
    public static class MvpWindowStateExt
    {
        public static MvpWindowState FromWinForms(FormWindowState state)
        {
            return (MvpWindowState)state;
        }

        public static FormWindowState ToWinForms(this MvpWindowState state)
        {
            return (FormWindowState)state;
        }
    }


    /// <summary>
    /// Basic IUiController implementation for WinForms.
    /// </summary>
    public class UiController : IUiController
    {
        public virtual MvpWindowState MainFormState
        {
            get
            {
                return MvpWindowStateExt.FromWinForms(MainForm.WindowState);
            }
            set
            {
                MainForm.WindowState = value.ToWinForms();
            }
        }

        public virtual bool BringToFront()
        {
            MainForm?.BringToFront();
            return true;
        }

        public virtual void ShowMessage(string message, string title = "")
        {
            MessageBox.Show(message, title);
        }

        public virtual object ShowMessage(MessageDialogParameters parameters)
        {
            // Mock/placeholder implementation:
            MessageBox.Show(parameters.Message, parameters.Title);
            //TOOD: support the other properties, with a better dialog box.
            return null;
        }

        protected virtual Form MainForm { get { return Application.OpenForms[0]; } }
    }
}
