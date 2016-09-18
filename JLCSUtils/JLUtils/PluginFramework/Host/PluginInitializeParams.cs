using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.PluginFramework.Host
{
//    public partial class PluginHost
//    {
    /// <summary>
    /// Parameters passed to extensions on initializing a plugin.
    /// </summary>
    public class PluginInitializeParams
    {
        public PluginInitializeParams(IPluginHost host, PluginInitializeStage stage)
        {
            this.Host = host;
            this.Stage = stage;
        }

        public IPluginHost Host { get; }
        public PluginInitializeStage Stage { get; }
        public object Plugin { get; set; }
        public PluginDetails Details { get; set; }
    }
    //    }

    /// <summary>
    /// <see cref="IPluginHost.OnPluginInitialize"/>.
    /// </summary>
    public delegate void PluginInitializeDelegate(PluginInitializeParams parameters);

    public enum PluginInitializeStage
    {
        Register,
        Initialise
    }
}
