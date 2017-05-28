using JohnLambe.Util.Misc;
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
        /// <param name="parameters">Details of the message to be shown.</param>
        /// <returns>Indicates which option was chosen, when the message has multiple options (e.g. buttons).</returns>
        /// <typeparam name="TResult">The type of the result - depends on the message.</typeparam>
        TResult ShowMessage<TResult>(IMessageDialogModel<TResult> messageModel);
    }


    /// <summary>
    /// Model of a message dialog.
    /// </summary>
    /// <typeparam name="TResult">The type returned when an option is chosen in the dialog.</typeparam>
    public interface IMessageDialogModel
    {
        string InstanceId { get; }

        MessageDisplayType DisplayType { get; set; }

        string Title { get; set; }
        string Message { get; set; }

        [IconId]
        string Icon { get; set; }

        [IconId]
        string MessageImage { get; set; }

        MessageDialogType MessageType { get; set; }
        string MessageTypeId { get; set; }

        Exception Exception { get; set; }

        string DiagnosticMessage { get; set; }

        IOptionCollection Options { get; set; }

        MessageDialogInterceptFlags InterceptFlags { get; }

        event MessageDialogRespondedDelegate Responded;

        void FireResponded(object messageDialogResult);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult">The type to be returned from the <see cref="IMessageDialogService.ShowMessage{TResult}(IMessageDialogModel{TResult})"/> method.</typeparam>
    public interface IMessageDialogModel<TResult> : IMessageDialogModel
    {
    }

    /// <summary>
    /// Delegate to be fired when the user responds to a message.
    /// </summary>
    /// <param name="dialog">The <see cref="IMessageDialogModel"/> to which the response relates.</param>
    /// <param name="messageDialogResult">The chosen option - the same as the return value from <see cref="IMessageDialogService.ShowMessage{TResult}(IMessageDialogModel{TResult})"/>.</param>
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