using JohnLambe.Util;
using JohnLambe.Util.Text;
using MvpFramework.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Menu
{
    /// <summary>
    /// Builds a collection of options (a type of menu model) from attributed methods.
    /// </summary>
    public class OptionCollectionBuilder
    {
        /// <summary>
        /// Build a list of options from handlers on a given object.
        /// </summary>
        /// <param name="target">The object with the handler methods.</param>
        /// <param name="filter">Value to filter handlers by (using their <see cref="MvpHandlerAttribute.Filter"/> property or equivalent).</param>
        /// <returns>the collection of menu items / options.</returns>
        public virtual OptionCollection Build(object target, string filter)
        {
            var options = new Dictionary<string, MenuItemModel>();

            foreach (var handlerInfo in _handlerResolver.GetHandlersInfo(target, null))
            {
                if (handlerInfo.Attribute != null
                    && ((filter == null) || (handlerInfo.Attribute.Filter?.Contains(filter) ?? false))
                    )
                {
                    var id = handlerInfo.Attribute?.Id ?? handlerInfo.Method?.Name.RemovePrefix(HandlerNamePrefix).RemoveSuffix(HandlerNameSuffix);
                    var option = new MenuItemModel(options, id)
                    {
                        DisplayName = handlerInfo.Attribute?.DisplayName ??
                            CaptionUtils.GetDisplayName(handlerInfo.Method, HandlerNamePrefix, HandlerNameSuffix),
                        //| Alternatively, we could use the ID.
                        HotKey = handlerInfo.Attribute.HotKey,
                        IconId = handlerInfo.Attribute.IconId,
                        IsDefault = handlerInfo.Attribute.IsDefault,
                        Order = handlerInfo.Attribute.Order,
                    };
                    option.Invoked += handlerInfo.HandlerWithArgsDelegate; // CreateInvokeDelegate(handlerInfo, target);
                        //(item, args) => handlerInfo.Method.Invoke(target, new object[] { });
                    options[id] = option;
                }
            }

            return new OptionCollection(options);
        }

        /// <summary>
        /// Optional prefix of names of handler methods.
        /// This is exlcuded from the handler name if present.
        /// </summary>
        public const string HandlerNamePrefix = "Handle_";

        /// <summary>
        /// Optional suffix of names of handler methods.
        /// This is exlcuded from the handler name if present.
        /// </summary>
        public const string HandlerNameSuffix = "Clicked";

        protected readonly HandlerResolver _handlerResolver = new HandlerResolver();
    }

}
