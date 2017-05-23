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
    /// Basic IUiController implementation for WinForms.
    /// </summary>
    public class UiController : IUiController
    {
        public UiController(IMessageDialogService messageDialogService = null)
        {
            MessageDialogService = messageDialogService ?? new BasicMessageDialogService();
        }

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

        public virtual bool BringToFront()
        {
            MainForm?.BringToFront();
            return true;
        }

        public virtual void ShowMessage(string message, string title = "")
        {
//            MessageBox.Show(message, title);
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
