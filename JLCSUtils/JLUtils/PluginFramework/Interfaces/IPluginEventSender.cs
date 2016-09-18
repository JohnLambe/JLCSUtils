using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.PluginFramework.Interfaces
{
    /// <summary>
    /// Interface to an event-sending service.
    /// <para>The implementing object is specific to the plugin it is injected into,
    /// so that it can provide the sender's <see cref="IPluginDetails"/> to the recipients.</para>
    /// </summary>
    public interface IEventSender
    {
        /// <summary>
        /// Send a given event to 0 or more handlers.
        /// </summary>
        /// <param name="evnt">The event to be sent. Must not be null.</param>
        /// <param name="destination">Destination plugin or container.
        /// null for the root (all plugins).
        /// Special constants:
        ///   Local - all siblings of the sender.
        ///   Children - all children of the sender.
        /// </param>
        /// <param name="filter">Filters plugins to receive the event. Returns true to deliver the event to the plugin.
        /// null for all plugins.</param>
        /// <param name="options"></param>
        EventHandlerStatus SendEvent(IPluginEvent evnt, IPluginDetails destination = null, PluginFilter filter = null, PluginEventOptions options = PluginEventOptions.Default);
    }

    /// <summary>
    /// Options on sending a Plugin Event.
    /// </summary>
    [Flags]
    public enum PluginEventOptions
    {
        None = 0,

        /// <summary>
        /// If set, the event is propagated to children of matching containers.
        /// </summary>
        IncludeChildren = 1 << 0,

//        /// <summary>
//        /// Stop propagation after the first handler that handles the event.
//        /// </summary>
//        FirstHandler = 1 << 1,

        /// <summary>
        /// The event may be delivered asynchronously.
        /// If this is not set, the event must be delivered to all recipients before the
        /// IEventSender call returns.
        /// </summary>
        Asynchronous = 1 << 8,

        Default = IncludeChildren
    }

    /// <summary>
    /// Return value of event handlers, indicating how the propagation of the event continues.
    /// Also, returned to the sender of an event, to inform it of whether an event was intercepted.
    /// </summary>
    [Flags]
    public enum EventHandlerStatus
    {
        None           = 0,

        Success        = 1 << 0,
        Failure        = 1 << 1,

        /// <summary>
        /// Stop propagation of the event.
        /// </summary>
        Intercept      = 1 << 8,

        /// <summary>
        /// Don't pass event to the rest of the plugins in the current container.
        /// </summary>
        LocalIntercept = 1 << 9,

        /// <summary>
        /// Don't deliver the event to the children of this container.
        /// </summary>
        ContainerIntercept = 1 << 10,
    }

    // To be decided:
    //  Event responses.
    //    Can IPluginEvent implementors allow modification by event recipients? 
    //    (Is EventSender allowed to deliver a copy - assuming no state change?).
    //  Callbacks.

}
