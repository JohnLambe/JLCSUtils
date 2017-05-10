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
        void UpdateOption(OptionUpdateArgs args);
    }

    public delegate void UpdateOptionDelegate(OptionUpdateArgs args);

    /// <summary>
    /// Details of an update to a command/option.
    /// </summary>
    public class OptionUpdateArgs
    {
        public virtual string Id { get; set; }
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

    public class OptionUpdateContext
    {
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
