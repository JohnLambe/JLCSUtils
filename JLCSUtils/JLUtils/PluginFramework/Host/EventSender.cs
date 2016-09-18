using JohnLambe.Util.PluginFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.PluginFramework.Host
{
    public class EventSender   //: IPluginEventSender
    {
        public EventSender(IPluginDetails sender)
        {
            this.Sender = sender;
        }

        public virtual EventHandlerStatus RaiseEvent(IPluginEvent evnt, string filter = null, PluginEventOptions options = PluginEventOptions.Default)
        {
            throw new NotImplementedException();
        }

        protected IPluginDetails Sender { get; set; }
        protected EventDispatcher Dispatcher { get; set; }
    }
}
