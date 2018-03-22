using JohnLambe.Util;
using MvpFramework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Binding
{
    /// <summary>
    /// Interface for something that has commands/options and can process updates to them.
    /// </summary>
    public interface IOptionUpdate
    {
        /// <summary>
        /// Update zero or more options.
        /// </summary>
        /// <param name="args"></param>
        void UpdateOption(OptionUpdateArgs args);
    }

    /// <summary>
    /// Extension methods of <see cref="IOptionUpdate"/>.
    /// </summary>
    public static class OptionUpdateExtension
    {
        /// <summary>
        /// Enable or disable options.
        /// </summary>
        /// <param name="optionUpdate">The interface to use to the update.</param>
        /// <param name="id">Id to match <see cref="MenuItemModel.Id"/>. null to not use this.</param>
        /// <param name="filter">Filter value to match against <see cref="MenuItemModel.Filter"/>. null to not filter on this.</param>
        /// <param name="value">true to enable; false to disable.</param>
        public static void SetEnabled(this IOptionUpdate optionUpdate, string id, string filter, bool value)
        {
            optionUpdate.UpdateOption(new OptionUpdateArgs() { Filter = filter, Id = id, OnUpdate = args => args.Option.Enabled = value });
        }

        /// <summary>
        /// Hide or show options.
        /// </summary>
        /// <param name="optionUpdate">The interface to use to the update.</param>
        /// <param name="id">Id to match <see cref="MenuItemModel.Id"/>. null to not use this.</param>
        /// <param name="filter">Filter value to match against <see cref="MenuItemModel.Filter"/>. null to not filter on this.</param>
        /// <param name="value">The value to assign to the <see cref="MenuItemModel.Visible"/> property.</param>
        public static void SetVisible(this IOptionUpdate optionUpdate, string id, string filter, bool value)
        {
            optionUpdate.UpdateOption(new OptionUpdateArgs() { Filter = filter, Id = id, OnUpdate = args => args.Option.Visible = value });
        }

        /// <summary>
        /// Update options using a delegate.
        /// </summary>
        /// <param name="optionUpdate">The interface to use to the update.</param>
        /// <param name="id">Id to match <see cref="MenuItemModel.Id"/>. null to not use this.</param>
        /// <param name="filter">Filter value to match against <see cref="MenuItemModel.Filter"/>. null to not filter on this.</param>
        /// <param name="d">Delegate to run on each option.</param>
        public static void UpdateOptionByDelegate(this IOptionUpdate optionUpdate, string id, string filter, VoidDelegate<MenuItemModel> d)
        {
            optionUpdate.UpdateOption(new OptionUpdateArgs() { Filter = filter, Id = id, OnUpdate = args => d(args.Option) });
        }
    }

    /// <summary>
    /// Delegate equivalent to <see cref="IOptionUpdate.UpdateOption(OptionUpdateArgs)"/>.
    /// </summary>
    /// <param name="args"></param>
    public delegate void UpdateOptionDelegate(OptionUpdateArgs args);

    /// <summary>
    /// Details of an update to a command/option.
    /// </summary>
    //| Arguments class to support extension.
    public class OptionUpdateArgs
    {
        /// <summary>Id to match <see cref="MenuItemModel.Id"/>. null to not use this.</summary>
        public virtual string Id { get; set; }

        /// <summary>Filter value to match against <see cref="MenuItemModel.Filter"/>. null to not filter on this.</summary>
        public virtual string Filter { get; set; }

        /// <summary>
        /// Delegate to do the update on an individual command/option.
        /// </summary>
        public virtual UpdateDelegate OnUpdate { get; set; }

        /// <summary>
        /// Delegate to process an update on an individual command/option.
        /// </summary>
        /// <param name="context">Parameters, including the item to update.</param>
        public delegate void UpdateDelegate(OptionUpdateContext context);
    }

    /// <summary>
    /// Context for a delegate that updates an individual <see cref="MenuItemModel"/>.
    /// </summary>
    //| Arguments class to support extension.
    public class OptionUpdateContext
    {
        /// <summary>
        /// </summary>
        /// <param name="option">Value of <see cref="Option"/>.</param>
        public OptionUpdateContext(MenuItemModel option)
        {
            this.Option = option;
        }

        /// <summary>
        /// The item to be updated.
        /// </summary>
        public virtual MenuItemModel Option { get; }
    }
}
