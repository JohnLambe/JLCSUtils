using JohnLambe.Util.Misc;
using JohnLambe.Util.Types;
using MvpFramework.Menu;
using System;

namespace MvpFramework.Dialog
{
    /// <summary>
    /// Interface to a system for showing message dialogs etc.
    /// </summary>
    public interface IMessageDialogService
    {
        /// <summary>
        /// Show a message (dialog etc.).
        /// </summary>
        /// <param name="messageModel">Details of the message to be shown.</param>
        /// <returns>Indicates which option was chosen, when the message has multiple options (e.g. buttons).</returns>
        /// <typeparam name="TResult">The type of the result - depends on the message.</typeparam>
        TResult ShowMessage<TResult>(IMessageDialogModel<TResult> messageModel);
    }


    /// <summary>
    /// Model of a message dialog.
    /// </summary>
    public interface IMessageDialogModel
    {
        /// <summary>
        /// ID unique to an message/warning/error type and a place where it can occur.
        /// <para>A hierarchical ID (parts separated by "/") is recommended.</para>
        /// <para>This can be used in analysing log files for occurrences of the same error condition, etc.,
        /// or potentially for applying UI styles, changing the default button,
        /// or suppressing the dialog and returning as if the default option had been chosen.</para>
        /// </summary>
        [Nullable]
        string InstanceId { get; }

        /// <summary>
        /// What type of UI to use to show the message.
        /// </summary>
        MessageDisplayType DisplayType { get; set; }

        /// <summary>
        /// Title of the message dialog. (For display).
        /// May be null. (If it is, it is recommended that the UI hide the title bar of the window etc.).
        /// </summary>
        [Nullable]
        string Title { get; set; }

        /// <summary>
        /// Main message for display (to a user).
        /// </summary>
        [Nullable]
        string Message { get; set; }

        /// <summary>
        /// Identifier of an icon indicating the type of message or error.
        /// May be null, for a default determined by <see cref="MessageType"/> (possibly no icon).
        /// </summary>
        [IconId]
        [Nullable]
        string Icon { get; set; }

        /// <summary>
        /// Identifier of an image related to the message (rather than the type (error, warning, etc.)).
        /// (It might be displayed in the background of the dialog.)
        /// </summary>
        [IconId]
        [Nullable]
        string MessageImage { get; set; }

        /// <summary>
        /// Type or category of message (see <see cref="MessageDialogType"/>).
        /// </summary>
        MessageDialogType MessageType { get; set; }
        /// <summary>
        /// Hierarchical ID of the type of message (parts separated by "/").
        /// Could have UI styles or defaults for other properties mapped to it.
        /// </summary>
        [Nullable]
        string MessageTypeId { get; set; }

        #region Dialog grouping

        /// <summary>
        /// Dialogs with the same <see cref="GroupId"/> can be combined into one (generally only for non-modal dialogs, but an implementation could support it for modal ones
        /// that can be updated asynchronously while visible).
        /// This is for when a series of related messages are shown to the user, and seeing them all in a list is preferable to getting a separate dialog for each one.
        /// (For example, consider an operation that applies to many items, some of which may fail, but the operation continues on individual failures. Having to acknowledge each one individually could be annoying to a user.)
        /// <para>
        /// The combined dialog will have certain fields taken from one <see cref="IMessageDialogModel"/> (implementations can have their 
        /// own way of combining them: they might take the first one, or make the type the most important/severe type of the combined items)
        /// and other fields that combine the corresponding fields of all <see cref="IMessageDialogModel"/>s (e.g. <see cref="Message"/> would usually be concatenated).
        /// </para>
        /// <para>
        /// Implementations can define their rules on how messages are combined. They could differ by <see cref="MessageTypeId"/>.
        /// </para>
        /// </summary>
        [Nullable]
        string GroupId { get; set; }

        /// <summary>
        /// Message (for users) that is combined with <see cref="Message"/>, and is considered common to all messages in a group.
        /// (This message is shown only once in a combined dialogs).
        /// </summary>
        [Nullable]
        string CommonMessage { get; set; }

        #endregion

        /// <summary>
        /// The exception which lead to this message.
        /// May be null.
        /// </summary>
        [Nullable]
        Exception Exception { get; set; }

        /// <summary>
        /// Message intended for developers or IT staff:
        /// Low-level details (not shown to the user, except possibly by their requesting it by a UI interaction
        /// (expanding a panel, clicking a 'Detail' button, etc.)).
        /// May be null (in which case, it is recommended that any UI control for showing it should be hidden).
        /// </summary>
        [Nullable]
        string DiagnosticMessage { get; set; }

        /// <summary>
        /// The buttons or options that the user can choose, in the order in which they are displayed.
        /// </summary>
        [Nullable("null for default based on MessageType.")]
        IOptionCollection Options { get; set; }

        /// <summary>
        /// Flags relating to what interceptors can do with this message.
        /// </summary>
        MessageDialogInterceptFlags InterceptFlags { get; }

        //TODO: Type InputType { get; set; }   // Type to be input in the dialog

        /// <summary>
        /// Event that is fired when an option is chosen (when the user responds to the dialog; e.g. the user clicks a button in it to acknowledge it, or confirm or cancel an action).
        /// (For use with modeless dialogs, but fired whether modal or not).
        /// </summary>
        event MessageDialogRespondedDelegate Responded;

        /// <summary>
        /// Fire the <see cref="Responded"/> event.
        /// </summary>
        /// <param name="messageDialogResult">The 'messageDialogResult' parameter to <see cref="MessageDialogRespondedDelegate"/>.</param>
        void FireResponded(object messageDialogResult);
    }


    /// <summary>
    /// <inheritdoc cref="IMessageDialogModel"/>
    /// </summary>
    /// <typeparam name="TResult">The type to be returned from the <see cref="IMessageDialogService.ShowMessage{TResult}(IMessageDialogModel{TResult})"/> method.</typeparam>
    public interface IMessageDialogModel<TResult> : IMessageDialogModel
    {
    }

    /// <summary>
    /// Delegate to be fired when the user responds to a message.
    /// </summary>
    /// <param name="dialog">The <see cref="IMessageDialogModel"/> to which the response relates.</param>
    /// <param name="args"></param>
    public delegate void MessageDialogRespondedDelegate(IMessageDialogModel dialog, MessageDialogRespondedEventArgs args);

    /// <summary>
    /// Arguments for <see cref="MessageDialogRespondedDelegate"/>.
    /// </summary>
    //| An EventArgs class to allow extension in future.
    public class MessageDialogRespondedEventArgs : EventArgs
    {
        public MessageDialogRespondedEventArgs(object result)
        {
            MessageDialogResult = result;
        }

        public virtual object MessageDialogResult { get; protected set; }
    }

    /// <summary>
    /// Flags specifying what changes interceptors can make to a message dialog.
    /// </summary>
    [Flags]
    public enum MessageDialogInterceptFlags
    {
        AllowSuppress = 1,
        AllowChangeDefault = 2,

        Default = AllowSuppress | AllowChangeDefault
    }
}