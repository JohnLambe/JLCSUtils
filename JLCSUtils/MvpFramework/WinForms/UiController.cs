using MvpFramework.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MvpFramework.WinForms
{
    /// <summary>
    /// Basic <see cref="IUiController"/> implementation for WinForms.
    /// </summary>
    public class UiController : IUiController
    {
        public UiController(IMessageDialogService messageDialogService = null)
        {
            MessageDialogService = messageDialogService ?? new BasicMessageDialogService();
        }

        /// <inheritdoc cref="IUiController.MainFormState" />
        public virtual MvpWindowState MainFormState
        {
            get
            {
                return MainForm.WindowState.ToMvpWindowState();
            }
            set
            {
                MainForm.WindowState = value.ToWinForms();
            }
        }

        /// <inheritdoc cref="IUiController.BringToFront" />
        public virtual bool BringToFront()
        {
            MainForm?.BringToFront();
            return true;
        }

        /// <inheritdoc cref="IUiController.ShowMessage" />
        public virtual void ShowMessage(string message, string title = "")
        {
            ShowMessage<object>(new MessageDialogModel<object>()
            {
                Message = message,
                Title = title
            });
        }

        public virtual TResult ShowMessage<TResult>(IMessageDialogModel<TResult> messageModel)
        {
            return MessageDialogService.ShowMessage<TResult>(messageModel);
/*
            // Mock/placeholder implementation:
            MessageBox.Show(messageModel.Message, messageModel.Title);
            //TOOD: support the other properties, with a better dialog box.
            return default(TResult);
*/
        }

        protected virtual Form MainForm { get { return Application.OpenForms[0]; } }
        protected readonly IMessageDialogService MessageDialogService;
    }
}
