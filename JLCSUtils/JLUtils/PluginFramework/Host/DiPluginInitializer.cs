using JohnLambe.Util.DependencyInjection;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.PluginFramework.Host
{
    /// <summary>
    /// Integrates plugins with the dependency injection framework.
    /// </summary>
    [DiRegisterInstance(Priority = 10000)]
    public class DiPluginInitializer : PluginInitializerBase
    {
        public DiPluginInitializer(IPluginHost host) : base(host)
        {
        }

        protected override void PluginInitialize(PluginInitializeParams parameters)
        {
            base.PluginInitialize(parameters);

            DiContext?.BuildUp(parameters.Plugin);
        }

        [Dependency]
        public virtual IDiContext DiContext { protected get; set; }
    }
}
