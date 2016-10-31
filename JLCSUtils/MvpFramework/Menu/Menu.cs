using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JohnLambe.Util.Encoding;

namespace MvpFramework.Menu
{
    public class MenuItem
    {
        public MenuItem(Dictionary<string, MenuItem> allItems, string id)
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

        public virtual MenuItem Parent { get; set; }

        public virtual bool IsMenu { get; set; }

        /// <summary>
        /// The class that handles invoking of this item.
        /// May be null (not all handlers are classes).
        /// </summary>
        public virtual Type HandlerType { get; set; }

        public virtual object[] Params { get; set; }

        public virtual string[] Rights { get; set; }

        public virtual IEnumerable<MenuItem> Children
            => _allItems.Values.Where( m => m.Parent == this )
            .OrderBy( m => SortStringCalculator.IntToSortString(m.Order) + m.DisplayName );

        public readonly Dictionary<string, MenuItem> _allItems;

        public virtual void Invoke()
        {

        }

        public delegate void MenuItemInvokeDelegate(MenuItem item);

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
    }


    public class MenuModel
    {
        public MenuModel(Dictionary<string, MenuItem> allItems)
        {
            _allItems = allItems;
        }

        public virtual MenuItem GetRootMenu(string menuId)
        {
            return _allItems[menuId];
        }

        protected readonly Dictionary<string, MenuItem> _allItems;
    }

}
