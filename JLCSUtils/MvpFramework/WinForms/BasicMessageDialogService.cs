using MvpFramework.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MvpFramework.WinForms
{
    /// <summary>
    /// A basic implementation of <see cref="IMessageDialogService"/>.
    /// </summary>
    public class BasicMessageDialogService : IMessageDialogService
    {
        public virtual TResult ShowMessage<TResult>(IMessageDialogModel<TResult> messageModel)
        {
            // Mock/placeholder implementation:
            MessageBox.Show(messageModel.Message ?? "", messageModel.Title);
            //TOOD: support the other properties, with a better dialog box.
            return default(TResult);
        }
    }
}
