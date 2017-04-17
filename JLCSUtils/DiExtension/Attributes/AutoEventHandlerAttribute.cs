using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.Attributes
{
    //| Possible implementations:
    //|  - Attribute on method that makes it a handler.
    //|  - Attribute on class, referencing a method.
    //| *- Attribute on class, method name is the same for all.
    //|
    //| How to identify the event to add the handler to:
    //|  - Name in attribute on event declaration, matches name in attribute on handler.
    //|  - Anything with compatible signature.
    //| *- Attribute specifies delegate type.
    //|  - Attribute specifies type of first event parameter (should be a class unique to that event).



    /// <summary>
    /// Registers a handler for an event.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class AutoEventHandlerAttribute : DiAttribute
    {
        public AutoEventHandlerAttribute(Type eventType, int priority = 0)
        {
            this.EventType = eventType;
            this.Priority = priority;
        }

        /// <summary>
        /// null to register for all events of the given type.
        /// </summary>
        public virtual string Key { get; set; }

        /// <summary>
        /// The type of the delegate of the event handled.
        /// </summary>
        public virtual Type EventType { get; set; }

        /// <summary>
        /// Determines the order in which event handlers are fired.
        /// Iff 0, the Priority of the plugin (from PluginAttribute) is used.
        /// </summary>
        public virtual int Priority { get; set; }
    }

}
