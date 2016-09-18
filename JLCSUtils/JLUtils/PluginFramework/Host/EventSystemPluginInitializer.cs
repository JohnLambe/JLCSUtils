using JohnLambe.Util.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.PluginFramework.Host
{
    /// <summary>
    /// Initialises plugin for using the event system.
    /// </summary>
    [DiRegisterInstance(Priority = 12000)]
    public class EventSystemPluginInitializer : PluginInitializerBase
    {
        public EventSystemPluginInitializer(IPluginHost host) : base(host)
        {
        }

        protected override void PluginInitialize(PluginInitializeParams parameters)
        {
            base.PluginInitialize(parameters);

            //TODO: Inject event sender
        }

    }
}
