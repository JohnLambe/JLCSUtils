using JohnLambe.Util.Types;
using JohnLambe.Util;
using MvpFramework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    public abstract class PresenterBinderWrapperBase
    {
        public abstract EventHandler GetHandler(string handlerId, string filter = null, bool allowNull = false);

        public abstract HandlerResolver.Handler GetHandlerInfo([NotNull] string handlerId, string filter = null);

        public abstract IOptionCollection GetOptionCollection(string filter);
    }

    /// <summary>
    /// Wraps a Presenter and provides an interface for locating handlers on it.
    /// </summary>
    public class PresenterBinderWrapper : PresenterBinderWrapperBase
    {
        public PresenterBinderWrapper([NotNull] IPresenter presenter)
        {
            this.Presenter = presenter.ArgNotNull(nameof(presenter));
        }

        /// <summary>
        /// Get a collection of <see cref="MenuItemModel"/> representing a collection of handlers on the presenter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override IOptionCollection GetOptionCollection(string filter)
        {
            return new OptionCollectionBuilder().Build(Presenter, filter);
        }

        /// <summary>
        /// Returns a delegate for an event identified by a Handler ID.
        /// </summary>
        /// <param name="handlerId">The Handler ID (required).</param>
        /// <param name="filter">If not null, this filters handlers by their <see cref="MvpUiAttributeBase.Filter"/> value.</param>
        /// <param name="allowNull">Determines what is returned if there are no handlers: Iff true, null is returned, otherwise a handler that does nothing is returned.</param>
        /// <returns>An <see cref="EventHandler"/> for the requested handler.
        /// (This typically invokes 0 or more methods on a Presenter).
        /// </returns>
        [return: Nullable]
        public override EventHandler GetHandler([NotNull] string handlerId, string filter = null, bool allowNull = false)
        {
            return _handlerResolver.GetHandler(Presenter, handlerId, filter, allowNull);
/*            if (handlerDelegate == null)
                return null;
            return (sender, args) => handlerDelegate.Invoke(sender,args);
            */
        }

        public override HandlerResolver.Handler GetHandlerInfo([NotNull] string handlerId, string filter = null)
        {
            return _handlerResolver.GetHandlersInfo(Presenter, handlerId, filter).FirstOrDefault();
        }

        public virtual IEnumerable<EventHandler> GetHandlers(string filter = null)
        {
            return _handlerResolver.GetHandlersInfo(Presenter, null, filter)
                      .Select(h => new EventHandler((sender,args) => h.HandlerDelegate.Invoke(sender,args))); //TODO: Doesn't need conversion?
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
