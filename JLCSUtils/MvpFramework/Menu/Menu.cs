using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JohnLambe.Util.Encoding;
using System.Diagnostics;
using JohnLambe.Util;
using JohnLambe.Util.Diagnostic;
using JohnLambe.Util.Types;

namespace MvpFramework.Menu
{
    /// <summary>
    /// Model of a menu item.
    /// This is mutable: Its state could change in response to user actions, etc. (by menu or the handler for the item).
    /// </summary>
    public class MenuItemModel
    {
        public MenuItemModel(string id = "") : this((IDictionary<string, MenuItemModel>)null, id)
        {
        }

        public MenuItemModel(MenuItemModel[] items, string id = "") : this((IDictionary < string, MenuItemModel > )null,id)
        {
            foreach(var item in items)
            {
                item.Parent = this;
                item._allItems = _allItems;
                _allItems[item.Id] = item;
            }
        }

        public MenuItemModel(IDictionary<string, MenuItemModel> allItems, string id)
        {
            if (allItems == null)
                allItems = new Dictionary<string, MenuItemModel>();

            _allItems = allItems;
            Id = id;
        }

        /// <summary>
        /// Identifies the menu item.
        /// Can be injected on invoking, so that the handler can tell where it was invoked from (if it appears in multiple menus).
        /// </summary>
        public virtual string Id { get; protected set; }

        /// <summary>
        /// Id of the parent menu.
        /// null for top level items.
        /// </summary>
        public virtual string ParentId { get; set; }

        /// <summary>
        /// Name as displayed in the menu.
        /// null for default (based on the class name).
        /// </summary>
        public virtual string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                Changed?.Invoke(this, new ChangedEventArgs(MenuItemChangeType.Name)); 
            }
        }
        protected string _displayName;

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
        /// The use of this depends on the consumer of the menu.
        /// It is generally for a value returned as a result when showing something modally.
        /// </summary>
        public virtual object ReturnValue { get; set; }

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
        protected IDictionary<string, MenuItemModel> _allItems;

        /// <summary>
        /// Do the action of this menu item.
        /// </summary>
        public virtual void Invoke(InvokedEventArgs args = null)
        {
            Invoked?.Invoke(this, args ?? InvokedEventArgs.EmptyInvokedEventArgs);
        }

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
        {
            get { return Children.FirstOrDefault(o => o.IsDefault); }
            set
            {
                ClearDefault();
                value.IsDefault = true;
            }
        }

        /// <summary>
        /// Make the item (in the children of this item) with the given <see cref="MenuItemModel.ReturnValue"/> the default.
        /// </summary>
        /// <param name="returnValue"></param>
        public virtual void SetDefaultByReturnValue(object returnValue)
        {
            ClearDefault();
            Children.FirstOrDefault(o => o.ReturnValue == returnValue).IsDefault = true;
        }

        /// <summary>
        /// Clear the default item (in the children of this item), so that there is no default in this menu.
        /// </summary>
        public virtual void ClearDefault()
        {
            foreach (var o in Children.Where(o => o.IsDefault))
            {
                o.IsDefault = false;
            }
        }

        /// <summary>
        /// The number of levels deep this item is in the hierarchy (0 if this is the root, i.e. <see cref="Parent"/> is null).
        /// </summary>
        public virtual int Level
        {
            get
            {
                int level = 0;
                var current = this;
                while(current.Parent != null)           // move up the hierarchy until the root is reached
                {
                    level++;
                    current = current.Parent;
                    Diagnostics.Assert(current != this, "INTERNAL ERROR: MenuItemModel: Circular Parent reference");   // if this happened, and we continued, it would loop indefinitely.
                }
                return level;
            }
        }

        [Nullable]
        public virtual Type ModelType { get; set; }

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
        /// This can be used by a consumer of this library.
        /// It must not be used within this library.
        /// Systems that use this must have their own rules for how it is used.
        /// It is recommended that use of this proprty is controlled by the View. It could be used to point to the UI object of the menu item in the view.
        /// </summary>
        public virtual object Tag { get; set; }

        /// <summary>
        /// True iff this item is currently displayed (or would be displayed when the menu is shown).
        /// (False if it is hidden, for example, due not being applicable in the current state or configuration.)
        /// </summary>
        public virtual bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                Changed?.Invoke(this, new ChangedEventArgs(MenuItemChangeType.Visible));
            }
        }
        protected bool _visible = true;

        /// <summary>
        /// Whether the menu item is enabled.
        /// Iff false, it should appear in a disabled (usually greyed) state in the user interface, and the UI should not allow invoking it.
        /// </summary>
        public virtual bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                Changed?.Invoke(this, new ChangedEventArgs(MenuItemChangeType.Enabled));
            }
        }
        protected bool _enabled = true;

        /// <summary>
        /// The state of a check box or toggle state of this menu item.
        /// null if this item does not support it.
        /// </summary>
        public virtual bool? Checked { get; set; }  = null;

        public virtual void Refresh()
        {
            Changed?.Invoke(this, new ChangedEventArgs(MenuItemChangeType.Refresh));
        }

        //TODO: Could copy attribute to property:
        public virtual MenuAttributeBase Attribute { get; set; }  // may be null
                                                                  // so that properties of subclasses of the attribute can be used by a consumer of this class.
                                                                  // Attributes are not the only way to populate this.

        /// <summary>
        /// True if this item matches either the given Id or Filter.
        /// </summary>
        /// <param name="id">Value to match <see cref="Id"/>, if not null.</param>
        /// <param name="filter">Value to match <see cref="Filter"/>, if not null.</param>
        /// <returns>true iff this item matches.</returns>
        internal virtual bool Matches(string id, string filter)
        {
            return (id != null && id == Id) || (filter != null && (Filter?.Contains(filter) ?? false));
        }

        #region Events

        /// <summary>
        /// A type of change to a menu item.
        /// </summary>
        public enum MenuItemChangeType
        {
            /// <summary>
            /// No change.
            /// </summary>
            None = 0,
            /// <summary>
            /// Force a refresh of a user interface displaying the item.
            /// </summary>
            Refresh = 1,
            /// <summary>
            /// A change other than those indicated by members of this enum.
            /// </summary>
            Other = 2,
            /// <summary>
            /// The DisplayName has changed.
            /// </summary>
            Name = 4,
            /// <summary>
            /// The Enabled state has changed.
            /// </summary>
            Enabled = 8,
            /// <summary>
            /// The Visible state has changed.
            /// </summary>
            Visible = 16,
        };

        /// <summary>
        /// Delegate for handlers to be notified of changes to a <see cref="MenuItemModel"/>.
        /// </summary>
        /// <param name="sender">The item on which something changed.</param>
        /// <param name="args"></param>
        public delegate void ChangedDelegate(MenuItemModel sender, ChangedEventArgs args);

        /// <summary>
        /// Arguments to <see cref="ChangedDelegate"/>.
        /// </summary>
        public class ChangedEventArgs : EventArgs
        {
            public ChangedEventArgs(MenuItemChangeType changeType)
            {
                ChangeType = changeType;
            }
            public virtual MenuItemChangeType ChangeType { get; protected set; }
        }

        /// <summary>
        /// Delegate for handlers for invoking a menu item / option.
        /// </summary>
        /// <param name="item">The item being invoked.</param>
        /// <param name="args"></param>
        public delegate void InvokedDelegate(MenuItemModel item, InvokedEventArgs args);

        /// <summary>
        /// Arguments to <see cref="InvokedDelegate"/>.
        /// </summary>
        public class InvokedEventArgs : EventArgs
        {
            public static readonly InvokedEventArgs EmptyInvokedEventArgs = new InvokedEventArgs();
        }

        /// <summary>
        /// Fired when the menu item is invoked (typically when chosen from the menu).
        /// </summary>
        public virtual event InvokedDelegate Invoked;

        /// <summary>
        /// Fired when certain properties of this class change.
        /// </summary>
        public virtual event ChangedDelegate Changed;

        #endregion
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
