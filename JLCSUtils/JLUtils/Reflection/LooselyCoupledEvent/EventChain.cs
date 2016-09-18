using JohnLambe.Util.DependencyInjection.ConfigInject;
using JohnLambe.Util.DependencyInjection.ConfigInject.Providers;
using JohnLambe.Util.PluginFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection.LooselyCoupledEvent
{
    /// <summary>
    /// A sorted list of loosely-coupled events handlers.
    /// </summary>
    /// <typeparam name="TEvent">The base interface type of events.</typeparam>
    public class EventChain<TEvent> : PriorityEventBase<object,int>
        where TEvent : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor">The event processor to use.
        /// Since it cached mappings for events, it is more efficient to share an instance
        /// between anything than could dispatch the same events.
        /// If null, one is created.</param>
        public EventChain(LooselyCoupledEventProcessor<TEvent> processor = null)
        {
            _processor = processor ?? new LooselyCoupledEventProcessor<TEvent>();
        }

        public virtual IConfigProvider ConfigProvider { get; set; }


        public virtual EventHandlerStatus Invoke(TEvent evt)
        {
            _valueProviders[0] = new ObjectValueProvider(evt);
            _valueProviders[1] = ConfigProvider;

            EventHandlerStatus status = EventHandlerStatus.None;
            EventHandlerStatus overallStatus = EventHandlerStatus.None;
            foreach (var handler in _handlers)
            {
                    //                try
                    //                {
                status = _processor.Invoke<object>(handler.Value, evt, _valueProviders) as EventHandlerStatus? ?? EventHandlerStatus.Success;
//                }
//                catch(Exception ex)
//                {
//                    status = EventHandlerStatus.Failure;
//                }
                overallStatus = overallStatus | (status & (EventHandlerStatus.Success | EventHandlerStatus.Failure));
                if (status.HasFlag(EventHandlerStatus.Intercept) || status.HasFlag(EventHandlerStatus.LocalIntercept))
                    break;

            }

            return overallStatus;
        }

        protected LooselyCoupledEventProcessor<TEvent> _processor;

        protected IConfigProvider[] _valueProviders = new IConfigProvider[2];
    }
}
