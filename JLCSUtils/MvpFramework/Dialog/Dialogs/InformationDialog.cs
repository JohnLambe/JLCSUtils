using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MvpFramework.Dialog;

namespace MvpFramework.Dialog.Dialogs
{
    public class InformationDialog : MessageDialogModel<bool>
    {

    }

    public class ErrorDialog : MessageDialogModel<bool>
    {
        public ErrorDialog()
        {
            MessageType = MessageType.Error;
        }

    }

}
