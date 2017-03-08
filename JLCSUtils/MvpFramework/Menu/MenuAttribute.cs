using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Menu
{
    // from JohnLambe.Utils.WebUtils.Menu

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public abstract class MenuAttributeBase : Attribute
    {
        public const string DefaultMenuSetId = "";

        /// <summary>
        /// Identifies the menu item (may be a menu or sub-menu).
        /// Must be unique among all items in an application (even if different assemblies) with the same effective MenuSetId.
        /// <para>Can be injected on invoking, so that the handler can tell where it was invoked from (if it appears in multiple menus).</para>
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Id of the parent menu.
        /// null for top level items.
        /// </summary>
        public virtual string ParentId { get; set; }

        /// <summary>
        /// Identifies the menu (whole menu structure) that the item is in.
        /// If null, and the assembly has a <see cref="DefaultMenuSetId"/> attribute,
        /// its value is used.
        /// </summary>
        public virtual string MenuSetId { get; set; }

        /// <summary>
        /// Name as displayed in the menu.
        /// null for default (based on the class name).
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Character that can be typed to choose this item, while in the parent menu.
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
        /// True iff this is a (sub)menu.
        /// </summary>
        public virtual bool IsMenu { get; }

        /// <summary>
        /// Parameters to be passed to the handler.
        /// </summary>
        public virtual object[] Params { get; set; }

        /// <summary>
        /// Rights or roles required to access this item.
        /// To access this, the user must have one of the rights specified by an element of the array.
        /// The format of the string depends on the consming system. It may specify a combination of rights/roles.
        /// (So elements of the array are ORed, but rights may be ANDed within each element.)
        /// </summary>
        public virtual string[] Rights { get; set; }

        /// <summary>
        /// Expression or value to filter on.
        /// </summary>
        public virtual string Filter { get; set; }
    }

    /// <summary>
    /// Adds an item to a menu.
    /// </summary>
    public class MenuItemAttribute : MenuAttributeBase
    {
        public MenuItemAttribute(string parentId, string displayName = null)
        {
            ParentId = parentId;
            DisplayName = displayName;
        }

        public virtual MenuStyle Style { get; set; }

        public override bool IsMenu => false;
    }

    /// <summary>
    /// Type or style of a menu or sub-menu.
    /// These specify different ways or styles of displaying the menu.
    /// </summary>
    public enum MenuStyle
    {
        /// <summary>
        /// In the style of a main menu, typically dropped down from the top of a window,
        /// or a ribbon control.
        /// </summary>
        MainMenu,

        /// <summary>
        /// A popup or context menu.
        /// </summary>
        PopupMenu,

        /// <summary>
        /// A group of items within a menu, such as a panel within a tab in a ribbon,
        /// or a group of items separated by a separator line in a vertical menu.
        /// </summary>
        Group,

        ToolBar,

        Custom
    };

    /// <summary>
    /// Creates a menu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class MenuAttribute : MenuAttributeBase
    {
        public MenuAttribute(string parentId = null, string id = null, string displayName = null)
        {
            ParentId = parentId;
            Id = id;
            DisplayName = displayName;
        }

        public override bool IsMenu => true;
    }

    /// <summary>
    /// An attribute that can be placed on an assembly or any class to register any class as a menu item.
    /// Can be used to register a class from another assembly, possibly with parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class MenuItemInstanceAttribute : MenuItemAttribute
    {
        public MenuItemInstanceAttribute(string parentId, string displayName = null) : base(parentId, displayName)
        {
        }

        /// <summary>
        /// The class that handles the menu item.
        /// May be null. If null, and this is defined on a class, the class that it is defined on is used.
        /// </summary>
        public virtual Type Handler { get; set; }
    }

    /// <summary>
    /// Specifies the default value of <see cref="MenuAttributeBase.MenuSetId"/> for an assembly.
    /// This applies to all <see cref="MenuAttributeBase"/> attributes in the assembly, where <see cref="MenuAttributeBase.MenuSetId"/> is null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class DefaultMenuSetId : Attribute
    {
        public DefaultMenuSetId(string menuSetId)
        {
            this.MenuSetId = menuSetId;
        }

        public virtual string MenuSetId { get; set; }
    }

}
