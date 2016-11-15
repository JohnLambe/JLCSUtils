using JohnLambe.Util;
using JohnLambe.Util.Collections;
using JohnLambe.Util.Encoding;
using JohnLambe.Util.FilterDelegates;
using JohnLambe.Util.Reflection;
using JohnLambe.Util.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Menu
{
    /// <summary>
    /// Builds menus based on attributes on handler classes.
    /// </summary>
    public class MenuBuilder
    {
        /// <summary>
        /// Scans assemblies and builds menus based on attributes on handler classes.
        /// </summary>
        /// <param name="assemblies">Assembly to scan. Defaults to the calling assembly and all assemblies directly referenced by it.</param>
        /// <param name="filter"></param>
        /// <returns>The collection of menus.</returns>
        public virtual MenuCollection BuildMenu(IEnumerable<Assembly> assemblies = null, BooleanExpression<Type> filter = null)
        {
            if (assemblies == null)
                assemblies = AssemblyUtils.GetReferencedAssemblies(Assembly.GetCallingAssembly(), true);

            var allItems = new Dictionary<string, MenuItemModel>();

            // find all types to be registered:
            foreach (var assm in assemblies)                // for each given assembly
            {
                var defaultMenuSetId = assm.GetCustomAttribute<DefaultMenuSetId>()?.MenuSetId ?? MenuAttributeBase.DefaultMenuSetId;

                foreach (var attribute in assm.GetCustomAttributes<MenuAttributeBase>())    // all MenuAttributeBase attributes on the assembly
                {
                    if (/*filter.TryEvaluate(type) &&*/ (attribute.MenuSetId ?? defaultMenuSetId) == MenuSetId)                   // apply the given filter
                    {
                        BuildItem(allItems, attribute, null);
                    }
                }

                foreach (var type in assm.GetTypes().Where(t => t.IsClass && !t.IsAbstract))    // all concrete classes
                {
                    foreach (var attribute in type.GetCustomAttributes<MenuAttributeBase>())    // all MenuAttribute attributes on each class
                    {
                        if (filter.TryEvaluate(type) && (attribute.MenuSetId ?? defaultMenuSetId) == MenuSetId)                   // apply the given filter
                        {
                            BuildItem(allItems, attribute, type);
                        }
                    }
                }
            }

            // assign parents:
            foreach (var item in allItems)
            {
                if (item.Value.ParentId != null && item.Value.Parent == null)
                {
                    item.Value.Parent = DictionaryUtils.TryGetValue(allItems, item.Value.ParentId).NotNull("Invalid parent for Menu item " + item.Value.CodeDescription + " - " + item.Value.ParentId + " not found");
                }
                AddInvokeDelegate(item.Value);
            }

            return new MenuCollection(allItems);
        }

        /// <summary>
        /// Create a menu item and add it to the collection of items.
        /// </summary>
        /// <param name="allItems">The collection of menu item in this set.
        /// The new item is added to this.</param>
        /// <param name="attribute">The attribute that defines this item.</param>
        /// <param name="handlerType">The class that handles invoking the menu item. May be null.</param>
        /// <returns>The new item.</returns>
        protected virtual MenuItemModel BuildItem(Dictionary<string,MenuItemModel> allItems, MenuAttributeBase attribute, Type handlerType)
        {
            string id = attribute.Id ?? Guid.NewGuid().ToString("N");   // generate a GUID ID if no ID is given
            var item = new MenuItemModel(allItems,
                id)       // create a menu item from the attribute
            {
                ParentId = attribute.ParentId,
                AcceleratorChar = attribute.AcceleratorChar,
                Order = attribute.Order,
                DisplayName = attribute.DisplayName ?? CaptionUtils.GetDisplayName(handlerType),
                HotKey = attribute.HotKey,
                IsMenu = attribute.IsMenu,
                HandlerType = (attribute as MenuItemInstanceAttribute)?.Handler ?? handlerType,   // not applicable to Menu (will be null)
                Params = attribute.Params,
                Rights = attribute.Rights,
                Filter = attribute.Filter,
                Attribute = attribute,
                // Could copy by reflection (wouldn't require a change when adding new properties of the attribute).
            };
            allItems.Add(id, item);
            return item;
        }

        /// <summary>
        /// Add delegate(s) to the item for when it is invoked.
        /// <para>Subclasses can provide custom strategies for handling specific handler class types.</para>
        /// </summary>
        /// <param name="item">The new menu item.</param>
        protected virtual void AddInvokeDelegate(MenuItemModel item)
        {
            if (item.HandlerType is IPresenter)
            {
                item.Invoked += MenuItemPresenter_Invoked;
            }

            //TODO: Other types: "Execute" method ?
        }

        /// <summary>
        /// Called when a menu item for a Presenter is invoked.
        /// </summary>
        /// <param name="item">The invoked menu item.</param>
        protected virtual void MenuItemPresenter_Invoked(MenuItemModel item)
        {
            Resolver.GetPresenterByType<IPresenter,object>(item.HandlerType, item.Params[0]).Show();
        }

        /// <summary>
        /// Menu Set to build.
        /// </summary>
        public virtual string MenuSetId { get; set; } = MenuAttributeBase.DefaultMenuSetId;

        /// <summary>
        /// MVP resolver used for resolving factories of presenters invoked by menu items.
        /// </summary>
        public virtual MvpResolver Resolver { get; set; }
    }
}
