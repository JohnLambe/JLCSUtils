using JohnLambe.Util.Reflection;
using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MvpFramework.Menu;
using JohnLambe.Util.Text;
using JohnLambe.Util.Misc;

namespace MvpFramework.Dialog
{

    /// <summary>
    /// Model-level parameters for a message dialog (or any UI item that displays a message).
    /// <para>
    /// Icon IDs (provided by some properties) are resolved by the Message Dialog Service, typically by delegating to an Icon Repository.
    /// A value of "" explicitly specifies that there should be no icon, even if there would be a default one if null was supplied.
    /// </para>
    /// </summary>
    public class MessageDialogModel<TResult> : IMessageDialogModel<TResult>
    {
        public MessageDialogModel()
        {
            MessageTypeId = GetType().Name;
        }

        /// <summary>
        /// ID unique to an message/warning/error type and a place where it can occur.
        /// May be null.
        /// <para>A hierarchical ID (parts separated by "/") is recommended.</para>
        /// <para>This can be used in analysing log files for occurrences of the same error condition, etc.,
        /// or potentially for applying UI styles, changing the default button,
        /// or hiding the dialog and returning as if the default option had been chosen.</para>
        /// </summary>
        public virtual string InstanceId { get; set; }

        /// <summary>
        /// Title of the message dialog. (For display).
        /// May be null. (If it is, it is recommended that the UI hide the title bar of the window etc.).
        /// </summary>
        [ViewTitle]
        public virtual string Title { get; set; }

        /// <summary>
        /// Main message for display.
        /// </summary>
        public virtual string Message { get; set; }

        public virtual MessageType MessageType { get; set; } = MessageType.None;
        //| Inferred from MessageTypeId ?

        /// <summary>
        /// Hierarchical ID of the type of message (parts separated by "/").
        /// Could have UI styles or defaults for other properties mapped to it.
        /// May be null.
        /// </summary>
        public virtual string MessageTypeId { get; set; }

        /// <summary>
        /// Identifier of an icon indicating the type of message or error.
        /// May be null, for a default determined by <see cref="MessageType"/> (possibly no icon).
        /// </summary>
        [IconId]
        public virtual string Icon { get; set; }

        /// <summary>
        /// Identifier of an image related to the message (rather than the type (error, warning, etc.)).
        /// (It might be displayed in the background of the dialog.)
        /// </summary>
        [IconId]
        public virtual string MessageImage { get; set; }

        /// <summary>
        /// What type of UI to use to show the message.
        /// </summary>
        public virtual MessageDisplayType DisplayType { get; set; } = MessageDisplayType.Default;

        /// <summary>
        /// The exception which lead to this message.
        /// May be null.
        /// </summary>
        public virtual Exception Exception { get; set; }

        /// <summary>
        /// Low-level details (not shown to the user, except possibly by their requesting it by a UI interaction
        /// (expanding a panel, clicking a 'Detail' button, etc.)).
        /// May be null (in which case, it is recommended that any UI control for showing it should be hidden).
        /// </summary>
        public virtual string DiagnosticMessage { get; set; }

        /// <summary>
        /// The buttons or options that the user can choose, in the order in which they are displayed.
        /// null for default based on MessageType.
        /// </summary>
        public virtual IOptionCollection Options { get; set; }

        /// <summary>
        /// Flags relating to what interceptors can do with this message.
        /// </summary>
        public virtual MessageDialogInterceptFlags InterceptFlags => MessageDialogInterceptFlags.Default;

        /// <summary>
        /// Event that is fired when an option is chosen.
        /// (For use with modeless dialogs, but fired whether modal or not).
        /// </summary>
        public virtual event MessageDialogRespondedDelegate Responded;

        /// <summary>
        /// Fire the <see cref="Responded"/> event.
        /// </summary>
        /// <param name="messageDialogResult">The 'messageDialogResult' parameter to <see cref="RespondedDelegate"/>.</param>
        public virtual void FireResponded(object messageDialogResult)
        {
            Responded?.Invoke(this, new MessageDialogRespondedEventArgs(messageDialogResult));
        }
    }


    /// <summary>
    /// Category of message.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// No type specified. Typically displayed with no icon etc. indicating the type of message.
        /// </summary>
        None  = 0,

        /// <summary>
        /// Information that is not an error or warning.
        /// By default has only one option (typically called "Ok") - to dismiss it.
        /// </summary>
        Informational = 1000,

        /// <summary>
        /// Asks the user to confirm an action.
        /// Default options are "Yes", "No" and "Cancel".
        /// </summary>
        Confirmation = 2000,

        // An error with a 'Retry' option:
        // Could be a separate type, but it could be more useful to be able to classify the category of error
        // (one of the values below), and specify the buttons separately.

        /// <summary>
        /// A warning.
        /// By default, has only one option (typically called "Ok") - to dismiss it.
        /// </summary>
        Warning = 3000,

        /// <summary>
        /// A warning that is more severe than the <see cref="Warning"/> option.
        /// This may have a different UI style to draw attention to it, and/or different UI behaviours
        /// (e.g. a delay of 1-2 seconds before allowing the user to respond, so that a user automatically pressing a key to dismiss when they couldn't possibly have read it won't be accepted).
        /// By default has only one option (typically called "Ok") - to dismiss it.
        /// </summary>
        SevereWarning = 3500,

        /// <summary>
        /// An error.
        /// By default, has only one option (typically called "Ok") - to dismiss it (the same applies to all type with names ending with "Error").
        /// </summary>
        Error = 4000,

        /// <summary>
        /// An error caused by a user, such as user input that failed validation.
        /// </summary>
        UserError = 4200,

        /// <summary>
        /// An system error, e.g. disc full, network connection failed.
        /// </summary>
        SystemError = 4600,

        /// <summary>
        /// An error that should never happen. An internal failure, such as an assertion failure.
        /// </summary>
        InternalError = 4800
    }


    /// <summary>
    /// Specifies how (what type of dialog etc.) a message is displayed.
    /// </summary>
    public enum MessageDisplayType
    {
        /// <summary>
        /// To use a default based on other properties of the <see cref="MessageDialogModel"/>.
        /// </summary>
        Default = 0,
        //| Could use Nullable<MessageDisplayType> instead ?

        /// <summary>
        /// Don't display. It may be logged.
        /// </summary>
        None,

        /// <summary>
        /// A message appears modelessly and dissappears after a certain time if the user does not explicity dismiss it.
        /// (Like an Android "Toast" message).
        /// </summary>
        Temporary,

        /// <summary>
        /// Show a modeless dialog or message, typically on the bottom or corner of a page/screen/window etc.
        /// </summary>
        NonModal,

        /// <summary>
        /// Show a modal dialog.
        /// </summary>
        Modal,
    }

}
