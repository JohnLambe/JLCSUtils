using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JohnLambe.Util.Types;

namespace MvpFramework.Menu
{
    /// <summary>
    /// A collection of options, such as a list of commands (which might correspond to buttons etc.)
    /// or a menu.
    /// </summary>
    public interface IOptionCollection
    {
        /// <summary>
        /// The buttons or options that the user can choose, in the order in which they are displayed.
        /// null for default based on MessageType.
        /// </summary>
        [NotNull]
        IEnumerable<MenuItemModel> Children { get; }

        /// <summary>
        /// Make the item (in the children of this item) with the given <see cref="MenuItemModel.ReturnValue"/> the default.
        /// </summary>
        /// <param name="returnValue"></param>
        [return: Nullable]
        MenuItemModel SetDefaultByReturnValue(object returnValue);

        /// <summary>
        /// Invoke option(s) by their keyboard shortcut.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyType">Which keys to match.</param>
        /// <returns>true iff any option was invoked.</returns>
        bool ProcessKey(KeyboardKey key, KeyType keyType = KeyType.All);

        /// <summary>
        /// Invoke the option(s) with the given accelerator character.
        /// </summary>
        /// <param name="acceleratorCharacter"></param>
        /// <returns>true iff any option was invoked.</returns>
        bool ProcessAcceleratorChar(char acceleratorCharacter);

        /// <summary>
        /// The default option.
        /// null if there is no default.
        /// </summary>
        [Nullable]
        MenuItemModel Default { get; set; }

        /// <summary>
        /// Fired on certain changes to collection or an option in it.
        /// </summary>
        event MenuItemModel.ChangedDelegate Changed;
    }


    /// <summary>
    /// A type of shortcut key (relating to how it is processed).
    /// </summary>
    [Flags]
    public enum KeyType
    {
        /// <summary>
        /// </summary>
        /// <seealso cref="Binding.MvpUiAttributeBase.HotKey"/>
        HotKey = 1,

        /// <summary>
        /// Keystroke to invoke an item while the UI representation of a menu or related item is focussed.
        /// <para>
        /// This can be a key that would do other things in other contexts in which a <see cref="HotKey"/> could be used.
        /// e.g. the context key could be a letter key (with no modifier) that can be used while the input focus is on a menu, group of buttons, or list/grid related to the menu item (or button, etc.).
        /// This clearly couldn't be used when an edit box was focussed, whereas a HotKey of (for example) a function key, or the Control or Alt key with a letter, could.
        /// </para>
        /// </summary>
        /// <seealso cref="Binding.MvpUiAttributeBase.ContextKey"/>
        ContextKey = 2,

        All = HotKey | ContextKey
    }


    /// <summary>
    /// <inheritdoc cref="IOptionCollection"/>
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

        /// <summary>
        /// Add a given option to the collection.
        /// </summary>
        /// <param name="option">the option to be added.</param>
        /// <returns>the given option.</returns>
        public virtual MenuItemModel AddOption(MenuItemModel option)
        {
            _allItems[option.Id] = option;
            option.Parent = this;
            option.ParentId = Id;
            return option;
            //            _options = _options.OrderBy(o => o.Order).ToList();
        }

        /// <summary>
        /// Remove the given item from the collection.
        /// </summary>
        /// <param name="option"></param>
        /// <returns>true iff the item was removed; false if it was not in the collection.</returns>
        public virtual bool RemoveOption(MenuItemModel option)
        {
            return _allItems.Remove(option.Id);
        }

        /// <summary>
        /// Create a new option and add it to the collection.
        /// </summary>
        /// <param name="id">the <see cref="MenuItemModel.Id"/> of the new item.</param>
        /// <returns>the new option.</returns>
        public virtual MenuItemModel NewOption(string id)
        {
            var item = new MenuItemModel(_allItems, id);
            AddOption(item);
            return item;
        }

        //        public virtual IEnumerable<MenuItemModel> Options
        //            => base.Children; //.OrderBy(o => o.Order);

        /// <summary>
        /// Invoke the item(s) that the given keystroke is a shortcut for, if any.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyType"></param>
        /// <returns>true iff any item was invoked.</returns>
        public virtual bool ProcessKey(KeyboardKey key, KeyType keyType = KeyType.All)
        {
            bool handled = false;
            foreach (var option in Children.Where(c => (keyType.HasFlag(KeyType.HotKey) && c.HotKey == key)
                || (keyType.HasFlag(KeyType.ContextKey) && c.ContextKey == key)) )
            {
                option.Invoke();
                handled = true;
            }
            return handled;
        }

        /// <summary>
        /// Invoke the item(s) that the given keystroke is an accelerator character for, if any.
        /// </summary>
        /// <param name="acceleratorCharacter">the accelerator character to process.</param>
        /// <returns>true iff any item was invoked.</returns>
        public virtual bool ProcessAcceleratorChar(char acceleratorCharacter)
        {
            bool handled = false;
            foreach (var option in Children.Where(c => c.AcceleratorChar == acceleratorCharacter))
            {
                option.Invoke();
                handled = true;
            }
            return handled;
        }
    }

}
