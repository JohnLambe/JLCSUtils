using MvpFramework.Dialog;
using MvpFramework.Dialog.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JohnLambe.Util.Exceptions;
using JohnLambe.Util.Types;

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
        /// <param name="map">Provides a mapping from exceptions to dialogs. null for default.</param>
        public ApplicationExceptionHandler(IMessageDialogService messageDialogService, [Nullable] ExceptionToDialogMap map = null)
        {
            this.MessageDialogService = messageDialogService;

            if (map != null)
            {
                _map = map;
            }
            else
            {
                _map = new ExceptionToDialogMap();
                _map.ScanAssemblies(typeof(ErrorDialog).Assembly);
            }

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
                    _enabled = value;
                }
            }
        }
        private bool _enabled;

        protected virtual void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if(exception != null)
                ShowDialog(exception,e.IsTerminating);
            //            MessageBox.Show(e.ExceptionObject.ToString() + "\n" + sender.ToString());
        }

        protected virtual void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ShowDialog(e.Exception);
//            MessageBox.Show(e.Exception.Message + "\n" + sender.ToString());
        }

        /// <summary>
        /// Show a dialog reporting the given exception.
        /// </summary>
        /// <param name="ex">The exception to report.</param>
        /// <param name="isTerminating">True iff the CLR is terminating.</param>
        protected virtual void ShowDialog(Exception ex, bool isTerminating = false)
        {
            MessageDialogService.ShowMessage(_map.GetMessageDialogModelForException(ex));
//                ErrorDialog.CreateDialogModelForException(ExtractException(ex))
        }

        protected ExceptionToDialogMap _map = new ExceptionToDialogMap();

        protected virtual Exception ExtractException(Exception ex)
            => ExceptionUtil.ExtractException(ex);
    }
}
