using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MvpFramework.Dialog;
using MvpFramework.Menu;

namespace MvpFramework.Dialog.Dialogs
{
    public class InformationDialog : MessageDialogModel<string>
    {
        public InformationDialog()
        {
            base.MessageType = MessageDialogType.Informational;
        }
    }

    public class ErrorDialog : MessageDialogModel<string>
    {
        public ErrorDialog()
        {
            base.MessageType = MessageDialogType.Error;
        }

        /// <summary>
        /// Create a dialog model to represent the given exception.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="fatal">True iff this is a fatal error that will cause the application to exit.
        /// Equivalent to .</param>
        /// <returns></returns>
        public static ErrorDialog CreateDialogModelForException(Exception ex, bool fatal = false)
        {
            if (ex == null)
                return null;

            return new ErrorDialog()
            {
                Exception = ex
                //TODO: Choose subclass by exception type
            };
        }
    }

    public class ConfirmationDialog : MessageDialogModel<string>
    {
        public ConfirmationDialog()
        {
            base.MessageType = MessageDialogType.Confirmation;
        }
    }

    public class WarningDialog : MessageDialogModel<string>
    {
        public WarningDialog()
        {
            base.MessageType = MessageDialogType.Warning;
        }
    }

    public class SevereWarningDialog : MessageDialogModel<string>
    {
        public SevereWarningDialog()
        {
            base.MessageType = MessageDialogType.SevereWarning;
        }
    }

    public class UserErrorDialog : MessageDialogModel<string>
    {
        public UserErrorDialog()
        {
            base.MessageType = MessageDialogType.UserError;
        }
    }

    public class SystemErrorDialog : MessageDialogModel<string>
    {
        public SystemErrorDialog()
        {
            base.MessageType = MessageDialogType.SystemError;
        }
    }

    public class InternalErrorDialog : MessageDialogModel<string>
    {
        public InternalErrorDialog()
        {
            base.MessageType = MessageDialogType.InternalError;
        }
    }
}
