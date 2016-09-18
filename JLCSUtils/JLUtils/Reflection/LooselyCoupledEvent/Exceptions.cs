using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.Reflection.LooselyCoupledEvent
{
    /// <summary>
    /// An event handler parameter could not be injected.
    /// </summary>
    public class EventInjectionFailedException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="mapping">The parameter mapping.</param>
        /// <param name="eventMapping">The mapping of the event.</param>
        /// <param name="target">The instance that the event is being invoked on.
        /// NOT CURRENTLY USED: May be used to provide diagnostic information in future.</param>
        /// <param name="innerException"></param>
        public EventInjectionFailedException(string message = null,
            LooselyCoupledEventProcessorBase.ParameterMapping mapping = null,
            LooselyCoupledEventProcessorBase.EventMapping eventMapping = null,
            object target = null,
            Exception innerException = null) : base(message, innerException)
        {
            Mapping = mapping;
            EventMapping = eventMapping;
        }

        public override string Message
        {
            get
            {
                return base.Message
                    + StrUtils.NullPropagate(" (Parameter: ", Mapping?.Parameter?.Name,
                    StrUtils.NullPropagate(" on ", EventMapping.ToString()) ?? "",
                    ")");
            }
        }

        /// <summary>
        /// If present, this is the mapping for the parameter on which the error occurred.
        /// May be null.
        /// </summary>
        public virtual LooselyCoupledEventProcessorBase.ParameterMapping Mapping { get; protected set; }

        /// <summary>
        /// If present, this is the mapping for the event handler on which the error occurred.
        /// May be null.
        /// </summary>
        public virtual LooselyCoupledEventProcessorBase.EventMapping EventMapping { get; protected set; }

    }

    /// <summary>
    /// An event hander parameter could not be injected.
    /// </summary>
    public class HandlerResolutionFailedException : Exception
    {
        public HandlerResolutionFailedException(string message = null,
            IHandlerMatch handler1 = null,
            IHandlerMatch handler2 = null,
            Exception innerException = null) : base(message, innerException)
        {
            Handlers = new IHandlerMatch[2]
                { handler1, handler2 };
        }

        public override string Message
        {
            get
            {
                return base.Message
                    + " ("
                    + StrUtils.ConcatWithSeparator(", " + Handlers)
                    + " )";
            }
        }

        /// <summary>
        /// If present, this specifies handlers 
        /// This may be null or contain null.
        /// </summary>
        public virtual IHandlerMatch[] Handlers { get; protected set; }
    }

    /// <summary>
    /// There was no handler for the event.
    /// </summary>
    public class NoHandlerException : HandlerResolutionFailedException
    {
        public NoHandlerException(string message = null,
            Exception innerException = null) : base(message, null, null, innerException)
        {
        }
    }

    /// <summary>
    /// There was no handler for the event.
    /// </summary>
    public class AmbiguousHandlerException : HandlerResolutionFailedException
    {
        public AmbiguousHandlerException(string message = null,
            IHandlerMatch handler1 = null,
            IHandlerMatch handler2 = null,
            Exception innerException = null) : base(message, handler1, handler2, innerException)
        {
        }
    }
}