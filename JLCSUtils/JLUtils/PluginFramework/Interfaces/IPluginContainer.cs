using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JohnLambe.Util.PluginFramework.Interfaces
{
    public interface IPlugin
    {
        // ID - property rather than attribute ?
        string Id { get; }
    }

    public interface IPluginContainer
    {
        IReadOnlyCollection<IPlugin> Children { get; }

        /// <summary>
        /// Add a child to this plugin.
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        bool RegisterPlugin(IPlugin plugin);

        // ProcessEvent(IPluginEvent);   ??
    }
}
