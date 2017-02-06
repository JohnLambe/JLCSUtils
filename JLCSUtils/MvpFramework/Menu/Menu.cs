using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JohnLambe.Util.Encoding;

namespace MvpFramework.Menu
{
    /// <summary>
    /// Model of a menu item.
    /// This is mutable: Its state could change in response to user actions, etc. (by menu or the handler for the item).
    /// </summary>
    public class MenuItemModel
    {
        public MenuItemModel(IDictionary<string, MenuItemModel> allItems, string id)
        {
            _allItems = allItems;
            Id = id;
        }

        /// <summary>
        /// Identifies the menu item.
        /// Can be injected on invoking, so that the handler can tell where it was invoked from (if it appears in multiple menus).
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Id of the parent menu.
        /// null for top level items.
        /// </summary>
        public virtual string ParentId { get; set; }

        /// <summary>
        /// Name as displayed in the menu.
        /// null for default (based on the class name).
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Character that can be typed to choose this item while in the parent menu.
        /// </summary>
        public virtual char AcceleratorChar { get; set; }

        /// <summary>
        /// Items are sorted, within their parent, in ascending order of this value.
        /// </summary>
        public virtual int Order { get; set; }

        /// <summary>
        /// A key that can be pressed to invoke this item even when the menu is not open.
        /// </summary>
        public virtual KeyboardKey HotKey { get; set; }

        /// <summary>
        /// <see cref="MenuAttributeBase.Parent"/>
        /// </summary>
        public virtual MenuItemModel Parent { get; set; }

        /// <summary>
        /// <see cref="MenuAttributeBase.IsMenu"/>
        /// </summary>
        public virtual bool IsMenu { get; set; }

        /// <summary>
        /// The class that handles invoking of this item.
        /// May be null (not all handlers are classes).
        /// </summary>
        public virtual Type HandlerType { get; set; }

        /// <summary>
        /// <see cref="MenuAttributeBase.Params"/>.
        /// </summary>
        public virtual object[] Params { get; set; }

        /// <summary>
        /// <see cref="MenuAttributeBase.Rights"/>.
        /// </summary>
        public virtual string[] Rights { get; set; }

        /// <summary>
        /// <see cref="MenuAttributeBase.Filter"/>
        /// </summary>
        public virtual string Filter { get; set; }

        /// <summary>
        /// True iff this is the default option.
        /// Only one (or zero) option in a set should have this set to true.
        /// </summary>
        public virtual bool IsDefault { get; set; }

        /// <summary>
        /// Ordered list of the immediate children of this item.
        /// Never null. Empty if this is a leaf menu item (not a menu).
        /// </summary>
        public virtual IEnumerable<MenuItemModel> Children
            => _allItems.Values.Where(m => m.Parent == this)
            .OrderBy(m => SortStringCalculator.IntToSortString(m.Order) + m.DisplayName);

        /// <summary>
        /// Collection of menu items, including this one and its children.
        /// </summary>
        protected readonly IDictionary<string, MenuItemModel> _allItems;

        /// <summary>
        /// Do the action of this menu item.
        /// </summary>
        public virtual void Invoke()
        {
            Invoked?.Invoke(this);
        }

        public delegate void MenuItemInvokeDelegate(MenuItemModel item);

        /// <summary>
        /// Fired when the menu item is invoked (typically when chosen from the menu).
        /// </summary>
        public virtual event MenuItemInvokeDelegate Invoked;

        /// <summary>
        /// Name of this item for display to developers (e.g. in error messages).
        /// </summary>
        /// <returns></returns>
        public virtual string CodeDescription
            => DisplayName + " (" + Id + ")";


        public override string ToString()
            => CodeDescription;

        /// <summary>
        /// The default item in this menu / collection.
        /// null if there is no default.
        /// </summary>
        public virtual MenuItemModel Default
            => Children.FirstOrDefault(o => o.IsDefault);

        /// <summary>
        /// Return a text representation of the menu tree of this item and all its children (including indirect children).
        /// (For diagnostic use).
        /// </summary>
        /// <param name="indent"></param>
        /// <returns></returns>
        public virtual string MenuHierarchyText(string indent = "")
        {
            StringBuilder output = new StringBuilder(1024);
            output.Append(indent + ToString() + "\n");
            string newIndent = indent + "  ";
            foreach (var item in Children)
            {
                output.Append(newIndent + item.MenuHierarchyText(newIndent));
            }
            return output.ToString();
        }

        /// <summary>
        /// Identifier of the icon of the menu.
        /// null for no icon.
        /// </summary>
        public virtual string IconId { get; set; }  //TODO
        // From attribute, OR separately mapped to ID.

        /// <summary>
        /// 
        /// </summary>
        public virtual object Tag { get; set; }

        /// <summary>
        /// True iff this item is currently displayed (or would be displayed when the menu is shown).
        /// (False if it is hidden, for example, due not being applicable in the current state or configuration.)
        /// </summary>
        public virtual bool Visible { get; set; } = true;

        //TODO: Could copy attribute to property:
        public virtual MenuAttributeBase Attribute { get; set; }  // may be null
        // so that properties of subclasses of the attribute can be used by a consumer of this class.
        // Attributes are not the only way to populate this.

        //TODO: Toggle/check state ?

    }


    /// <summary>
    /// Model details of a collection of menus.
    /// </summary>
    public class MenuCollection
    {
        public MenuCollection(Dictionary<string, MenuItemModel> allItems)
        {
            _allItems = allItems;
        }

        public virtual MenuItemModel GetRootMenu(string menuId)
        {
            return _allItems[menuId];
        }

        public virtual MenuItemModel GetMenuItem(string menuId)
        {
            return _allItems[menuId];
        }

        protected readonly Dictionary<string, MenuItemModel> _allItems;
    }

}

