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
            MessageTypeId = GetType().FullName;
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

        public virtual MessageDialogType MessageType { get; set; } = MessageDialogType.Informational;
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
        /// The minimum log detail level that at which this message should be logged.
        /// </summary>
        public virtual int LogLevel { get; set; }


        /// <summary>
        /// Flags relating to what interceptors can do with this message.
        /// </summary>
        public virtual MessageDialogInterceptFlags InterceptFlags => MessageDialogInterceptFlags.Default;


        /// <summary>
        /// A human-readable desription of this type of message.
        /// Can be used for showing configuration settings relating to it, etc.
        /// </summary>
        public virtual string Description { get; set; }

        
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
    /// Specifies how (what type of dialog etc.) a message is displayed.
    /// </summary>
    public class MessageDisplayType
    {
        /// <summary>
        /// To use a default based on other properties of the <see cref="MessageDialogModel"/>.
        /// </summary>
        public static readonly MessageDisplayType Default = new MessageDisplayType();
        //| Could use Nullable<MessageDisplayType> instead ?

        /// <summary>
        /// Don't display. It may be logged.
        /// </summary>
        public static readonly MessageDisplayType None = new MessageDisplayType();

        /// <summary>
        /// A message appears modelessly and dissappears after a certain time if the user does not explicity dismiss it.
        /// (Like an Android "Toast" message).
        /// </summary>
        public static readonly MessageDisplayType Temporary = new MessageDisplayType();

        /// <summary>
        /// Show a modeless dialog or message, typically on the bottom or corner of a page/screen/window etc.
        /// </summary>
        public static readonly MessageDisplayType NonModal = new MessageDisplayType();

        /// <summary>
        /// Show a modal dialog.
        /// </summary>
        public static readonly MessageDisplayType Modal = new MessageDisplayType();
    }


    /// <summary>
    /// Regsiter a message dialog type to be available for configuration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RegisterMessageDialogAttribute : MvpAttribute
    {
        public virtual string[] Filter { get; set; }

        public virtual string Category { get; set; }
    }

}
