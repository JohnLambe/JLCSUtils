using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Dialog
{
    /// <summary>
    /// View for a message dialog.
    /// </summary>
    public interface IMessageDialogView : IWindowView
    {
    }

    /// <summary>
    /// Presenter for showing message dialogs.
    /// </summary>
    public interface IMessageDialogPresenter : IWindowPresenter
    {
        /// <summary>
        /// Show a message dialog.
        /// </summary>
        /// <param name="messageDialog">Details of the dialog message.</param>
        /// <returns>The response to the dialog (option chosen by the user), if applicable.</returns>
        object ShowDialog(IMessageDialogModel messageDialog);
    }
}
