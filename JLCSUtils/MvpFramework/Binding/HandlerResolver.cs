using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Reflection;
using JohnLambe.Util;
using System.Reflection;
using JohnLambe.Util.Collections;
using JohnLambe.Util.Text;
using MvpFramework.Menu;
using JohnLambe.Util.Types;

namespace MvpFramework.Binding
{

    public class HandlerResolver
    {
        /// <summary>
        /// Details of a single handler instance - its metadata and the instance on which it is defined.
        /// </summary>
        public class Handler   //TODO: Refactor to subclass of MemberAttribute<MethodInfo,Attribute>
        {
            /// <summary>
            /// Attribute with metadata of the handler.
            /// </summary>
            public virtual MvpHandlerAttribute Attribute { get; set; }

            /// <summary>
            /// The method that implements the handler.
            /// </summary>
            public virtual MethodInfo Method { get; set; }

            /// <summary>
            /// The object on which the handler is defined.
            /// </summary>
            public virtual object Target { get; set; }

            public virtual EventHandler HandlerDelegate
                => Target == null ? (EventHandler)((sender, args) => { })
                : (sender, args) => Method.Invoke(Target, Array.Empty<object>());

            /// <summary>
            /// Delegate to invoke the handler.
            /// </summary>
            public virtual MenuItemModel.InvokedDelegate HandlerWithArgsDelegate
                => Target == null ? (sender, args) => { }
            : CreateInvokeDelegate();

            /// <summary>
            /// Name corresponding to this handler, for displaying in a user interface.
            /// </summary>
            public virtual string DisplayName
                => Attribute?.DisplayName ?? CaptionUtil.GetDisplayName(Method);

            /// <summary>
            /// Returns a delegate to call <see cref="Method"/>, populating the method's parameters from the parameters of the delegate.
            /// </summary>
            /// <returns></returns>
            protected virtual MenuItemModel.InvokedDelegate CreateInvokeDelegate()
            {
                var parameters = Method.GetParameters();
                if (parameters.Length == 0)
                {                                                      // the handler method has no parameters
                    return (sender, args) => Method.Invoke(Target, Array.Empty<object>());
                }
                else if (parameters.Length == 1)
                {   // 1 parameter, check its type:
                    if (parameters[0].ParameterType.IsAssignableFrom(typeof(MenuItemModel)))    // if the parameter is MenuItemModel
                        return (sender, args) => Method.Invoke(Target, new object[] { sender });
                    else if (parameters[0].ParameterType.IsAssignableFrom(typeof(MenuItemModel.InvokedEventArgs)))
                        return (sender, args) => Method.Invoke(Target, new object[] { args });
                }
                else if (parameters.Length == 2)
                {   // 2 parameters: pass sender and args (like a WinForms event handler):
                    if (parameters[0].ParameterType.IsAssignableFrom(typeof(MenuItemModel))             // validate the parameter types
                        && parameters[1].ParameterType.IsAssignableFrom(typeof(MenuItemModel.InvokedEventArgs)))
                    {
                        return (sender, args) => Method.Invoke(Target, new object[] { sender, args });
                    }
                }

                throw new MvpResolutionException("Handler method has invalid parameters: " + Method.ToString()
                    + "\nSupported parameters are " + typeof(MenuItemModel).ToString()
                    + " and/or " + typeof(MenuItemModel.InvokedDelegate).ToString());

                //TODO?: Could dynamically populate method arguments according to attributes that map them to event event parameters
                //  or other information available (Model, View or Presenter properties), or inject by DI.
            }
        }

        //TODO: Merge this with Handler.CreateInvokeDelegate
        protected static EventHandler CreateEventHandlerDelegateForMethod(MethodInfo method, object target)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0)
            {                                                      // the handler method has no parameters
                return (sender, args) => method.Invoke(target, Array.Empty<object>());
            }
            else if (parameters.Length == 1)
            {   // 1 parameter, check its type:
                if (parameters[0].ParameterType.IsAssignableFrom(typeof(MenuItemModel)))    // if the parameter is MenuItemModel
                    return (sender, args) => method.Invoke(target, new object[] { sender });
                else if (parameters[0].ParameterType.IsAssignableFrom(typeof(MenuItemModel.InvokedEventArgs)))
                    return (sender, args) => method.Invoke(target, new object[] { args });
            }
            else if (parameters.Length == 2)
            {   // 2 parameters: pass sender and args (like a WinForms event handler):
                if (parameters[0].ParameterType.IsAssignableFrom(typeof(MenuItemModel))             // validate the parameter types
                    && parameters[1].ParameterType.IsAssignableFrom(typeof(MenuItemModel.InvokedEventArgs)))
                {
                    return (sender, args) => method.Invoke(target, new object[] { sender, args });
                }
            }

            throw new MvpResolutionException("Handler method has invalid parameters: " + method.ToString()
                + "\nSupported parameters are " + typeof(MenuItemModel).ToString()
                + " and/or " + typeof(MenuItemModel.InvokedDelegate).ToString());

            //TODO?: Could dynamically populate method arguments according to attributes that map them to event event parameters
            //  or other information available (Model, View or Presenter properties), or inject by DI.
            // Could support any event handler delegate.
        }


        /// <summary>
        /// Returns a delegate to invoke a handler on a given object.
        /// It may fire more than one handler method.
        /// </summary>
        /// <param name="target">the object on which the handlers are defined.</param>
        /// <param name="handlerId">Handler ID, null for all handlers.</param>
        /// <param name="filter">Filter to match <see cref="MvpUiAttributeBase.Filter"/>. null to not filter by this.</param>
        /// <param name="allowNull">Determines what is returned if there are no handlers: Iff true, null is returned, otherwise a delegate that does nothing is returned.</param>
        /// <returns></returns>
        [return: Nullable]
        public virtual EventHandler GetHandler([NotNull] object target, [Nullable] string handlerId, [Nullable] string filter = null, bool allowNull = false)
        {
            var handlersSorted = GetHandlersInfo(target, handlerId).Select(h => h.Method);  // get list of handlers
            if (allowNull && !handlersSorted.Any())
                return null;   // no handlers

            // Make a delegate to invoke them in order:
            return (sender, args) =>
            {
                foreach (var handlerMethod in handlersSorted)
                {
                    CreateEventHandlerDelegateForMethod(handlerMethod, target).Invoke(sender,args);
//                    handlerMethod.Invoke(target, EmptyCollection<object>.EmptyArray);
                }
            };
        }

        /// <summary>
        /// Returns a delegate to invoke a handler on a given object.
        /// It may fire more than one handler method.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="handlerId"></param>
        /// <param name="filter">Filter to match <see cref="MvpUiAttributeBase.Filter"/>. null to not filter by this.</param>
        /// <returns></returns>
        [Obsolete("?")]
        public virtual MenuItemModel.InvokedDelegate GetHandlerWithArgs(object target, string handlerId, string filter = null) //TODO
        {
            var handlerSorted = GetHandlersInfo(target, handlerId).Select(h => h.Method);  // get list of handlers
            // Make a delegate to invoke them in order:
            return (sender, args) =>
            {
                foreach (var handlerMethod in handlerSorted)
                {
                    handlerMethod.Invoke(target, EmptyCollection<object>.EmptyArray);
                }
            };
        }

        /// <summary>
        /// Return details of handlers for the given ID and filter.
        /// </summary>
        /// <param name="target">the object on which the handlers are defined.</param>
        /// <param name="handlerId">Handler ID, null to return all handlers.</param>
        /// <param name="filter">Filter to match <see cref="MvpUiAttributeBase.Filter"/>. null to not filter by this.</param>
        /// <returns></returns>
        public virtual IEnumerable<Handler> GetHandlersInfo([NotNull] object target, [Nullable] string handlerId = null, [Nullable] string filter = null)
        {
            var handlers = new List<Handler>();                                // to hold a list of all handlers for the handlerId
            foreach (var method in target.ArgNotNull().GetType().GetMethods())             // all methods
            {
                foreach (var attrib in method.GetCustomAttributes<MvpHandlerAttribute>()      // all attributes of each method
                    .Where(a => a.Enabled && (handlerId == null || (a.Id?.Equals(handlerId) ?? false))    // attributes for the specified handler on this method
                    && FilterMatches(filter, a.Filter))                // apply the filter
                    )
                {
                    handlers.Add(new Handler()
                    {
                        Attribute = attrib,
                        Method = method,
                        Target = target
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
        public virtual bool FilterMatches([Nullable] string searchFilter, string[] targetFilters)
        {
            if (searchFilter == null)
                return true;
            if (targetFilters == null)
                targetFilters = DefaultFilter;
            foreach (var f in targetFilters)
            {
                if (f.Equals(searchFilter))
                    return true;
            }
            return false;
        }

        protected readonly string[] DefaultFilter = new string[] { "" };

    }
}
