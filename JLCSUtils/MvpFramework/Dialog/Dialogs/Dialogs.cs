using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MvpFramework.Dialog;
using MvpFramework.Menu;
using JohnLambe.Util.Validation;

namespace MvpFramework.Dialog.Dialogs
{
    // Model classes for dialogs:

    /// <summary>
    /// Dialog model for <see cref="InformationalDialogType"/>
    /// </summary>
    public class InformationDialog : MessageDialogModel<string>
    {
        public InformationDialog()
        {
            MessageType = MessageDialogType.Informational;
        }
    }

    /// <summary>
    /// Dialog model for <see cref="ErrorDialogType"/>
    /// </summary>
    public class ErrorDialog : MessageDialogModel<string>
    {
        public ErrorDialog()
        {
            MessageType = MessageDialogType.Error;
        }

        /// <summary>
        /// true iff this causes a task/process/application to exit.
        /// </summary>
        public virtual bool IsFatal { get; protected set; }

        /// <summary>
        /// Create a dialog model to represent the given exception.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="fatal">True iff this is a fatal error that will cause the application to exit.
        /// Equivalent to <see cref="UnhandledExceptionEventArgs.IsTerminating"/>.
        /// </param>
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

    /// <summary>
    /// Dialog model for <see cref="ConfirmationDialogType"/>
    /// </summary>
    public class ConfirmationDialog : MessageDialogModel<string>
    {
        public ConfirmationDialog()
        {
            MessageType = MessageDialogType.Confirmation;
        }
    }

    /// <summary>
    /// Dialog model for <see cref="WarningDialogType"/>
    /// </summary>
    public class WarningDialog : MessageDialogModel<string>
    {
        public WarningDialog()
        {
            MessageType = MessageDialogType.Warning;
        }
    }

    /// <summary>
    /// Dialog model for <see cref="SevereWarningDialogType"/>.
    /// </summary>
    public class SevereWarningDialog : MessageDialogModel<string>
    {
        public SevereWarningDialog()
        {
            MessageType = MessageDialogType.SevereWarning;
        }
    }

    /// <summary>
    /// Dialog model for <see cref="UserErrorDialogType"/>.
    /// </summary>
    public class UserErrorDialog : MessageDialogModel<string>
    {
        public UserErrorDialog()
        {
            MessageType = MessageDialogType.UserError;
        }

        /// <summary>
        /// Create a message dialog model for the given validation results.
        /// </summary>
        /// <param name="validationResults"></param>
        /// <returns>Message dialog model. May be a subclass of this.</returns>
        public static UserErrorDialog CreateDialogModelForValidationResult(ValidationResults validationResults)
        {
            return new UserErrorDialog()
            {
                Validation = validationResults,
                Message = validationResults.ToString()
            };
        }

        /// <summary>
        /// Create a message dialog model for the given validation result.
        /// </summary>
        /// <param name="validationResults"></param>
        /// <returns>Message dialog model. May be a subclass of this.</returns>
        public static UserErrorDialog CreateDialogModelForValidationResult(ValidationResultEx validationResults)
        {
            return new UserErrorDialog()
            {
                Validation = new ValidationResults(validationResults)
            };
        }

        public virtual ValidationResults Validation { get; protected set; }
    }

    /// <summary>
    /// Dialog model for <see cref="SystemErrorDialogType"/>.
    /// </summary>
    public class SystemErrorDialog : MessageDialogModel<string>
    {
        public SystemErrorDialog()
        {
            MessageType = MessageDialogType.SystemError;
        }
    }

    /// <summary>
    /// Dialog model for <see cref="InternalErrorDialogType"/>.
    /// </summary>
    public class InternalErrorDialog : MessageDialogModel<string>
    {
        public InternalErrorDialog()
        {
            MessageType = MessageDialogType.InternalError;
        }
    }
}
