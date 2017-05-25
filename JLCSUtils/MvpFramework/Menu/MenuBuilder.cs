using DiExtension;
using DiExtension.Attributes;
using JohnLambe.Util;
using JohnLambe.Util.Collections;
using JohnLambe.Util.Encoding;
using JohnLambe.Util.FilterDelegates;
using JohnLambe.Util.Misc;
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
        /// Create without an MVP resolver. Presenters will not be invoked automatically.
        /// </summary>
        public MenuBuilder()
        {
        }

        /// <summary>
        /// Create, with a resolver for invoking Presenters.
        /// </summary>
        /// <param name="resolver"></param>
        [Inject]
        public MenuBuilder(MvpResolver resolver, IDiResolver diResolver = null)
        {
            this.Resolver = resolver;
            this.DiResolver = diResolver;
        }

        /// <summary>
        /// Scans assemblies and builds menus based on attributes on handler classes.
        /// </summary>
        /// <param name="assemblies">Assemblies to scan. Defaults to the <see cref="Assemblies"/> property, and it if it is null, the calling assembly and all assemblies directly referenced by it.</param>
        /// <param name="filter"></param>
        /// <returns>The collection of menus.</returns>
        public virtual MenuCollection BuildMenu(IEnumerable<Assembly> assemblies = null, BooleanExpression<Type> filter = null)
        {
            if (assemblies == null)
                assemblies = Assemblies ?? AssemblyUtil.GetReferencedAssemblies(Assembly.GetCallingAssembly(), true);

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
                    item.Value.Parent = DictionaryUtil.TryGetValue(allItems, item.Value.ParentId).NotNull("Invalid parent for Menu item " + item.Value.CodeDescription + " - " + item.Value.ParentId + " not found");
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
                DisplayName = attribute.DisplayName ?? CaptionUtil.GetDisplayName(handlerType),
                HotKey = attribute.HotKey,
                IsMenu = attribute.IsMenu,
                HandlerType = (attribute as MenuItemInstanceAttribute)?.Handler ?? handlerType,   // not applicable to Menu (will be null)
                Params = attribute.Params,
                Rights = attribute.Rights,
                Filter = attribute.Filter,
                Attribute = attribute,
                //| Could copy by reflection (wouldn't require a change when adding new properties of the attribute).
            };
            if(attribute is GenerateMenuItemAttribute)
            {
                item.HandlerType = null;
                item.ModelType = handlerType;
                item.Invoked += InvokeGeneratedItem;// GeneratedItem_Invoked;
            }
            allItems.Add(id, item);
            return item;
        }

        /*
        /// <summary>
        /// Fired when a presenter for a generated view is requried.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="args"></param>
        protected virtual void GeneratedItem_Invoked(MenuItemModel item, MenuItemModel.InvokedEventArgs args)
        {
            
        }
        */

        /// <summary>
        /// Fired when a presenter for a generated view is requried.
        /// </summary>
        public virtual event MenuItemModel.InvokedDelegate InvokeGeneratedItem;

        /// <summary>
        /// Name of static method to be invoked (if present) on invoking a menu item.
        /// <para>If both this and <see cref="MenuExecuteInstanceMethodName"/> are present,
        /// this is called first, then the instance one is called unless this returns false.</para>
        /// </summary>
        protected const string MenuExecuteMethodName = "MenuExecute";
        /// <summary>
        /// Name of instance method to be invoked (if present) on invoking a menu item.
        /// <para>An instance is created from the DI container and this is called on it.</para>
        /// </summary>
        [Obsolete("Not implemented yet")]
        protected const string MenuExecuteInstanceMethodName = "Execute";

        /// <summary>
        /// Add delegate(s) to the item for when it is invoked.
        /// <para>Subclasses can provide custom strategies for handling specific handler class types.</para>
        /// </summary>
        /// <param name="item">The new menu item.</param>
        protected virtual void AddInvokeDelegate(MenuItemModel item)
        {
            if (Resolver != null && item.HandlerType != null)
            {
                MenuItemModel.InvokedDelegate instanceExecute = null;

                // Instance methods:

                if (typeof(IPresenter).IsAssignableFrom(item.HandlerType) || item.HandlerType.IsDefined<PresenterAttribute>())  // if the handler it is a Presenter
                {
                    instanceExecute = MenuItemPresenter_Invoked;
                }

                /*
                var instanceExecuteMethod = GeneralUtils.IgnoreException(
                    () => item.HandlerType.GetMethod(MenuExecuteInstanceMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreReturn),
                    typeof(AmbiguousMatchException)
                    );
                */


                // Static method:

                var staticExecuteMethod =
                    DiResolver == null ? null :                    // if no DiResover, don't try to use this method (alternatively, we could allow it if it doesn't have any parameters to be injected by DI)
                    GeneralUtil.IgnoreException(
                    () => item.HandlerType.GetMethod(MenuExecuteMethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreReturn),
                    typeof(AmbiguousMatchException)
                    );

                if (staticExecuteMethod != null)          // if there is a static execute method
                {
                    item.Invoked += (menuItem,args) => MenuItemExecute_Invoked(menuItem, args, staticExecuteMethod, instanceExecute);
                }
                else if (instanceExecute != null)                  // if instance method only
                {
                    item.Invoked += instanceExecute;               // call instance method directly
                }

            }

            //TODO: Other types: If instance method "Execute" exists, get instance from DI, and call it.
            // if Static Method 'MenuExecute' exists, invoke it, populating parameters by DI.
            // Support special parameters for menu state ?
        }

        /// <summary>
        /// Handles invoking of a menu item, when the handler is not an IPresenter.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="args"></param>
        /// <param name="method"></param>
        /// <param name="invoke"></param>
        protected virtual void MenuItemExecute_Invoked(MenuItemModel item, MenuItemModel.InvokedEventArgs args, MethodInfo method, MenuItemModel.InvokedDelegate invoke)
        {
            var result = DiUtil.CallMethod<object>(DiResolver,method,null, new object[] { item, args }, DiMvpResolver.AttributeSourceSelector);
            if (result is bool && !(bool)result)
            {
                // suppress creating an instance
            }
            else
            {
                invoke?.Invoke(item, args);
            }
        }

        /// <summary>
        /// Called when a menu item for a Presenter is invoked.
        /// </summary>
        /// <param name="item">The invoked menu item.</param>
        protected virtual void MenuItemPresenter_Invoked(MenuItemModel item, MenuItemModel.InvokedEventArgs args)
        {
            Resolver.GetPresenterByType<IPresenter>(item.HandlerType, item.Params).Show();
        }

        /// <summary>
        /// Menu Set to build.
        /// </summary>
        public virtual string MenuSetId { get; set; } = MenuAttributeBase.DefaultMenuSetId;

        /// <summary>
        /// MVP resolver used for resolving factories of presenters invoked by menu items.
        /// </summary>
        public virtual MvpResolver Resolver { get; protected set; }

        /// <summary>
        /// DI resolver. Used for resolving parameters on invoking handlers.
        /// </summary>
        public virtual IDiResolver DiResolver { get; protected set; }

        /// <summary>
        /// Assemblies to scan.
        /// </summary>
        public virtual IEnumerable<Assembly> Assemblies { get; set; }
    }
}
