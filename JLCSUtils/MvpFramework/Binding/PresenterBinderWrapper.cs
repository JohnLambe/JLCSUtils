﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    public abstract class PresenterBinderWrapperBase
    {
        public abstract EventHandler GetHandler(string handlerId, string filter = null);

    }

    /// <summary>
    /// Wraps a Presenter and provides an interface for locating handlers on it.
    /// </summary>
    public class PresenterBinderWrapper : PresenterBinderWrapperBase
    {
        public PresenterBinderWrapper(IPresenter presenter)
        {
            this.Presenter = presenter;
        }


        public override EventHandler GetHandler(string handlerId, string filter = null)
        {
            var handlerDelegate = _handlerResolver.GetHandler(Presenter, handlerId, filter);
            return (sender, args) => handlerDelegate.Invoke();
            /*
            foreach( var method in Presenter.GetType()
                .GetMethods(BindingFlags.Public)
                // get attributes
                .Where(m => )
            {

            }
            */
        }

        //TODO: Return a type more decoupled from the presenter - that provides delegates instead of methods.
        // Maybe not Attributes.
        // public virtual IEnumerable< > GetHandlers(string filter = null)

        public virtual IEnumerable<HandlerResolver.Handler> GetHandlerMethods(string filter = null)
        {
            return _handlerResolver.GetHandlersInfo(Presenter, null, filter);
        }

        /*
        protected virtual string GetHandlerIdKey(MethodInfo method, string handlerId, string filter)
        {
            method.GetCustomAttributes<MvpHandlerAttribute>()
                .Where(a => a.Name.Equals(handlerId) && );
        }
        */

        public virtual IPresenter Presenter { get; private set; }
        protected readonly HandlerResolver _handlerResolver = new HandlerResolver();
    }
}