using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Reflection;

using SimpleInjector;
using SimpleInjector.Advanced;

using JohnLambe.Util.Reflection;
using JohnLambe.Util.Collections;
using DiExtension.Attributes;

namespace DiExtension.SimpleInject
{
    /// <summary>
    /// Tags a class that can have auto-wired events.
    /// </summary>
    public interface IHasAutoWiredEvent
    {
    }


    /// <summary>
    /// Automatically assigns event handlers (on classes identified by attributes)
    /// to events (identified by attributes) on objects created by the DI container.
    /// </summary>
    public class EventAutoWirer
    {
        /// <summary>
        /// The name of the method of the handler class that implements the event delegate.
        /// </summary>
        protected const string HandlerMethodName = "Execute";

        public EventAutoWirer(Container container /*, IExtendedDiContext context*/)
        {
//            this.Context = context;
            this.Container = container;
            Action<IHasAutoWiredEvent> action =
                new Action<IHasAutoWiredEvent>(obj => AutoWireEvents(obj));
            container.RegisterInitializer<IHasAutoWiredEvent>(action);

            _handlerMappings = new CachedSimpleLookup<Type, IEnumerable<Type>>( t => FindHandlersForDelegate(t) );
        }

        /// <summary>
        /// Create an instance of this class and register it with the given container.
        /// </summary>
        /// <param name="container"></param>
        public static void RegisterWith(Container container /*, IExtendedDiContext context*/)
        {
            new EventAutoWirer(container /*, context*/);
        }

        /// <summary>
        /// Injects event handlers to events of the given object.
        /// Called on registration of the object with the DI container.
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void AutoWireEvents(IHasAutoWiredEvent obj)
        {
            foreach (var evt in obj.GetType().GetEvents())
            {
                var attribute = evt.GetCustomAttribute<InjectAttribute>();
                if (attribute != null && attribute.Enabled)       // the attribute is present and its Enabled property is true (the default)
                {
                    Type eventType = evt.EventHandlerType;
                    var handlers = GetHandlersForDelegate(eventType);
                    if (attribute.Required && handlers.Count() == 0)    // if handlers are required, and there are none
                    {
                        throw new InjectionFailedException("Injection failed for event: " + evt.ToString()
                            + " (No handlers found)");
                    }

                    foreach (Type handlerType in handlers)
                    {
                        var handlerInstance = Container.GetInstance(handlerType);  // get instance of handler class

                        //TODO: make parameter type list for handler (from delegate's parameters), so that class can implement multiple delegates.
                        // Type[] args =

                        var handlerDelegate = Delegate.CreateDelegate(eventType, handlerInstance, HandlerMethodName);  // make a delegate that calls the method

                        evt.AddEventHandler(obj, handlerDelegate);     // add it to the event of the object being created
                    }
                }
            }
        }

        /// <summary>
        /// Get the handler classes for a given delegate type.
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        protected virtual IEnumerable<Type> GetHandlersForDelegate(Type eventType)
        {
            return _handlerMappings[eventType];
        }

        /// <summary>
        /// Scan assemblies for attributed handler classes for the given delegate.
        /// </summary>
        /// <param name="eventType">The delegate type for which to find handlers.</param>
        /// <returns></returns>
        protected virtual IEnumerable<Type> FindHandlersForDelegate(Type eventType)
        {
            IList<Type> handlersList = new List<Type>();
            foreach (var assembly in Assemblies)
            {
                handlersList.AddAll(
                    assembly.GetTypes().Where(t => t.GetCustomAttribute<AutoEventHandlerAttribute>()?.EventType == eventType)
                );
            }
            return handlersList.OrderBy(t => t.GetCustomAttribute<AutoEventHandlerAttribute>()?.Priority);
        }



        //        protected readonly IExtendedDiContext Context;
        protected readonly Container Container;

        /// <summary>
        /// The assemblies to be scanned for handlers.
        /// </summary>
        public virtual ICollection<Assembly> Assemblies { get; set; } = new Assembly[] { Assembly.GetEntryAssembly() };

        /// <summary>
        /// Mapping from delegate type to a collection of handlers for it.
        /// </summary>
        protected readonly ISimpleLookup<Type, IEnumerable<Type>> _handlerMappings;
    }
}
