using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.PluginFramework.Attributes
{
    public abstract class EventHandlerAttributeBase : PluginAttributeBase
    {
    }

    /// <summary>
    /// Indicates that a method handles an event.
    /// <para>The method parameters can be injected with:
    /// - The event itself
    /// - Properties of the event
    /// - The sender of the event (IPluginDetails)
    /// - Values injected from the dependency injection container.
    /// </para>
    /// <para>If no event type is specified in the attribute, there must be an argument of type IPluginEvent (or descendant),
    /// and the method will be invoked for events that implement that interface.
    /// </para>
    /// <para>If the method has a return type of EventHandlerStatus, it indicates how propagation of the event continues.
    /// Otherwise, it no exception is thrown it behaves as if <see cref="EventHandlerStatus.Success"/> was returned.
    /// </para>
    /// <para>If the method throws an exception, it is treated as equivalent to returning <see cref="EventHandlerStatus.Failure"/>.
    /// (Propagation of the event continues).
    /// </para>
    /// <para>The method may be static. It must not be a constructor or destructor.
    /// </para>
    /// <para>The method must be public.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,Inherited=true,AllowMultiple=true)]
    public class EventHandlerAttribute : EventHandlerAttributeBase
    {
        public EventHandlerAttribute(Type eventType = null)
        {
            this.EventType = eventType;
            this.Enabled = true;
        }

        /// <summary>
        /// Useful for disabling a handler on an overridden method.
        /// </summary>
        /// <param name="enabled">False to disable.</param>
        public EventHandlerAttribute(bool enabled)
        {
            this.Enabled = enabled;
        }

        /// <summary>
        /// The interface type of the event handled by this method.
        /// Matches all events that implement this interface,
        /// so it can handle a category of events with a common base interface.
        /// When there are multiple methods that can handle an event,
        /// the method that specifies an interface lower in the type hierarchy is preferred.
        /// There must not be multiple handlers specifying the same interface type on the same class.
        /// <para>If there is an attribute of this type on a the same method in a base class,
        /// the one higher in the hierarchy is ignored.</para>
        /// <para>There must not be two attributes of with the same <see cref="EventType"/> on the same method declaration.</para>
        ///    // // Prefer handler defined on lower subclass??
        /// </summary>
        public virtual Type EventType { get; set; }

        /// <summary>
        /// This can be set to false to prevent the attributed method from handling the event.
        /// (For overriding an attribute on the method on a base class).
        /// If this is set to false and <see cref="EventType"/> is null, it disables all inherited <see cref="EventHandlerAttribute"/> attributes.
        /// <para>Defaults to true.</para>
        /// </summary>
        public virtual bool Enabled { get; set; }
/*
        /// <summary>
        /// ID of the event or event category to be handled.
        /// If both this and EventType are defined, the handler is called if either match.
        /// </summary>
        public virtual string EventTypeId { get; set; }
        //| So that the handler doesn't have to have a dependency on the assembly that defines the event.
*/

        //| Filter : Delegate or expression to further filter events    ??
        //|   Just let handler filter.
    }

/*
    //| Separate class to EventHandlerAttribute, so that it is not possible to provide both 
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandlerByIdAttribute : EventHandlerAttributeBase
    {
    }
*/

    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public abstract class EventHandlerParameterAttributeBase : PluginAttributeBase
    {
        public EventHandlerParameterAttributeBase()
        {
            Required = true;
            //| ignoring failed injection could hide errors, such as a mistyped property name, or a change that removes the item that should be injected.
        }

        /// <summary>
        /// If false, and no value is available, the default for the type is used.
        /// If true and no value is available, the handler is not called,
        /// and the event system continues as if the handler had thrown an exception.
        /// </summary>
        public virtual bool Required { get; set; }

        /// <summary>
        /// Disable early validation of the availability of the source property.
        /// Can be used when a parameter can be injected from a sub-interface of the declared event interface
        /// and the actual type is not known at compile time (e.g. if there are multiple sub-interfaces that have the property).
        /// </summary>
        public virtual bool NoStaticValidation { get; set; }

        /// <summary>
        /// The name of the property of the event or value from the DI context to be mapped to this parameter.
        /// If null, the parameter name is used.
        /// </summary>
        public virtual string Name { get; set; }

        // DefaultValue: The default value comes from the parameter's actual default value,
        // not overridable in a mapping attribute.
        //| We could provide the ability to override here, but it would probably rarely be used.
    }

    /// <summary>
    /// For methods with an EventHandlerAttributeBase attribute.
    /// Identifies a property of the event to be injected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class EventHandlerParameterAttribute : EventHandlerParameterAttributeBase
    {
        public EventHandlerParameterAttribute(string propertyName = null)
        {
            this.Name = propertyName;
        }
    }

    /// <summary>
    /// For methods with an EventHandlerAttributeBase attribute.
    /// Identifies an item to be injected from a dependency-injection container (not the event itself).
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class EventHandlerParameterDiInjectAttribute : EventHandlerParameterAttributeBase
    {
        public EventHandlerParameterDiInjectAttribute(string name = null)
        {
            this.Name = name;
        }
/*
        /// <summary>
        /// The name of the property of the event to be mapped to this parameter.
        /// If null, the parameter name is used.
        /// </summary>
        public virtual string Name { get; set; }
 */
    }

    [AttributeUsage(AttributeTargets.Property,Inherited=true,AllowMultiple=false)]
    public class InjectableAttribute : PluginAttributeBase
    {
        public InjectableAttribute(bool enabled = true)
        {
            this.Enabled = enabled;
        }

        /// <summary>
        /// True (default) if the value of this item can be injected into other objects.
        /// </summary>
        public virtual bool Enabled { get; set; }
    }


}
