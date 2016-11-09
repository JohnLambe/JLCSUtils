using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Reflection;
using JohnLambe.Util;
using System.Reflection;
using JohnLambe.Util.Collections;

namespace MvpFramework.Binding
{
    
    public class HandlerResolver
    {
        public struct Handler
        {
            public MvpHandlerAttribute Attribute;
            public MethodInfo Method;
        }

        /// <summary>
        /// Returns a delegate to invoke a handler on a given object.
        /// It may fire more than one handler method.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="handlerId"></param>
        /// <returns></returns>
        public VoidDelegate GetHandler(object target, string handlerId)
        {
            var handlerSorted = GetHandlersInfo(target,handlerId).Select(h => h.Method);  // get list of handlers
            // Make a delegate to invoke them in order:
            return () =>
            {
                foreach (var handlerMethod in handlerSorted)
                {
                    handlerMethod.Invoke(target, EmptyCollection<object>.EmptyArray);
                }
            };
        }

        public IEnumerable<Handler> GetHandlersInfo(object target, string handlerId)
        {
            var handlers = new List<Handler>();                                // to hold a list of all handlers for the handlerId
            foreach (var method in target?.GetType().GetMethods())             // all methods
            {
                foreach (var attrib in method.GetCustomAttributes<MvpHandlerAttribute>()
                    .Where(a => a.Name.Equals(handlerId)))              // attributes for the specified handler on this method
                {
                    handlers.Add(new Handler()
                    {
                        Attribute = attrib,
                        Method = method
                    });
                }
            }
            return handlers.OrderBy(h => h.Attribute.Order);
        }
    }
}
