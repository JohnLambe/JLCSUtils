using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JohnLambe.Util.PluginFramework.Interfaces;
using JohnLambe.Util.DependencyInjection;
using Microsoft.Practices.Unity;

namespace JohnLambe.Util.PluginFramework.Host
{
    // The Plugin Host should manage plugins, and facilitate communication between them only.

    /// <summary>
    /// Manages plugins - maintains details of loaded plugins and event handlers.
    /// There is one instance of this class in a plugin system.
    /// Only application initialisation code and extensions of this system should see
    /// this instance.
    /// The public interface of this enables configuration of the plugin system.
    /// </summary>
    [DiRegisterInstance(Priority = 1000)]   // Initialise early. All extensions must have a higher Priority value than this.
    public partial class PluginHost : IPluginHost
    {
        public PluginHost(EventSender eventSender = null)
        {
            this.Sender = eventSender ?? new EventSender(null);
        }

        public virtual void RegisterPluginClass(Type pluginClass)
        {
            PluginClasses.Add(pluginClass);

        }

        public virtual void InitializePlugins()
        {
            foreach(var pluginClass in PluginClasses)
            {

            }
        }

        /// <summary>
        /// Register and initialise a plugin instance.
        /// </summary>
        /// <param name="plugin"></param>
        public virtual void AddPlugin(object plugin)
        {
            // Set up the parameters for the initializers:
            var initializeParams = new PluginInitializeParams(this, PluginInitializeStage.Initialise)
            {
                Plugin = plugin,
                Details = new PluginDetails(plugin)
            };
            // Run all initializers:
            OnPluginInitialize?.Invoke(initializeParams);

            Plugins.Add(initializeParams.Details);
        }

        public virtual PluginCollection Plugins { get; protected set; }
            = new PluginCollection();

        protected virtual EventSender Sender { get; set; }

        public virtual event PluginInitializeDelegate OnPluginRegister;
        /// <summary>
        /// Implements <see cref="IPluginHost.OnPluginInitialize"/>.
        /// </summary>
        public virtual event PluginInitializeDelegate OnPluginInitialize;

        [Dependency]
        public virtual IDiContext DiContext { protected get; set; }

        protected IList<Type> PluginClasses { get; set; }
    }

}
