using MvpFramework.Dialog;
using MvpFramework.Dialog.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms
{
    /// <summary>
    /// Application-wide exception handler.
    /// </summary>
    public class ApplicationExceptionHandler : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageDialogService">Service for showing dialog to report exceptions.</param>
        public ApplicationExceptionHandler(IMessageDialogService messageDialogService)
        {
            this.MessageDialogService = messageDialogService;
            Enabled = true;
        }

        public virtual void Dispose()
        {
            Enabled = false;
        }

        protected readonly IMessageDialogService MessageDialogService;

        /// <summary>
        /// True iff exceptions are being handled by this instance.
        /// </summary>
        public virtual bool Enabled
        {
            get { return _enabled; }
            set
            {
                if(value != _enabled)
                {
                    if (value)
                    {
                        Application.ThreadException += Application_ThreadException;   // handler for UI thread exceptions

                        //                        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

                        //            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;  // handler for non-UI thread exceptions
                    }
                    else
                    {
                        Application.ThreadException -= Application_ThreadException;
                        //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

                        //            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                    }
                }
            }
        }
        private bool _enabled;

        protected virtual void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if(exception != null)
                MessageDialogService.ShowMessage(ErrorDialog.CreateDialogModelForException(exception,e.IsTerminating));
            //            MessageBox.Show(e.ExceptionObject.ToString() + "\n" + sender.ToString());

        }

        protected virtual void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageDialogService.ShowMessage(ErrorDialog.CreateDialogModelForException(e.Exception));
//            MessageBox.Show(e.Exception.Message + "\n" + sender.ToString());
        }


    }
}
