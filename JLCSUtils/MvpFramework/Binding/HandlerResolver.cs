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
        public virtual VoidDelegate GetHandler(object target, string handlerId, string filter = null)
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

        /// <summary>
        /// Return details of handlers for the given ID.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="handlerId">Handler ID, null to return all handlers.</param>
        /// <returns></returns>
        public virtual IEnumerable<Handler> GetHandlersInfo(object target, string handlerId, string filter = null)
        {
            var handlers = new List<Handler>();                                // to hold a list of all handlers for the handlerId
            foreach (var method in target?.GetType().GetMethods())             // all methods
            {
                foreach (var attrib in method.GetCustomAttributes<MvpHandlerAttribute>()      // all attributes of each method
                    .Where(a => a.Enabled && (handlerId == null || a.Name.Equals(handlerId))    // attributes for the specified handler on this method
                    && FilterMatches(filter,a.Filter))                // apply the filter
                    )              
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchFilter">The filter value to search for. null (no filter) matches anything - always returns true.</param>
        /// <param name="targetFilters">The list of filters for an item. <paramref name="searchFilter"/> can match any of these.</param>
        /// <returns>True iff the filter matches.</returns>
        public virtual bool FilterMatches(string searchFilter, string[] targetFilters)
        {
            if (searchFilter == null)
                return true;
            foreach (var f in targetFilters)
            {
                if (f.Equals(searchFilter))
                    return true;
            }
            return false;
        }

    }
}
