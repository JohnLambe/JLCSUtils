using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.DependencyInjection.ConfigInject;  //TODO Move IConfigProvider
using JohnLambe.Util.PluginFramework.Attributes;
using JohnLambe.Util.PluginFramework.Interfaces;
using JohnLambe.Util.Collections;
using JohnLambe.Util.DependencyInjection.ConfigInject.Providers;

namespace JohnLambe.Util.Reflection.LooselyCoupledEvent
{
    public class LooselyCoupledEventProcessorBase
    {
        #region Mappings metadata

        // Nested classes are declared here so that references to them don't require a generic type parameter.

        /// <summary>
        /// Details of how an event parameter is populated.
        /// </summary>
        public class ParameterMapping
        {
            /// <summary>
            /// Value of <see cref="Source"/> to indicate that the event object itself is injected.
            /// </summary>
            public const int EventSource_Event = -1;
            public const int EventSource_EventProperty = 0;
            public const int EventSource_DI = 1;

            /// <summary>
            /// A parameter to the event handler method (the event handler is specified by <see cref="EventMapping.Method"/>.
            /// </summary>
            public ParameterInfo Parameter { get; set; }

            /// <summary>
            /// The property name in the event (or DI).
            /// </summary>
            public string SourceKey { get; set; }

            /// <summary>
            /// The type of the parameter to be populated.
            /// </summary>
            public Type RequiredType { get; set; }

            /// <summary>
            /// Which source to use (Event or DI).
            /// </summary>
            public int Source { get; set; }

            /// <summary>
            /// True iff this parameter is required.
            /// If true, an exception is thrown if the parameter cannot be populated.
            /// </summary>
            public bool Required { get; set; }

            /// <summary>
            /// Value to be passed if none is available from the event or other sources.
            /// </summary>
            public object DefaultValue { get; set; }

            /// <summary>
            /// When the source is a class/interface type, this is the resolved property on that type.
            /// </summary>
            public PropertyInfo SourceProperty { get; set; }   // ?

            public override string ToString()
            {
                return SourceKey + " -> " + Parameter?.Name
                    + StrUtils.NullPropagate(" (" + RequiredType + ") ")
                    + (Required ? "Required" : "")
                    + StrUtils.NullPropagate(" = " + DefaultValue)
                    ;
            }
        }

        /// <summary>
        /// Details of how a specific event is mapped to a specific target (class).
        /// </summary>
        public class EventMapping
        {
            // the event and target type must be known. They're specified in EventMappingKey.
//            public Type TargetClass { get; set; }

            /// <summary>
            /// The method on the target class that handles the event.
            /// </summary>
            public MethodInfo Method { get; set; }

            /// <summary>
            /// Mappings between event handler parameters and the event properties (and other sources, such as DI).
            /// </summary>
            public ParameterMapping[] Parameters { get; set; }

            public override string ToString()
            {
                return Method.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected readonly EventMapping _noMapping = new EventMapping();

        #endregion
    }

    public class LooselyCoupledEventProcessor<TEvent> : LooselyCoupledEventProcessorBase
        where TEvent: class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The return type of the event handler. Use <see cref="System.Object"/> if it is void (null will be returned).</typeparam>
        /// <param name="target"></param>
        /// <param name="eventInstance"></param>
        /// <param name="valueProvider"></param>
        /// <returns></returns>
        public virtual T Invoke<T>(object target, TEvent eventInstance, IConfigProvider[] valueProvider = null)
        {
            var mapping = GetMapping(target.GetType(), eventInstance.GetType());
            if (mapping == null)
                return default(T);
            if (valueProvider == null)
                valueProvider = new IConfigProvider[] { new ObjectValueProvider(eventInstance) };
            return InvokeInternal<T>(target, mapping, eventInstance, valueProvider);
        }


        protected virtual T InvokeInternal<T>(object target, EventMapping eventMapping, TEvent eventInstance, IConfigProvider[] valueProvider)
        {
//            var mappings = GenerateParameterMappings(method,valueProvider);
            var arguments = new object[eventMapping.Parameters.Length]; // create an array for the arguments, of the same length as the number of parameters
            int index = 0;
            foreach (var mapping in eventMapping.Parameters)
            {
                object value;
                if(mapping.Source == ParameterMapping.EventSource_Event)
                {   // injecting the event object
                    arguments[index] = eventInstance;
                }
                else if (valueProvider[mapping.Source].GetValue(mapping.SourceKey, mapping.RequiredType, out value))
                {   // value resolved
                    arguments[index] = value;
                }
                else    // not resolved
                {
                    if (mapping.Required)
                        throw new EventInjectionFailedException("Event parameter injection failed for required parameter",mapping,eventMapping,target);
                    else
                        arguments[index] = mapping.DefaultValue;
                }
                index++;
            }

            return (T)eventMapping.Method.Invoke(target, arguments);
        }


        #region Mapping

        /// <summary>
        /// Get a mapping, creating it and caching it if it is not already in the cache.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        protected virtual EventMapping GetMapping(Type targetType, Type eventType)
        {
            var key = new EventMappingKey { EventType = eventType, TargetClass = targetType };
            EventMapping mapping;
            if(_mappings.TryGetValue(key, out mapping))
            {   // existing mapping found; may be null (no handler)
                return mapping;           // return it
            }
            else     // no mapping found
            {
                mapping = GenerateEventMapping(targetType, eventType);
                _mappings[key] = mapping;
                return mapping;
            }
        }

        /// <summary>
        /// Create and return the mappings for the given event and target type.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        protected virtual EventMapping GenerateEventMapping(Type targetType, Type eventType)
        {
            var mapping = new EventMapping();
            mapping.Method = GetMethodForEvent(targetType,eventType)?.Method;
            if(mapping.Method == null)
            {
                return null;   // no handler
            }
            mapping.Parameters = GenerateParameterMappings(mapping.Method, null, eventType);
            return mapping;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="valueProvider">Value provider for validation only.</param>
        /// <returns></returns>
        public virtual ParameterMapping[] GenerateParameterMappings(MethodInfo method, IConfigProvider[] valueProvider = null, Type eventInterface = null)
        {
            var parameters = method.GetParameters();       // get the parameters
            var mappings = new ParameterMapping[parameters.Length]; // create an array for the arguments, of the same length as the number of parameters
            int index = 0;
            foreach (var parameter in parameters)
            {
                var attribute = parameter.GetCustomAttribute<EventHandlerParameterAttributeBase>();
                mappings[index] = new ParameterMapping();
                mappings[index].Parameter = parameter;
                mappings[index].SourceKey = attribute?.Name ?? parameter.Name;  // get from attribute if present, otherwise parameter
                //                    (attribute != null && attribute.Name != null) ?
                //                        attribute.Name
                //                        : mappings[index].SourceKey = parameter.Name;
                mappings[index].RequiredType = parameter.ParameterType;

                mappings[index].Source = (attribute is EventHandlerParameterDiInjectAttribute) ?
                    ParameterMapping.EventSource_DI
                    : ParameterMapping.EventSource_EventProperty;
                if (mappings[index].Source == ParameterMapping.EventSource_EventProperty
                    && IsEventInjectable(parameter,eventInterface)
                    )
                {
                    mappings[index].Source = ParameterMapping.EventSource_Event;
                }

                mappings[index].Required = attribute == null ? !parameter.HasDefaultValue : attribute.Required;
                    // if no attribute, injection is required if there is no default value for the parameter.
//                mappings[index].DefaultValue = attribute == null ? null : attribute.DefaultValue;
                mappings[index].DefaultValue = parameter.DefaultValue;

                if(valueProvider != null
                    && valueProvider.Length >= mappings[index].Source
                    && valueProvider[mappings[index].Source] != null)   // if the provider is given (it should be provided only if it should be validated at this time)
                {
                    // required only ?
                    object value;
                    if(!valueProvider[mappings[index].Source].GetValue(mappings[index].SourceKey, mappings[index].RequiredType, out value))
                    {
                        throw new EventInjectionFailedException("Event parameter injection failed for required parameter", mappings[index]);
                    }
                }

                index++;
            }
            return mappings;
        }

        /// <summary>
        /// True if the given parameter can be injected with an event instance of the given type
        /// (injecting the event object rather than a property of it).
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="eventInterface"></param>
        /// <returns></returns>
        protected static bool IsEventInjectable(ParameterInfo parameter, Type eventInterface)
        {
            return (typeof(TEvent).IsAssignableFrom(parameter.ParameterType))    // if this parameter is of an event type
                            && parameter.ParameterType.IsAssignableFrom(eventInterface);   // and the current event type can be assigned to it
        }

        #region Handler Resolution

        /// <summary>
        /// Given a target type and an event type,
        /// choose the method to be invoked to handle the event.
        /// </summary>
        /// <param name="targetClass">target type (usually a class, but could be an interface)</param>
        /// <param name="eventInterface">event type (usually an interface)</param>
        /// <returns>The event handler method, or null if there is no handler for the event.</returns>
        public virtual HandlerMatch GetMethodForEvent(Type targetClass, Type eventInterface)
        {
            HandlerMatch bestMatch = null;

            foreach(var method in targetClass.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {           
                var attributes = method.GetCustomAttributes<EventHandlerAttribute>();
                foreach (var attribute in attributes)
                {
                    if (attribute.Enabled)  //TODO: Support EventHandlerAttribute.Enabled
                    {
                        var handler = new HandlerMatch(method, attribute, eventInterface);
                        switch (handler.Compare(bestMatch))
                        {
                            case HandlerMatch.HandlerComparisonResult.PreferThis:
                                bestMatch = handler;
                                break;
                            case HandlerMatch.HandlerComparisonResult.Equivalent:
                                if (bestMatch != null && handler.IsValid)   // bestMatch is validif not null (otherwise it would not have been preferred to null)
                                    throw new AmbiguousHandlerException("Ambiguous handler specification", bestMatch, handler);
                                // throw exception if there are two handler attributes for exactly the same event type at the same level in the class hierarchy.
                                break;
                        }
                    }
                }
            }

            //TODO

            return bestMatch;
        }

        public class HandlerMatch : IHandlerMatch
        {
            public HandlerMatch(MethodInfo method, EventHandlerAttribute handlerAttribute, Type eventInterface)
            {
                Method = method;
                HandlerAttribute = handlerAttribute;
                EventInterface = eventInterface;

                if (handlerAttribute.EventType != null)
                {
                    HandledEventType = handlerAttribute.EventType;
                }
                else
                {   // Handler attribute does not specify an event type, so it must be inferred from the parameters:
                    foreach (var parameter in method.GetParameters())
                    {
//                        if ( (typeof(TEvent).IsAssignableFrom(parameter.ParameterType))    // if this parameter is of an event type
//                            && parameter.ParameterType.IsAssignableFrom(eventInterface))   // and the current event type can be assigned to it
                        if(IsEventInjectable(parameter, eventInterface))
                        {
                            if (HandledEventType != null)
                                throw new HandlerResolutionFailedException("Ambiguous handler definition - there are two parameters of an event type. This can be handled if you define the event type in the EventHandler attribute.");  // there are two parameters that could be injected with the event.
                            //| We could resolve this similarly to how methods are chosen - use the more specific one.
                            //| For simplicity, we just reject it. It would be likely to be consufing to someone reading that method.
                            //| Parameters of multiple event types are possible by defining the event type in the handler attribute on the method.

                            HandledEventType = parameter.ParameterType;
                            // searching the paramter list continues to ensure that they are not ambiguous

                            EventParameterPosition = parameter.Position;
                        }
                    }
                }
            }

            /// <summary>
            /// true iff this is a valid handler for <see cref="EventInterface"/>
            /// (i.e. it could be invoked with the event; it would the correct handler if there were no other valid handlers).
            /// </summary>
            public virtual bool IsValid => HandledEventType==null ? false : HandledEventType.IsAssignableFrom(EventInterface);

            /// <summary>
            /// The event handler method.
            /// </summary>
            public virtual MethodInfo Method { get; protected set; }

            /// <summary>
            /// The relevant event handler attribute on this method.
            /// </summary>
            public virtual EventHandlerAttribute HandlerAttribute { get; protected set; }

            /// <summary>
            /// The event type that this handler is declared (by annotation or convention) as being able to handle.
            /// </summary>
            public virtual Type HandledEventType { get; protected set; }

            public virtual Type EventInterface { get; protected set; }

            /// <summary>
            /// The index of the parameter to which the event object itself will be injected,
            /// and which determined the type of the event.
            /// (null if the event object is not injected, but null does not necessarily mean that it is not injected.)
            /// ///TODO: reword
            /// </summary>
            //| Named this, rather than 'EventParameterIndex', for consistency with ParameterInfo.Position.
            public virtual int? EventParameterPosition { get; set; }

            /// <summary>
            /// Result status of comparing two handlers.
            /// </summary>
            public enum HandlerComparisonResult { Equivalent, PreferThis, PreferOther };

            /// <summary>
            /// Returns true if this handler is preferred to <see cref="handler"/> 
            /// for handling <see cref="EventInterface"/>.
            /// </summary>
            /// <param name="handler">Details of another handler. It must have the same
            /// <see cref="EventInterface"/> as this one.</param>
            /// <returns>true if <see cref="handler"/> is preferred.</returns>
            public virtual HandlerComparisonResult Compare(HandlerMatch handler)
            {
                if (handler == null)
                {
                    if (IsValid)
                        return HandlerComparisonResult.PreferThis;  // any valid handler is better than null
                    else
                        return HandlerComparisonResult.Equivalent;
                }

                // if one is invaid, the other is preferred:
                if (!IsValid)
                {
                    if (handler.IsValid)                    // this is not valid and `handler` is
                        return HandlerComparisonResult.PreferOther;
                    else                                    // both invalid
                        return HandlerComparisonResult.Equivalent;
                }
                else    // this is valid
                {
                    if (!handler.IsValid)                   // this is valid and `handler` is not
                        return HandlerComparisonResult.PreferThis;
                }
                // both handlers are valid at this point

                // if one is more specific, it is preferred:
                if (IsMoreSpecific(handler))
                {   // if the other one is more specific
                    return HandlerComparisonResult.PreferOther;     // prefer `handler`
                }
                if (handler.IsMoreSpecific(this))
                {   // if this one is more specific
                    return HandlerComparisonResult.PreferThis;     // prefer this
                }

                //TODO: if attribute of other handler is declared on a method on a class lower in the type hierarchy, it is preferred (so that a handler attribute of a base class can be overridden) ??
                // or require base class one to be explicitly disabled.

                return HandlerComparisonResult.Equivalent;
            }

            /// <summary>
            /// True iff `handler` is more specific (in what events it handles) than this
            /// (e.g. if handler handles a derived class or interface of what this handles).
            /// </summary>
            /// <param name="handler"></param>
            /// <returns></returns>
            protected virtual bool IsMoreSpecific(HandlerMatch handler)
            {
                return HandledEventType.IsAssignableFrom(handler.HandledEventType)   // e.g. if this handles an ancestor of what `handler` handles (HandlerEventType ancestor of handler.HandlerEventType)
                    && !handler.HandledEventType.IsAssignableFrom(HandledEventType); // and the converse is not true
                /*                return !(
                                    handler.HandledEventType.IsAssignableFrom(HandledEventType)   // e.g. if handler.HandledEventType is an ancestor of HandledEventType
                                    && !HandledEventType.IsAssignableFrom(handler.HandledEventType)
                                    );
                                    */
            }

            public override string ToString()
            {
                return "Handler: " + Method
                    + " [" + StrUtils.NullPropagate("Handles " + HandledEventType?.Name + "; ")
                    + StrUtils.NullPropagate("For ", EventInterface?.Name, "; ")
                    + StrUtils.NullPropagate("Attribute: ", HandlerAttribute.ToString(), "; ")
                    + (IsValid ? "Valid " : "INVALID! ")
                    + "]";
            }
        }

        #endregion

        /// <summary>
        /// The key of the event mappings cache.
        /// </summary>
        protected struct EventMappingKey
        {
            /// <summary>
            /// The event (interface) type.
            /// </summary>
            public Type EventType;

            /// <summary>
            /// The type (class) that the event is to be delivered to.
            /// </summary>
            public Type TargetClass;
        }

        /// <summary>
        /// Cached event mappings.
        /// </summary>
        protected IDictionary<EventMappingKey, EventMapping> _mappings = new Dictionary<EventMappingKey, EventMapping>();

        #endregion

    }

}
