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
        /// <param name="target"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual OptionCollection Build(object target, string filter)
        {
            var options = new Dictionary<string, Menu.MenuItemModel>();

            foreach (var handlerInfo in _handlerResolver.GetHandlersInfo(target, null))
            {
                if (handlerInfo.Attribute != null
                    && ((filter == null) || (handlerInfo.Attribute.Filter?.Contains(filter) ?? false))
                    )
                {
                    var option = new MenuItemModel(options, handlerInfo.Attribute.Id)
                    {
                        DisplayName = handlerInfo.Attribute.DisplayName ??
                            CaptionUtils.GetDisplayName(handlerInfo.Method, "Handle_", "Clicked"),
                        //| Alternatively, we could use the ID.
                        HotKey = handlerInfo.Attribute.HotKey,
                        IconId = handlerInfo.Attribute.IconId,
                        IsDefault = handlerInfo.Attribute.IsDefault,
                        Order = handlerInfo.Attribute.Order,
                    };
                    option.Invoked += (item, args) => handlerInfo.Method.Invoke(target, new object[] { });
                    options[handlerInfo.Attribute.Id] = option;
                }
            }

            var optionSet = new OptionCollection(options);
            return optionSet;
        }

        protected readonly HandlerResolver _handlerResolver = new HandlerResolver();
    }

}
