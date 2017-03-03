using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Dialog
{
    public class MessageInterceptor
    {
        public virtual void Execute(IMessageDialogModel messageModel, MessageDialogEventArgs args)
        {
            var interceptDetails = Repository.Get(messageModel.InstanceId);
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
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceId">The message's <see cref="IMessageDialogModel.InstanceId"/>.</param>
        /// <returns>Interception details for the specified message.
        /// null to not intercept.</returns>
        MessageDialogInterceptDetails Get(string instanceId);
    }


    public class MessageDialogInterceptDetails
    {
        public virtual object DefaultValue { get; set; }

        public virtual bool Suppress { get; set; }
    }
}
