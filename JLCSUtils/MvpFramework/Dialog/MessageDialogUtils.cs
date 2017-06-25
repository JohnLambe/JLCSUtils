using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Reflection;

namespace MvpFramework.Dialog
{

    public class MessageDialogUtil
    {
        public IEnumerable<MessageDialogDetails> FindDialogs()
        {
            return Assemblies.SelectMany(a => a.GetTypes().Where(t => !t.IsAbstract && t.IsDefined<RegisterMessageDialogAttribute>()))
                .Select(t => new MessageDialogDetails()
                {
                    Model = t.Create<IMessageDialogModel>(),
                    Intercept = new MessageDialogInterceptDetails()
                }
            );
        }

        public IEnumerable<Assembly> Assemblies { get; set; }
    }

    public class MessageDialogDetails
    {
        public IMessageDialogModel Model { get; set; }
        public MessageDialogInterceptDetails Intercept { get; set; }
    }
}
