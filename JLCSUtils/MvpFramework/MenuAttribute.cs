using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    // from JohnLambe.Utils.WebUtils.Menu

    public class MenuAttributeBase : Attribute
    {
        /// <summary>
        /// Identifies the menu (whole menu structure) that the item is in.
        /// </summary>
        public virtual string MenuId { get; set; }

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
        /// Identifies the menu item.
        /// Can be injected on invoking, so that the handler can tell where it was invoked from (if it appears in multiple menus).
        /// </summary>
        public virtual string MenuItemId { get; set; }

        //        public virtual Keys AcceleratorKey { get; set; }

        /// <summary>
        /// MenuItemId of the parent menu.
        /// null for top level items.
        /// </summary>
        public virtual string ParentId { get; set; }

        /// <summary>
        /// True iff this is a (sub)menu.
        /// </summary>
//        public virtual bool IsMenu { get; set; }

        // Rights ?
    }

    /// <summary>
    /// Adds an item to a menu.
    /// </summary>
    public class MenuItemAttribute : MenuAttributeBase
    {
    }

    /// <summary>
    /// Creates a menu.
    /// </summary>
    public class MenuAttribute : MenuAttributeBase
    {
    }
}
