using JohnLambe.Util.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.PluginFramework.Interfaces
{
    public interface IPluginInitializer
    {
        bool Initialize(PluginInitializeStage stage, IDiContext diContext);
    }

    public enum PluginInitializeStage
    {
        PluginLoaded = 1,
        PluginInitialized,
        SystemInitialized,
        SystemFinalizing,
        PluginFinalize
    }
}
