using JohnLambe.Util.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Dialog
{
    public class MessageInterceptor
    {
        /// <summary>
        /// Apply any applicable registered interceptors on the given message dialog.
        /// </summary>
        /// <param name="args"></param>
        public virtual void Execute(MessageDialogEventArgs args)
        {
            var interceptDetails = Repository[args.Message.InstanceId];
            if (interceptDetails != null)     // do nothing if null
            {
                args.Suppress = args.Suppress || interceptDetails.Suppress;
                if (interceptDetails.DefaultValue != null)
                {
                    args.Message.Options.SetDefaultByReturnValue(interceptDetails.DefaultValue);
                }
            }
        }

        public virtual IMessageDialogInterceptDetailsRepository Repository { get; set; }
    }


    /// <summary>
    /// Interface to look up interception details for messages.
    /// </summary>
    public interface IMessageDialogInterceptDetailsRepository 
        : ISimpleLookup<string, MessageDialogInterceptDetails>
    {
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceId">The message's <see cref="IMessageDialogModel.InstanceId"/>.</param>
        /// <returns>Interception details for the specified message.
        /// null to not intercept.</returns>
        MessageDialogInterceptDetails Get(string instanceId);
        */
    }


    /// <summary>
    /// Details of how or if a message dialog is to be modified or suppressed.
    /// </summary>
    public class MessageDialogInterceptDetails
    {
        /// <summary>
        /// The default option/value returned from the dialog.
        /// null to leave it unchanged.
        /// <para>
        /// When the dialog is suppressed, this value is returned.
        /// When it is shown, this determines which option/button is the default in the user interface.
        /// </para>
        /// </summary>
        public virtual object DefaultValue { get; set; }

        /// <summary>
        /// True iff the dialog should not be shown.
        /// </summary>
        public virtual bool Suppress { get; set; }
    }


}
