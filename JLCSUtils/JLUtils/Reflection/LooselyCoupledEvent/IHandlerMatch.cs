using System;
using System.Reflection;
using JohnLambe.Util.PluginFramework.Attributes;

namespace JohnLambe.Util.Reflection.LooselyCoupledEvent
{
    public interface IHandlerMatch
    {
        Type HandledEventType { get; }
        EventHandlerAttribute HandlerAttribute { get; }
        MethodInfo Method { get; }
    }
}