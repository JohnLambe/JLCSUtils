using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Dialog
{
    public interface IMessageDialogView : IWindowView
    {
    }

    public interface IMessageDialogPresenter : IWindowPresenter
    {
        object ShowModal(IMessageDialogModel messageDialog);
    }
}
