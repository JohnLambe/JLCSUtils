using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiExtension.Attributes
{
    //| Possible implementations:
    //| *- Attribute on method that makes it a handler.
    //|  - Attribute on class, referencing a method.
    //|  - Attribute on class, method name is the same for all.
    //|
    //| How to identify the event to add the handler to:
    //|  - Name in attribute on event declaration, matches name in attribute on handler.
    //|  - Anything with compatible signature.
    //| *- Attribute specifies delegate type.
    //|  - Attribute specifies type of first event parameter (should be a class unique to that event).

    //TODO: Static validation:
    // - Report error if an event has an [Inject] attribute in a class that does not implement IHasAutoWiredEvent.
    // - Report error if AutoEventHandlerAttribute is used on a member of a class that does not have HasAutoWiredEventHandlerAttribute.
    // - Report error if AutoEventHandlerAttribute method does not have a prototype matching the specified delegate.


    /// <summary>
    /// Registers a handler for an event.
    /// <para>
    /// The class of any method that this is placed on must have the attribute <see cref="HasAutoWiredEventHandlerAttribute"/>.
    /// Event injection must be enabled/registered on the DI container.
    /// </para>
    /// </summary>
    /// <seealso cref="SimpleInject.EventAutoWirer"/>
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
        /// <para>When an event has no key in its <see cref="InjectAttribute"/>, all handlers,
        /// including those non-matching non-null keys, will be injected.</para>
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
