using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Dialog
{
    /// <summary>
    /// Implements the Message Dialog Service.
    /// </summary>
    public class MessageDialogService : IMessageDialogService
    {
        public MessageDialogService(IMessageDialogPresenter dialogPresenter)
        {
            DialogPresenter = dialogPresenter;
        }

        /// <summary>
        /// Show a dialog.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="messageModel"></param>
        /// <returns></returns>
        public virtual TResult ShowMessage<TResult>(IMessageDialogModel<TResult> messageModel)
        {
            //TODO: Validate ReturnValue of all items (even if dialog is suppressed)

            TResult dialogResult;

            var flags = messageModel.InterceptFlags;

            var eventArgs = new MessageDialogEventArgs() { Message = messageModel };
            Intercept?.Invoke(this, eventArgs);

            if (!flags.HasFlag(MessageDialogInterceptFlags.AllowSuppress))    // if suppressing is not allowed
                eventArgs.Suppress = false;                             // ignore any attempt to do so
            if (!flags.HasFlag(MessageDialogInterceptFlags.AllowChangeDefault))
            {
                eventArgs.Result = null;
                //TODO: check the default hasn't changed
            }

            if (!eventArgs.Suppress)            // it not suppressed
            {
                dialogResult = ShowDialog(messageModel);
            }
            else
            {
                if (eventArgs.Result != null)                       // if this is set, it overrides the default value in the message model
                    dialogResult = (TResult)eventArgs.Result;
                else
                    dialogResult = (TResult)messageModel.Options.Default.ReturnValue;   // this can also be modified by an interceptor
            }

            eventArgs.Result = dialogResult;

            Log?.Invoke(this, eventArgs);

            return dialogResult;
        }

        protected virtual TResult ShowDialog<TResult>(IMessageDialogModel<TResult> messageModel)
        {
            return (TResult)DialogPresenter.Show(messageModel);
        }

        /// <summary>
        /// The presenter of the dialog UI.
        /// </summary>
        public virtual IMessageDialogPresenter DialogPresenter { get; protected set; }

        /// <summary>
        /// Event shown before showing the dialog, for handlers that can modify or suppress messages.
        /// </summary>
        public virtual event MessageDialogInterceptDelegate Intercept;

        /// <summary>
        /// Event shown after the dialog is shown or suppressed, for logging messages (shown or suppressed) and responses.
        /// </summary>
        public virtual event MessageDialogInterceptDelegate Log;
    }


    public delegate void MessageDialogInterceptDelegate(object sender, MessageDialogEventArgs args);


    public class MessageDialogEventArgs : EventArgs
    {
        /// <summary>
        /// The details of the message.
        /// </summary>
        public virtual IMessageDialogModel Message { get; set; }

        /// <summary>
        /// The response to the dialog, either chosen by the user, or (if <see cref="Suppress"/> is true) chosen automatically.
        /// </summary>
        public virtual object Result { get; set; }

        /// <summary>
        /// Handlers can set this to true to prevent the dialog from being shown.
        /// For loggers, this is true iff the dialog was suppressed.
        /// </summary>
        public virtual bool Suppress { get; set; } = false;
    }

}
