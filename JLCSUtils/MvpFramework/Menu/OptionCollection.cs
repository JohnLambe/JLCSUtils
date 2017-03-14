using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Menu
{
    public interface IOptionCollection
    {
        /// <summary>
        /// The buttons or options that the user can choose, in the order in which they are displayed.
        /// null for default based on MessageType.
        /// </summary>
        IEnumerable<MenuItemModel> Children { get; }

        /// <summary>
        /// Make the item (in the children of this item) with the given <see cref="MenuItemModel.ReturnValue"/> the default.
        /// </summary>
        /// <param name="returnValue"></param>
        void SetDefaultByReturnValue(object returnValue);

        /// <summary>
        /// The default option.
        /// null if there is no default.
        /// </summary>
        MenuItemModel Default { get; set; }

        event MenuItemModel.ChangedDelegate Changed;
    }


    /// <summary>
    /// A collection of options, such as a list of commands (which might correspond to buttons etc.)
    /// or a menu.
    /// </summary>
    public class OptionCollection : MenuItemModel, IOptionCollection
    {
        public OptionCollection(MenuItemModel[] items, string id = "")
            : base(items, id)
        {
        }

        public OptionCollection(IDictionary<string, MenuItemModel> options = null, string id = "")
            : base(options, id)
        {
            if (options != null)
            {
                foreach (var option in options.Values)
                {
                    option.Parent = this;
                    option.ParentId = Id;
                }
            }
        }

        public virtual MenuItemModel AddOption(MenuItemModel option)
        {
            _allItems[option.Id] = option;
            option.Parent = this;
            option.ParentId = Id;
            return option;
            //            _options = _options.OrderBy(o => o.Order).ToList();
        }

        public virtual bool RemoveOption(MenuItemModel option)
        {
            return _allItems.Remove(option.Id);
        }

        public virtual MenuItemModel NewOption(string id)
        {
            var item = new MenuItemModel(_allItems, id);
            AddOption(item);
            return item;
        }

        //        public virtual IEnumerable<MenuItemModel> Options
        //            => base.Children; //.OrderBy(o => o.Order);
    }
}
